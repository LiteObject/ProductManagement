using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ProductManagement.API;
using ProductManagement.Infra.Contexts;
using System.Net;
using System.Threading.RateLimiting;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ProductManagement.Test
{
    public class RateLimitingTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly ITestOutputHelper _output;
        
        public RateLimitingTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _output = output;
            _factory = factory.WithWebHostBuilder(builder =>
            {
                // Set the environment to Production
                builder.UseEnvironment("Production");

                builder.ConfigureServices(services =>
                {
                    // Remove all existing IDbContextOptionsConfiguration<ProductContext> registrations
                    ServiceDescriptor? serviceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<ProductContext>));
                    if (serviceDescriptor != null)
                    {
                        services.Remove(serviceDescriptor);
                    }

                    // Add DbContext using an in-memory database for testing
                    services.AddDbContext<ProductContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                    });

                    // Ensure the database is created
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ProductContext>();
                    db.Database.EnsureCreated();

                    // Remove the RateLimiter service
                    services.RemoveAll<RateLimiter>();

                    // Remove the RateLimiterOptions configuration
                    services.RemoveAll<IConfigureOptions<RateLimiterOptions>>();

                    // Re-add rate-limiting services with a new policy
                    services.AddRateLimiter(options =>
                    {
                        options.AddFixedWindowLimiter(policyName: "FixedPolicy", options =>
                        {
                            options.PermitLimit = 2; // Allow 2 requests
                            options.Window = TimeSpan.FromSeconds(10); // Per 10 seconds
                            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                            options.QueueLimit = 0;
                        }).OnRejected = (context, cancellationToken) =>
                        {
                            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                            return new ValueTask();
                        };
                    });
                });
            });
        }

        [Fact]
        public async Task RequestsExceedingLimit_ShouldReturn429()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = "/products";

            // Act: Send 3 requests (exceeding the limit of 2)
            var response1 = await client.GetAsync(request);
            var response2 = await client.GetAsync(request);
            var response3 = await client.GetAsync(request);

            _output.WriteLine($"Response 1 Status: {response1.StatusCode}");
            _output.WriteLine($"Response 2 Status: {response2.StatusCode}");
            _output.WriteLine($"Response 3 Status: {response3.StatusCode}");

            // Assert: The third request should return 429 Too Many Requests
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
            Assert.Equal(HttpStatusCode.TooManyRequests, response3.StatusCode);
        }

        [Fact]
        public async Task QueuedRequests_ShouldBeProcessed()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = "/products";

            // Act: Send 3 requests (2 allowed, 1 queued)
            var response1 = await client.GetAsync(request);
            var response2 = await client.GetAsync(request);
            var response3 = await client.GetAsync(request);

            _output.WriteLine($"Response 1 Status: {response1.StatusCode}");
            _output.WriteLine($"Response 2 Status: {response2.StatusCode}");
            _output.WriteLine($"Response 3 Status: {response3.StatusCode}");

            // Wait for the queue to process
            await Task.Delay(TimeSpan.FromSeconds(11));
            var response4 = await client.GetAsync(request);

            // Assert: The queued request should succeed after the window resets
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
            Assert.Equal(HttpStatusCode.TooManyRequests, response3.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response4.StatusCode);
        }
    }
}

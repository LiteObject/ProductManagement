using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ProductManagement.API;
using ProductManagement.Infra.Contexts;
using System.Net;
using Xunit;
using Xunit.Abstractions;

namespace ProductManagement.Test
{
    public class ExceptionHandlingTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly ITestOutputHelper _output;

        public ExceptionHandlingTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
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
                });
            });
        }

        [Fact]
        public async Task Test_ExceptionResponse_DoesNotContainStackTrace_InProduction()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/throw");
            var contentType = response.Content.Headers.ContentType?.ToString();

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine(content);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Contains("application/problem+json", contentType);
            Assert.DoesNotContain("at ", content);
        }
    }
}

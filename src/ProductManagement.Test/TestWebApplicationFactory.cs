using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using ProductManagement.Infra.Contexts;
using System.Threading.RateLimiting;

namespace ProductManagement.Test
{
    public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
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
                        options.QueueLimit = 1;
                    }).OnRejected = (context, cancellationToken) =>
                    {
                        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        return new ValueTask();
                    };
                });
            });
        }
    }
}

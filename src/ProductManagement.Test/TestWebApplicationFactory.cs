using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace ProductManagement.Test
{
    public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove all rate-limiting services
                var rateLimitingDescriptors = services
                    .Where(d =>               
                        d.ImplementationType != null 
                        && d.ImplementationType.FullName?.Contains("RateLimit") == true)
                    .ToList();

                foreach (var descriptor in rateLimitingDescriptors)
                {
                    services.Remove(descriptor);
                }

                // Re-add rate-limiting services with a new policy
                services.AddRateLimiter(options =>
                {
                    options.AddFixedWindowLimiter(policyName: "FixedPolicy", options =>
                    {
                        options.PermitLimit = 2; // Allow 2 requests
                        options.Window = TimeSpan.FromSeconds(10); // Per 10 seconds
                        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                        options.QueueLimit = 0; // No queuing allowed
                    });
                });
            });
        }
    }
}

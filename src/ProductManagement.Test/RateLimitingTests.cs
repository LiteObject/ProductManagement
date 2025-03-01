using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.RateLimiting;
using ProductManagement.API;
using System.Net;
using System.Threading.RateLimiting;
using Xunit;
using Xunit.Abstractions;

namespace ProductManagement.Test
{
    public class RateLimitingTests(TestWebApplicationFactory<Program> factory, ITestOutputHelper testOutput) : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory = factory;
        private readonly ITestOutputHelper _output = testOutput;
        
        [Fact]
        public async Task RequestsWithinLimit_ShouldSucceed()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = "/products";

            // Act: Send 2 requests (within the limit)
            var response1 = await client.GetAsync(request);
            var response2 = await client.GetAsync(request);

            // Assert: Both requests should succeed
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        }

        [Fact]
        public async Task RequestsExceedingLimit_ShouldReturn429()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = "/products";

            // Act: Send 3 requests (exceeding the limit of 2)
            var response1 = await client.GetAsync(request);
            await Task.Delay(2000); // Small delay to simulate realistic request timing
            var response2 = await client.GetAsync(request);
            await Task.Delay(3000); // Small delay to simulate realistic request timing
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

            _output.WriteLine($"Response Status 3: {response3.StatusCode}");

            // Wait for the queue to process
            await Task.Delay(TimeSpan.FromSeconds(11)); // Wait for the window to reset

            var response4 = await client.GetAsync(request);

            // Assert: The queued request should succeed after the window resets
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
            Assert.Equal(HttpStatusCode.TooManyRequests, response3.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response4.StatusCode);
        }
    }
}

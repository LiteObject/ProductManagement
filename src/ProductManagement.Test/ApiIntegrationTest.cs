using Microsoft.AspNetCore.Mvc.Testing;
using ProductManagement.API;
using Xunit;

namespace ProductManagement.Test
{
    public class ApiIntegrationTest(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory = factory;

        [Fact]
        public async Task Get_Products_ReturnsSuccessStatusCode()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = "/products";

            // Act
            var response = await client.GetAsync(request);

            // Assert
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}). Content: {errorContent}");
            }

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseString);
        }
    }
}

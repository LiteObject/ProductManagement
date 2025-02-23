using Microsoft.AspNetCore.Mvc.Testing;
using ProductManagement.API;
using Xunit;
using Xunit.Abstractions;

namespace ProductManagement.Test
{
    public class ApiIntegrationTest(WebApplicationFactory<Program> factory, ITestOutputHelper testOutput) : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory = factory;
        private readonly ITestOutputHelper _output = testOutput;

        [Fact]
        public async Task Get_Products_ReturnsSuccessStatusCode()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = "/products";

            // Act
            HttpResponseMessage response = await client.GetAsync(request);
            _output.WriteLine($"Response Status: {response.StatusCode}, Response Content: {await response.Content.ReadAsStringAsync()}");

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

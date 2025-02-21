using Microsoft.EntityFrameworkCore;
using Moq;
using ProductManagement.Core.Entities;
using ProductManagement.Infra.Contexts;
using ProductManagement.Infra.Repositories;
using Xunit;
using Xunit.Abstractions;

namespace ProductManagement.Test
{
    public class ProductRepositoryTests
    {
        private readonly ProductContext _context;
        private readonly ProductRepository _productRepository;
        private readonly ITestOutputHelper output;
        public ProductRepositoryTests(ITestOutputHelper output)
        {
            this.output = output;

            var options = new DbContextOptionsBuilder<ProductContext>()
                 .UseInMemoryDatabase(databaseName: "TestDatabase")
                 .Options;

            _context = new ProductContext(options);
            _productRepository = new ProductRepository(_context);

            // Seed the in-memory database with test data
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var products = new List<Product>
            {
                Product.Create("Product 1", 10, "Description 1"),
                Product.Create("Product 2", 20, "Description 2")
            };

            _context.Products.AddRange(products);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetProducts_ReturnsAllProducts()
        {
            // Act
            var products = await _productRepository.GetAllAsync();

            // Assert
            Assert.NotNull(products);
            Assert.NotEmpty(products);
            Assert.IsType<IEnumerable<Product>>(products, exactMatch: false);
        }
    }
}

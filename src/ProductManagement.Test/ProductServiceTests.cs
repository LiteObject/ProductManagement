using AutoMapper;
using Moq;
using ProductManagement.App.DTOs;
using ProductManagement.App.Profiles;
using ProductManagement.App.Services;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace ProductManagement.Test
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly IMapper _mapper;
        // CA1859 suggests changing the type to the concrete class ProductService for improved performance.
        private readonly ProductService _productService;
        private readonly ITestOutputHelper output;

        public ProductServiceTests(ITestOutputHelper output)
        {
            this.output = output;

            _mockProductRepository = new Mock<IProductRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductMappingProfile>();
            });

            _mapper = config.CreateMapper();

            ILoggerFactory loggerFactory = new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory();
            var logger = loggerFactory.CreateLogger<ProductService>();

            _productService = new ProductService(_mockProductRepository.Object, _mapper, logger);

            // Set up mock data
            var products = new List<Product>
            {
                Product.Create("Product 1", 10, "Description 1"),
                Product.Create("Product 2", 20, "Description 2")
            };

            _mockProductRepository.Setup(repo => repo.GetAll()).Returns(products);
            _mockProductRepository.Setup(repo => repo.GetById(It.IsAny<int>())).Returns((int id) => products.FirstOrDefault(p => p.Id == id));
        }

        [Fact]
        public void Test_GetAllProduct()
        {
            // Act
            IEnumerable<ProductDto> products = _productService.GetAllProducts();

            // Assert
            Assert.NotNull(products);
            Assert.NotEmpty(products);
            Assert.IsType<IEnumerable<ProductDto>>(products, exactMatch: false);
        }

        [Fact]
        public void Test_GetProductById()
        {
            // Arrange
            var product = _productService.GetAllProducts().FirstOrDefault();

            // Act
            ProductDto? productDto = _productService.GetProductById(product?.Id ?? 0);
            output.WriteLine("The product {0}", productDto);

            // Assert
            Assert.NotNull(productDto);
            Assert.IsType<ProductDto>(productDto, exactMatch: false);
            Assert.Equal(product?.Id, productDto.Id);
            Assert.Equal(product?.Name, productDto.Name);
            Assert.Equal(product?.Price, productDto.Price);
            Assert.Equal(product?.Description, productDto.Description);
        }
    }
}




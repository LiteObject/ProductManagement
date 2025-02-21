using AutoMapper;
using Microsoft.VisualStudio.TestPlatform.Utilities;
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
        private readonly ITestOutputHelper _output;

        public ProductServiceTests(ITestOutputHelper output)
        {
            this._output = output;

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
            _output.WriteLine("The product {0}", productDto);

            // Assert
            Assert.NotNull(productDto);
            Assert.IsType<ProductDto>(productDto, exactMatch: false);
            Assert.Equal(product?.Id, productDto.Id);
            Assert.Equal(product?.Name, productDto.Name);
            Assert.Equal(product?.Price, productDto.Price);
            Assert.Equal(product?.Description, productDto.Description);
        }

        [Fact]
        public void Test_AddProduct()
        {
            // Arrange
            var createProductDto = new CreateProductDto
            {
                Name = "New Product",
                Price = 30,
                Description = "New Product Description"
            };

            var newProduct = Product.Create(createProductDto.Name, createProductDto.Price, createProductDto.Description);
            var newProductId = 3; // Simulate the new product ID

            _mockProductRepository.Setup(repo => repo.Add(It.IsAny<Product>()))
                .Callback<Product>(p => p.GetType().GetProperty("Id", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(p, newProductId)); // Simulate setting the ID

            // Act
            int addedProductId = _productService.AddProduct(createProductDto);
            _output.WriteLine("The new product ID is {0}", addedProductId);

            // Assert
            _mockProductRepository.Verify(repo => repo.Add(It.IsAny<Product>()), Times.Once);
            Assert.True(newProductId > 0);
        }

        [Fact]
        public void Test_UpdateProduct()
        {
            // Arrange
            var updateProductDto = new UpdateProductDto
            {
                Id = 1,
                Name = "Updated Product",
                Price = 40,
                Description = "Updated Product Description"
            };

            // Act
            _productService.UpdateProduct(updateProductDto);

            // Assert
            _mockProductRepository.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public void Test_DeleteProduct()
        {
            // Arrange
            var product = _productService.GetAllProducts().FirstOrDefault();

            // Act
            _productService.DeleteProduct(product?.Id ?? 0);

            // Assert
            _mockProductRepository.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Once);
        }
    }
}




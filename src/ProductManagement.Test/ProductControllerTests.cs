﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProductManagement.API.Controllers;
using ProductManagement.App.DTOs;
using ProductManagement.App.Services;
using ProductManagement.Core.Models;
using Xunit;

namespace ProductManagement.Test
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly ProductsController _productsController;

        public ProductsControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _productsController = new ProductsController(_mockProductService.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAsync_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            var products = new List<ProductDto>
            {
                new() { Id = 1, Name = "Product 1", Price = 10, Description = "Description One" },
                new() { Id = 2, Name = "Product 2", Price = 20, Description = "Description Two" }
            };

            var paginationMetadata = new PaginationMetadata(100, 1, 10);
            

            _mockProductService.Setup(service => service.GetAllProductsAsync(null, 1, 10)).ReturnsAsync((products, paginationMetadata));

            // Act
            var result = await _productsController.GetAsync(null, 1, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProducts = Assert.IsType<List<ProductDto>>(okResult.Value);
            Assert.Equal(2, returnProducts.Count);
        }

        [Fact]
        public async Task GetProductByIdAsync_ReturnsOkResult_WithProduct()
        {
            // Arrange
            var product = new ProductDto { Id = 1, Name = "Product 1", Price = 10, Description = "Description 1" };
            _mockProductService.Setup(service => service.GetProductByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _productsController.GetProductByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProduct = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal(1, returnProduct.Id);
        }

        [Fact]
        public async Task GetProductByIdAsync_ReturnsNotFoundResult_WhenProductNotFound()
        {
            // Arrange
            _mockProductService.Setup(service => service.GetProductByIdAsync(1)).ReturnsAsync((ProductDto?)null);

            // Act
            var result = await _productsController.GetProductByIdAsync(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PostAsync_ReturnsCreatedAtRouteResult_WithProductId()
        {
            // Arrange
            var createProductDto = new CreateProductDto { Name = "New Product", Price = 30, Description = "New Product Description" };
            _mockProductService.Setup(service => service.AddProductAsync(createProductDto)).ReturnsAsync(1);

            // Act
            var result = await _productsController.PostAsync(createProductDto);

            // Assert
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result);
            Assert.NotNull(createdAtRouteResult.RouteValues);
            Assert.Equal(1, createdAtRouteResult.RouteValues["id"]);
        }

        [Fact]
        public async Task Put_ReturnsNoContentResult()
        {
            // Arrange
            var updateProductDto = new UpdateProductDto { Id = 1, Name = "Updated Product", Price = 40, Description = "Updated Product Description" };

            // Act
            var result = await _productsController.PutAsync(1, updateProductDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Put_ReturnsBadRequestResult_WhenIdMismatch()
        {
            // Arrange
            var updateProductDto = new UpdateProductDto { Id = 1, Name = "Updated Product", Price = 40, Description = "Updated Product Description" };

            // Act
            var result = await _productsController.PutAsync(2, updateProductDto);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}

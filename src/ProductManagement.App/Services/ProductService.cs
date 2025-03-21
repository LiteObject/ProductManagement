﻿using AutoMapper;
using Microsoft.Extensions.Logging;
using ProductManagement.App.DTOs;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Interfaces;
using ProductManagement.Core.Models;
using System.Linq.Expressions;

namespace ProductManagement.App.Services
{
    /// <summary>
    ///  The ProductService plays a crucial role in encapsulating business logic and 
    ///  ensuring that the application remains modular, maintainable, and testable.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepo, IMapper mapper, ILogger<ProductService> logger)
        {
            _productRepository = productRepo;
            _mapper = mapper;
            _logger = logger;

            _logger.LogProductServiceInstantiated();
        }

        public async Task<(IEnumerable<ProductDto>, PaginationMetadata)> GetAllProductsAsync(string? searchKeyword, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogGetAllProductsInvoked();
            var (products, paginationMetadata) = await _productRepository.GetAllAsync(searchKeyword, pageNumber, pageSize).ConfigureAwait(false);
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return (productDtos, paginationMetadata);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            _logger.LogGetProductByIdInvoked(id);
            var product = await _productRepository.GetByIdAsync(id).ConfigureAwait(false);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<int> AddProductAsync(CreateProductDto productDto)
        {
            _logger.LogAddProductInvoked();
            var product = _mapper.Map<Product>(productDto);
            await _productRepository.AddAsync(product).ConfigureAwait(false);
            return product.Id;
        }

        public async Task UpdateProductAsync(UpdateProductDto productDto)
        {
            _logger.LogUpdateProductInvoked();
            var product = _mapper.Map<Product>(productDto);

            // TODO: check if the generated sql override with null/empty,
            // if so, try _mapper.Map(productDto, product)
            await _productRepository.UpdateAsync(product).ConfigureAwait(false);
        }

        public async Task DeleteProductAsync(int id)
        {
            _logger.LogDeleteProductInvoked(id);
            var product = await _productRepository.GetByIdAsync(id).ConfigureAwait(false);
            if (product != null)
            {
                await _productRepository.DeleteAsync(product).ConfigureAwait(false);
            }
        }
    }

    public static class ProductServiceLogMessages
    {
        private static readonly Action<ILogger, Exception?> _productServiceInstantiated =
            LoggerMessage.Define(LogLevel.Information, new EventId(1, nameof(ProductService)), "ProductService instantiated");

        private static readonly Action<ILogger, Exception?> _getAllProductsInvoked =
            LoggerMessage.Define(LogLevel.Information, new EventId(2, nameof(ProductService.GetAllProductsAsync)), $"{nameof(ProductService.GetAllProductsAsync)} invoked");

        private static readonly Action<ILogger, int, Exception?> _getProductByIdInvoked =
            LoggerMessage.Define<int>(LogLevel.Information, new EventId(3, nameof(ProductService.GetProductByIdAsync)), $"{nameof(ProductService.GetProductByIdAsync)} invoked with ID: {{Id}}");

        private static readonly Action<ILogger, Exception?> _addProductInvoked =
            LoggerMessage.Define(LogLevel.Information, new EventId(4, nameof(ProductService.AddProductAsync)), $"{nameof(ProductService.AddProductAsync)} invoked");

        private static readonly Action<ILogger, Exception?> _updateProductInvoked =
            LoggerMessage.Define(LogLevel.Information, new EventId(5, nameof(ProductService.UpdateProductAsync)), $"{nameof(ProductService.UpdateProductAsync)} invoked");

        private static readonly Action<ILogger, int, Exception?> _deleteProductInvoked =
            LoggerMessage.Define<int>(LogLevel.Information, new EventId(6, nameof(ProductService.DeleteProductAsync)), $"{nameof(ProductService.DeleteProductAsync)} invoked with ID: {{Id}}");


        public static void LogProductServiceInstantiated(this ILogger logger) => _productServiceInstantiated(logger, null);
        public static void LogGetAllProductsInvoked(this ILogger logger) => _getAllProductsInvoked(logger, null);
        public static void LogGetProductByIdInvoked(this ILogger logger, int id) => _getProductByIdInvoked(logger, id, null);
        public static void LogAddProductInvoked(this ILogger logger) => _addProductInvoked(logger, null);
        public static void LogUpdateProductInvoked(this ILogger logger) => _updateProductInvoked(logger, null);
        public static void LogDeleteProductInvoked(this ILogger logger, int id) => _deleteProductInvoked(logger, id, null);
    }
}

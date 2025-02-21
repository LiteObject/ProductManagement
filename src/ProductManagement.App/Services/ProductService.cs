using AutoMapper;
using Microsoft.Extensions.Logging;
using ProductManagement.App.DTOs;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Interfaces;

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

        public IEnumerable<ProductDto> GetAllProducts()
        {
            _logger.LogGetAllProductsInvoked();
            IEnumerable<Product> products = _productRepository.GetAll();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public ProductDto? GetProductById(int id)
        {
            _logger.LogGetProductByIdInvoked(id);
            var product = _productRepository.GetById<int>(id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public int AddProduct(CreateProductDto productDto)
        {
            _logger.LogAddProductInvoked();
            var product = _mapper.Map<Product>(productDto);
            _productRepository.Add(product);
            return product.Id;
        }

        public void UpdateProduct(UpdateProductDto productDto)
        {
            _logger.LogUpdateProductInvoked();
            var product = _mapper.Map<Product>(productDto);
            _productRepository.Update(product);
        }

        public void DeleteProduct(int id)
        {
            _logger.LogDeleteProductInvoked(id);
            var product = _productRepository.GetById<int>(id);
            if (product != null)
            {
                _productRepository.Delete(product);
            }
        }
    }

    public static class ProductServiceLogMessages
    {
        private static readonly Action<ILogger, Exception?> _productServiceInstantiated =
            LoggerMessage.Define(LogLevel.Information, new EventId(1, nameof(ProductService)), "ProductService instantiated");

        private static readonly Action<ILogger, Exception?> _getAllProductsInvoked =
            LoggerMessage.Define(LogLevel.Information, new EventId(2, nameof(ProductService.GetAllProducts)), $"{nameof(ProductService.GetAllProducts)} invoked");

        private static readonly Action<ILogger, int, Exception?> _getProductByIdInvoked =
            LoggerMessage.Define<int>(LogLevel.Information, new EventId(3, nameof(ProductService.GetProductById)), $"{nameof(ProductService.GetProductById)} invoked with ID: {{Id}}");

        private static readonly Action<ILogger, Exception?> _addProductInvoked =
            LoggerMessage.Define(LogLevel.Information, new EventId(4, nameof(ProductService.AddProduct)), $"{nameof(ProductService.AddProduct)} invoked");

        private static readonly Action<ILogger, Exception?> _updateProductInvoked =
            LoggerMessage.Define(LogLevel.Information, new EventId(5, nameof(ProductService.UpdateProduct)), $"{nameof(ProductService.UpdateProduct)} invoked");

        private static readonly Action<ILogger, int, Exception?> _deleteProductInvoked =
            LoggerMessage.Define<int>(LogLevel.Information, new EventId(6, nameof(ProductService.DeleteProduct)), $"{nameof(ProductService.DeleteProduct)} invoked with ID: {{Id}}");


        public static void LogProductServiceInstantiated(this ILogger logger) => _productServiceInstantiated(logger, null);
        public static void LogGetAllProductsInvoked(this ILogger logger) => _getAllProductsInvoked(logger, null);
        public static void LogGetProductByIdInvoked(this ILogger logger, int id) => _getProductByIdInvoked(logger, id, null);
        public static void LogAddProductInvoked(this ILogger logger) => _addProductInvoked(logger, null);
        public static void LogUpdateProductInvoked(this ILogger logger) => _updateProductInvoked(logger, null);
        public static void LogDeleteProductInvoked(this ILogger logger, int id) => _deleteProductInvoked(logger, id, null);
    }
}

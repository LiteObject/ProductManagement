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

            _logger.LogInformation("ProductService instantiated");
        }

        public IEnumerable<ProductDto> GetAllProducts()
        {
            _logger.LogInformation($"{nameof(GetAllProducts)} invoked");
            IEnumerable<Product> products = _productRepository.GetAll();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public ProductDto? GetProductById(int id)
        {
            _logger.LogInformation($"{nameof(GetProductById)} invoked with ID: {id}");
            var product = _productRepository.GetById<int>(id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public int AddProduct(ProductDto productDto)
        {
            _logger.LogInformation($"{nameof(AddProduct)} invoked");
            var product = _mapper.Map<Product>(productDto);
            _productRepository.Add(product);
            return product.Id;
        }

        public void UpdateProduct(ProductDto productDto)
        {
            _logger.LogInformation($"{nameof(UpdateProduct)} invoked");
            var product = _mapper.Map<Product>(productDto);
            _productRepository.Update(product);
        }

        public void DeleteProduct(int id)
        {
            _logger.LogInformation($"{nameof(DeleteProduct)} invoked with ID: {id}");
            var product = _productRepository.GetById<int>(id);
            if (product != null)
            {
                _productRepository.Delete(product);
            }
        }
    }
}

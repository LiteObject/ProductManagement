using ProductManagement.App.DTOs;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Interfaces;
using ProductManagement.Core.Models;

namespace ProductManagement.App.Services
{
    /// <summary>
    ///  The ProductService plays a crucial role in encapsulating business logic and 
    ///  ensuring that the application remains modular, maintainable, and testable.
    /// </summary>
    public interface IProductService
    {
        Task<(IEnumerable<ProductDto>, PaginationMetadata)> GetAllProductsAsync(string? searchKeyword, int pageNumber = 1, int pageSize = 10);
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<int> AddProductAsync(CreateProductDto productDto);
        Task UpdateProductAsync(UpdateProductDto productDto);
        Task DeleteProductAsync(int id);
    }
}


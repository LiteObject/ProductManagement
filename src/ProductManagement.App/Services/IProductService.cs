using ProductManagement.App.DTOs;

namespace ProductManagement.App.Services
{
    /// <summary>
    ///  The ProductService plays a crucial role in encapsulating business logic and 
    ///  ensuring that the application remains modular, maintainable, and testable.
    /// </summary>
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<int> AddProductAsync(CreateProductDto productDto);
        Task UpdateProductAsync(UpdateProductDto productDto);
        Task DeleteProductAsync(int id);
    }
}


using ProductManagement.App.DTOs;

namespace ProductManagement.App.Services
{
    /// <summary>
    ///  The ProductService plays a crucial role in encapsulating business logic and 
    ///  ensuring that the application remains modular, maintainable, and testable.
    /// </summary>
    public interface IProductService
    {
        IEnumerable<ProductDto> GetAllProducts();
        ProductDto? GetProductById(int id);
        int AddProduct(CreateProductDto productDto);
        void UpdateProduct(UpdateProductDto productDto);
        void DeleteProduct(int id);
    }
}


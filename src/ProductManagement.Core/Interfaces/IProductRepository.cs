using ProductManagement.Core.Entities;
using ProductManagement.Core.Models;

namespace ProductManagement.Core.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<(IEnumerable<Product>, PaginationMetadata)> GetAllAsync(string? searchKeyword, int pageNumber = 1, int pageSize = 10);
    }
}

using Microsoft.EntityFrameworkCore;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Interfaces;
using ProductManagement.Core.Models;
using ProductManagement.Infra.Contexts;

namespace ProductManagement.Infra.Repositories
{

    /// <summary>
    /// The ProductRepository plays a specific role in the project by providing a 
    /// concrete implementation of the IProductRepository interface, which extends 
    /// the generic repository functionality for the Product entity. It serves as 
    /// a bridge between the data access layer and the business logic layer, 
    /// ensuring that data operations related to Product entities are handled 
    /// efficiently and consistently.
    /// </summary>
    public class ProductRepository(ProductContext context) :
        GenericRepository<Product, ProductContext>(context), 
        IProductRepository
    {
        public async Task<(IEnumerable<Product>, PaginationMetadata)> GetAllAsync(string? searchKeyword, int pageNumber = 1, int pageSize = 10)
        {
            IQueryable<Product> collection = _context.Products;

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                searchKeyword = searchKeyword.Trim();
                collection = collection.Where(e => e.Name.Contains(searchKeyword) 
                    || (e.Description != null && e.Description.Contains(searchKeyword)));
            }

            int totalItems = await collection.CountAsync().ConfigureAwait(false);

            PaginationMetadata paginationMetadata = new (totalItems, pageNumber, pageSize);

            var products = await collection.OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync()
                .ConfigureAwait(false);

            return (products, paginationMetadata);
        }
    }
}

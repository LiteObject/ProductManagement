using System.ComponentModel.DataAnnotations;

namespace ProductManagement.App.DTOs
{
    /// <summary>
    /// Data Transfer Object (DTO) for the Product entity.
    /// </summary>
    public class ProductDto
    {        
        public int Id { get; set; }

        public string? Name { get; set; }

        public decimal Price { get; set; }
       
        public string? Description { get; set; }

        public string? LastModifiedOn { get; set; }
    }
}

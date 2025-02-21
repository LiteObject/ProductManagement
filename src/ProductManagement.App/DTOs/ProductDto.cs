using System.ComponentModel.DataAnnotations;

namespace ProductManagement.App.DTOs
{
    public class ProductDto
    {        
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }
        public string? Description { get; set; }
    }
}

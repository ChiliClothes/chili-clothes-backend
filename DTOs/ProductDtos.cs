using System.ComponentModel.DataAnnotations;

namespace ChiliClothes.DTOs
{
    public class ProductDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ChiliClothes.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        public int Stock { get; set; } = 0;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

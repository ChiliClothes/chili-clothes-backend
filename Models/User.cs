using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ChiliClothes.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;
        
        public string Role { get; set; } = "USER"; // USER or ADMIN
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}

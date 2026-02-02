using System.Text.Json.Serialization;

namespace ChiliClothes.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        
        [JsonIgnore]
        public User? User { get; set; }
        
        public string Status { get; set; } = "PENDING"; // PENDING, PREPARING, DELIVERED, CANCELLED
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

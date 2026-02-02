using System.ComponentModel.DataAnnotations;

namespace ChiliClothes.DTOs
{
    public class CreateOrderItemDto
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    public class CreateOrderDto
    {
        [Required]
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
    }
    
    public class UpdateOrderStatusDto {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}

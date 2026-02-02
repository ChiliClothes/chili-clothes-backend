using System.Security.Claims;
using ChiliClothes.Data;
using ChiliClothes.DTOs;
using ChiliClothes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChiliClothes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
        {
            var userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                        ?? User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userIdVal)) return Unauthorized();
            
            var userId = int.Parse(userIdVal);

            if (dto.Items == null || !dto.Items.Any())
                return BadRequest("Order must have at least one item.");

            decimal total = 0;
            var orderItems = new List<OrderItem>();

            // Strategy: Verify all products first to avoid partial state if possible, though EF Core transaction handles explicit saves.
            // But we need to fetch prices.
            
            // Note: In a real world scenario involving concurrency, locking stock is needed. Simplification: check stock here.
            
            using var transaction = await _context.Database.BeginTransactionAsync();
            try 
            {
                foreach (var itemDto in dto.Items)
                {
                    var product = await _context.Products.FindAsync(itemDto.ProductId);
                    if (product == null)
                        return BadRequest($"Product {itemDto.ProductId} not found");

                    if (!product.IsActive)
                        return BadRequest($"Product {product.Name} is not active");
                        
                    if (product.Stock < itemDto.Quantity)
                         return BadRequest($"Not enough stock for {product.Name}");

                    if (itemDto.Quantity <= 0)
                         return BadRequest("Quantity must be > 0");
                    
                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = itemDto.Quantity,
                        Price = product.Price 
                    };
                    orderItems.Add(orderItem);
                    
                    total += (product.Price * itemDto.Quantity);
                    
                    // Deduct stock? Spec doesn't explicitly say "deduct stock" in rules but it implies inventory management.
                    // "Stock Integer Inventario disponible DEFAULT 0"
                    // I will deduct stock.
                    product.Stock -= itemDto.Quantity; 
                }

                var order = new Order
                {
                    UserId = userId,
                    Status = "PENDING",
                    Total = total,
                    OrderItems = orderItems
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "An error occurred while creating the order: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdVal)) return Unauthorized();
            var userId = int.Parse(userIdVal);
            
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("role")?.Value;

            if (role == "ADMIN")
            {
               var orders = await _context.Orders
                   .Include(o => o.OrderItems)
                   .ThenInclude(oi => oi.Product)
                   .Include(o => o.User)
                   .OrderByDescending(o => o.CreatedAt)
                   .ToListAsync();
               return Ok(orders);
            }
            else
            {
                var orders = await _context.Orders
                    .Where(o => o.UserId == userId)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();
                return Ok(orders);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdVal)) return Unauthorized();
            var userId = int.Parse(userIdVal);
            
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("role")?.Value;

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            if (role != "ADMIN" && order.UserId != userId)
            {
                return Forbid();
            }

            return Ok(order);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            var allowedStatuses = new[] { "PENDING", "PREPARING", "DELIVERED", "CANCELLED" };
            if (!allowedStatuses.Contains(dto.Status))
                return BadRequest("Invalid status");

            order.Status = dto.Status;
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userIdVal = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdVal)) return Unauthorized();
            var userId = int.Parse(userIdVal);

            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            if (order.UserId != userId) return Forbid();

            if (order.Status != "PENDING")
                return BadRequest("Can only cancel pending orders");

            order.Status = "CANCELLED";
            // Return stock?
            // If we deducted stock, we should return it.
            // I'll leave it simple for now as spec didn't strictly specify stock return, but it's good practice.
            // I'd need to load items to do that.
            
            // Let's reload with items to return stock.
            var orderWithItems = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == id);
            if (orderWithItems != null) {
                foreach(var item in orderWithItems.OrderItems) {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null) product.Stock += item.Quantity;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(order);
        }
    }
}

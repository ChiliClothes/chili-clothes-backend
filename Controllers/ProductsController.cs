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
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Where(p => p.IsActive) 
                .ToListAsync();
                
            return Ok(products);
        }
        
        [HttpGet("admin/all")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllProductsForAdmin()
        {
             var products = await _context.Products.ToListAsync();
             return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateProduct(ProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                IsActive = dto.IsActive
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateProduct(int id, ProductDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.Include(p => p.OrderItems).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            if (product.OrderItems.Any())
            {
                product.IsActive = false;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Product deactivated because it has orders" });
            }

            product.IsActive = false;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Product deactivated" });
        }
    }
}

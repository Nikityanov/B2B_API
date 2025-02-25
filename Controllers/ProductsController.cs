using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2B_API.Models;
using B2B_API.Models.DTO;
using B2B_API.Interfaces;
using B2B_API.Services;
using B2B_API.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace B2B_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IGenericRepository<Product> _repository;
        private readonly ApplicationDbContext _context;

        public ProductsController(IGenericRepository<Product> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts(int? categoryId)
        {
            var query = _repository.GetAllAsync().Result.AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var products = await query.ToListAsync(); // Use await and ToListAsync
            return Ok(products.Select(p => p.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product.ToDto());
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<ProductResponseDto>> CreateProduct([FromForm] ProductCreateDto createDto)
        {
            
            var product = createDto.ToEntity();
            product.ImageUrl = null; // Игнорируем загрузку изображений пока что
            try
            {
                await _repository.AddAsync(product);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving product to database: {ex}");
                return StatusCode(500, "Ошибка при сохранении продукта в базе данных");
            }

            return CreatedAtAction(
                nameof(GetProduct),
                new { id = product.Id },
                product.ToDto());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")]
        [Consumes("application/json")] // Specify that this action consumes JSON
        public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDto updateDto)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.UpdateFromDto(updateDto);
            _repository.Update(product);
            var result = await _repository.SaveChangesAsync();

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _repository.Remove(product);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("BuyerPriceList")]
        [Authorize(Roles = "Buyer")] // Endpoint только для покупателей
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetBuyerPriceListProducts()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userRoleClaim = User.FindFirst(ClaimTypes.Role);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Forbid(); // Или BadRequest("Invalid user ID");
            }

            string? userRole = userRoleClaim?.Value;

            if (userRole != "Buyer")
            {
                return Forbid(); // Endpoint только для покупателей
            }

            var priceLists = await _context.PriceLists
                .Include(p => p.Products)
                    .ThenInclude(pp => pp.Product)
                .Include(p => p.AllowedBuyers) // Добавлено Include для AllowedBuyers
                .Where(p => p.AllowedBuyers.Any(b => b.Id == userId))
                .ToListAsync();

            var products = new List<ProductResponseDto>();
            foreach (var priceList in priceLists)
            {
                foreach (var priceListProduct in priceList.Products)
                {
                    var productDto = priceListProduct.Product.ToDto();
                    productDto.Price = priceListProduct.SpecialPrice; // Use special price from price list
                    products.Add(productDto);
                }
            }

            return Ok(products);
        }

        [HttpPost("{id}/images")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> AddImageToProduct(int id, [FromBody] string imageUrl)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(imageUrl))
            {
                return BadRequest("Image URL is required");
            }

            if (product.ImageGallery == null)
            {
                product.ImageGallery = new List<string>();
            }

            product.ImageGallery.Add(imageUrl);
            _repository.Update(product);
            await _repository.SaveChangesAsync();

            return Ok("Image added successfully");
        }

        [HttpDelete("{id}/images")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> RemoveImageFromProduct(int id, [FromBody] string imageUrl)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(imageUrl))
            {
                return BadRequest("Image URL is required");
            }

            if (product.ImageGallery == null || !product.ImageGallery.Contains(imageUrl))
            {
                return NotFound("Image URL not found in product gallery");
            }

            product.ImageGallery.Remove(imageUrl);
            _repository.Update(product);
            await _repository.SaveChangesAsync();

            return Ok("Image removed successfully");
        }

        [HttpPut("{id}/image")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdateProductImage(int id, [FromBody] string imageUrl)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(imageUrl))
            {
                return BadRequest("Image URL is required");
            }

            product.ImageUrl = imageUrl;
            _repository.Update(product);
            await _repository.SaveChangesAsync();

            return Ok("Product image updated successfully");
        }

        [HttpPut("{id}/characteristics")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdateProductCharacteristics(int id, [FromBody] string characteristics)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(characteristics))
            {
                return BadRequest("Product characteristics are required");
            }

            product.Characteristics = characteristics;
            _repository.Update(product);
            await _repository.SaveChangesAsync();

            return Ok("Product characteristics updated successfully");
        }

        [HttpGet("SellerProducts")]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetSellerProducts()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Forbid();
            }

            try
            {
                Console.WriteLine("GetSellerProducts: before _repository.GetAllAsync()");
                var products = await _repository.GetAllAsync();
                Console.WriteLine("GetSellerProducts: after _repository.GetAllAsync()");
                return Ok(products.Select(p => p.ToDto()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetSellerProducts: Exception: {ex}");
                return StatusCode(500, "Error loading products.");
            }
        }

    }
}

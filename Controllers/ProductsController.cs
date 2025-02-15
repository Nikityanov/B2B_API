using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2B_API.Models;
using B2B_API.Models.DTO;
using B2B_API.Interfaces;
using B2B_API.Services;

namespace B2B_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IGenericRepository<Product> _repository;

        public ProductsController(IGenericRepository<Product> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts(int? categoryId)
        {
            var query = _repository.GetAllAsync().Result.AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var products = await Task.FromResult(query.ToList());
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
        public async Task<ActionResult<ProductResponseDto>> CreateProduct(ProductCreateDto createDto)
        {
            var product = createDto.ToEntity();
            await _repository.AddAsync(product);
            await _repository.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProduct),
                new { id = product.Id },
                product.ToDto());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")]
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
    }
}
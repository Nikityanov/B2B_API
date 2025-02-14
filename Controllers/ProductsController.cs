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
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts()
        {
            var products = await _repository.GetAllAsync();
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
    }
} 
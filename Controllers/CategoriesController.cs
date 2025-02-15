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
    public class CategoriesController : ControllerBase
    {
        private readonly IGenericRepository<Category> _repository;
        private readonly CategoryService _categoryService; // Сервис категорий

        public CategoriesController(IGenericRepository<Category> repository, CategoryService categoryService)
        {
            _repository = repository;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetCategories([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Page and PageSize must be greater than 0.");
            }

            var (categories, totalCount) = await _repository.GetPagedAsync(page, pageSize);

            var response = new
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
                Categories = categories.Select(c => _categoryService.ToDto(c))
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponseDto>> GetCategory(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(_categoryService.ToDto(category)); // Используем сервис для маппинга
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Seller")] // Доступно админам и продавцам
        public async Task<ActionResult<CategoryResponseDto>> CreateCategory(CategoryCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var category = _categoryService.ToEntity(createDto); // Используем сервис для создания сущности
            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, _categoryService.ToDto(category)); // Используем сервис для маппинга
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Доступно только админам
        public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _categoryService.UpdateFromDto(category, updateDto); // Используем сервис для обновления сущности
            _repository.Update(category);
            await _repository.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Доступно только админам
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _repository.Remove(category);
            await _repository.SaveChangesAsync();
            return NoContent();
        }
    }
}
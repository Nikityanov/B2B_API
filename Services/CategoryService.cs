using B2B_API.Models;
using B2B_API.Models.DTO;

namespace B2B_API.Services
{
    public class CategoryService
    {
        public CategoryResponseDto ToDto(Category entity)
        {
            return new CategoryResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl
            };
        }

        public Category ToEntity(CategoryCreateDto dto)
        {
            return new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl
            };
        }

        public void UpdateFromDto(Category entity, CategoryUpdateDto dto)
        {
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.ImageUrl = dto.ImageUrl;
        }
    }
}
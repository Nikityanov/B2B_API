using System.ComponentModel.DataAnnotations;

namespace B2B_API.API.DTOs
{
    /// <summary>
    /// DTO для обновления информации о категории
    /// </summary>
    public class UpdateCategoryDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
    }
}
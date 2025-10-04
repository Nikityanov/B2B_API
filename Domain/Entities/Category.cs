using System.ComponentModel.DataAnnotations;

namespace B2B_API.Domain.Entities
{
    /// <summary>
    /// Доменная сущность категории продуктов
    /// </summary>
    public class Category : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        // Навигационные свойства
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

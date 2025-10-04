using System.ComponentModel.DataAnnotations;

namespace B2B_API.API.DTOs
{
    /// <summary>
    /// DTO для ответа API с информацией о продукте в прайс-листе
    /// </summary>
    public class PriceListProductDto
    {
        public int Id { get; set; }

        [Required]
        public int PriceListId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal SpecialPrice { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public ProductDto? Product { get; set; }
    }
}
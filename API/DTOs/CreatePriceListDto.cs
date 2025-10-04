using System.ComponentModel.DataAnnotations;

namespace B2B_API.API.DTOs
{
    /// <summary>
    /// DTO для создания нового прайс-листа
    /// </summary>
    public class CreatePriceListDto
    {
        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public required string Currency { get; set; }

        [Required]
        public int SellerId { get; set; }
    }
}
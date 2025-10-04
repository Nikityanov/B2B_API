using System.ComponentModel.DataAnnotations;

namespace B2B_API.API.DTOs
{
    /// <summary>
    /// DTO для добавления продукта в прайс-лист
    /// </summary>
    public class CreatePriceListProductDto
    {
        [Required]
        public int PriceListId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal SpecialPrice { get; set; }
    }
}
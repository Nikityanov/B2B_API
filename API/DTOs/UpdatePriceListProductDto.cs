using System.ComponentModel.DataAnnotations;

namespace B2B_API.API.DTOs
{
    /// <summary>
    /// DTO для обновления цены продукта в прайс-листе
    /// </summary>
    public class UpdatePriceListProductDto
    {
        [Range(0.01, double.MaxValue)]
        public decimal? SpecialPrice { get; set; }

        public bool? IsActive { get; set; }
    }
}
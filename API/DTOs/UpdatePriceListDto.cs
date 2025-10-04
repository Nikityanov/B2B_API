using System.ComponentModel.DataAnnotations;

namespace B2B_API.API.DTOs
{
    /// <summary>
    /// DTO для обновления прайс-листа
    /// </summary>
    public class UpdatePriceListDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public string? Currency { get; set; }

        public bool? IsActive { get; set; }
    }
}
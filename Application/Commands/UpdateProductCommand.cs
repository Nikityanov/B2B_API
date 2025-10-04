using FluentResults;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Commands
{
    /// <summary>
    /// Команда для обновления продукта
    /// </summary>
    public class UpdateProductCommand : ICommand
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public required string SKU { get; set; }
        public string? Manufacturer { get; set; }
        public string? Unit { get; set; }
        public string? ImageUrl { get; set; }
        public List<string>? ImageGallery { get; set; }
        public string? Characteristics { get; set; }
        public int? CategoryId { get; set; }
    }
}

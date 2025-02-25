using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2B_API.Models;
using B2B_API.Models.DTO;
using B2B_API.Interfaces;
using B2B_API.Services;
using B2B_API.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace B2B_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IGenericRepository<Product> _repository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IGenericRepository<Product> repository, 
            ApplicationDbContext context,
            ILogger<ProductsController> logger)
        {
            _repository = repository;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Получает список всех продуктов с возможностью фильтрации по категории
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts(int? categoryId)
        {
            try
            {
                IEnumerable<Product> products;
                
                if (categoryId.HasValue)
                {
                    // Оптимизация: используем прямой запрос с фильтрацией вместо загрузки всех продуктов
                    if (_context.Products != null) // Добавлена проверка на null, хотя _context.Products не должен быть null
                    {
                        products = await _context.Products
                            .Where(p => p.CategoryId == categoryId.Value)
                            .ToListAsync();
                    }
                    else
                    {
                        _logger.LogError("Ошибка: _context.Products is null"); // Логирование на случай, если _context.Products все же null
                        return StatusCode(500, "Ошибка при загрузке продуктов"); // Возвращаем ошибку, если _context.Products null
                    }
                }
                else
                {
                    products = await _repository.GetAllAsync();
                }

                return Ok(products.Select(p => p.ToDto()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка продуктов");
                return StatusCode(500, "Ошибка при загрузке продуктов");
            }
        }

        /// <summary>
        /// Получает продукт по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(int id)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении продукта с ID {ProductId}", id);
                return StatusCode(500, "Ошибка при загрузке продукта");
            }
        }

        /// <summary>
        /// Создает новый продукт
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Seller")]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<ProductResponseDto>> CreateProduct([FromForm] ProductCreateDto createDto)
        {
            try
            {
                var product = createDto.ToEntity();
                
                // ИСПРАВЛЕНО: Не игнорируем ImageUrl, если он предоставлен
                if (string.IsNullOrEmpty(createDto.ImageUrl))
                {
                    product.ImageUrl = null;
                }
                
                await _repository.AddAsync(product);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Создан новый продукт с ID {ProductId}", product.Id);

                return CreatedAtAction(
                    nameof(GetProduct),
                    new { id = product.Id },
                    product.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании продукта");
                return StatusCode(500, "Ошибка при сохранении продукта в базе данных");
            }
        }

        /// <summary>
        /// Обновляет существующий продукт
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDto updateDto)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                // Проверка, принадлежит ли продукт текущему продавцу
                if (!await IsProductOwnedByCurrentUser(product))
                {
                    return Forbid();
                }

                product.UpdateFromDto(updateDto);
                _repository.Update(product);
                var result = await _repository.SaveChangesAsync();

                if (!result)
                {
                    return StatusCode(500, "Не удалось обновить продукт");
                }

                _logger.LogInformation("Обновлен продукт с ID {ProductId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении продукта с ID {ProductId}", id);
                return StatusCode(500, "Ошибка при обновлении продукта");
            }
        }

        /// <summary>
        /// Удаляет продукт
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                // Проверка, принадлежит ли продукт текущему продавцу
                if (!await IsProductOwnedByCurrentUser(product))
                {
                    return Forbid();
                }

                _repository.Remove(product);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Удален продукт с ID {ProductId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении продукта с ID {ProductId}", id);
                return StatusCode(500, "Ошибка при удалении продукта");
            }
        }

        /// <summary>
        /// Получает список продуктов для покупателя из доступных прайс-листов
        /// </summary>
        [HttpGet("BuyerPriceList")]
        [Authorize(Roles = "Buyer")]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetBuyerPriceListProducts()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Forbid();
                }

                // Оптимизация: используем более эффективный запрос с проекцией
                if (_context.PriceLists == null)
                {
                    _logger.LogError("Ошибка: _context.PriceLists is null");
                    return StatusCode(500, "Ошибка при загрузке прайс-листов");
                }

                var products = await _context.PriceLists
                    .Where(pl => pl.AllowedBuyers.Any(b => b.Id == userId))
                    .SelectMany(pl => pl.Products)
                    .Select(pp => new ProductResponseDto
                    {
                        Id = pp.Product.Id,
                        Name = pp.Product.Name,
                        Description = pp.Product.Description ?? string.Empty,
                        StockQuantity = pp.Product.StockQuantity,
                        Price = pp.SpecialPrice, // Используем специальную цену из прайс-листа
                        SKU = pp.Product.SKU,
                        Manufacturer = pp.Product.Manufacturer,
                        Unit = pp.Product.Unit,
                        ImageUrl = pp.Product.ImageUrl,
                        ImageGallery = pp.Product.ImageGallery,
                        Characteristics = pp.Product.Characteristics,
                        CategoryId = pp.Product.CategoryId
                    })
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка продуктов для покупателя");
                return StatusCode(500, "Ошибка при загрузке продуктов");
            }
        }

        /// <summary>
        /// Добавляет изображение в галерею продукта
        /// </summary>
        [HttpPost("{id}/images")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> AddImageToProduct(int id, [FromBody] string imageUrl)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                // Проверка, принадлежит ли продукт текущему продавцу
                if (!await IsProductOwnedByCurrentUser(product))
                {
                    return Forbid();
                }

                if (string.IsNullOrEmpty(imageUrl))
                {
                    return BadRequest("Image URL is required");
                }

                if (product.ImageGallery == null)
                {
                    product.ImageGallery = new List<string>();
                }

                product.ImageGallery.Add(imageUrl);
                _repository.Update(product);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Добавлено изображение в галерею продукта с ID {ProductId}", id);
                return Ok("Image added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении изображения в галерею продукта с ID {ProductId}", id);
                return StatusCode(500, "Ошибка при добавлении изображения");
            }
        }

        /// <summary>
        /// Удаляет изображение из галереи продукта
        /// </summary>
        [HttpDelete("{id}/images")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> RemoveImageFromProduct(int id, [FromBody] string imageUrl)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                // Проверка, принадлежит ли продукт текущему продавцу
                if (!await IsProductOwnedByCurrentUser(product))
                {
                    return Forbid();
                }

                if (string.IsNullOrEmpty(imageUrl))
                {
                    return BadRequest("Image URL is required");
                }

                if (product.ImageGallery == null || !product.ImageGallery.Contains(imageUrl))
                {
                    return NotFound("Image URL not found in product gallery");
                }

                product.ImageGallery.Remove(imageUrl);
                _repository.Update(product);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Удалено изображение из галереи продукта с ID {ProductId}", id);
                return Ok("Image removed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении изображения из галереи продукта с ID {ProductId}", id);
                return StatusCode(500, "Ошибка при удалении изображения");
            }
        }

        /// <summary>
        /// Обновляет основное изображение продукта
        /// </summary>
        [HttpPut("{id}/image")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdateProductImage(int id, [FromBody] string imageUrl)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                // Проверка, принадлежит ли продукт текущему продавцу
                if (!await IsProductOwnedByCurrentUser(product))
                {
                    return Forbid();
                }

                if (string.IsNullOrEmpty(imageUrl))
                {
                    return BadRequest("Image URL is required");
                }

                product.ImageUrl = imageUrl;
                _repository.Update(product);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Обновлено основное изображение продукта с ID {ProductId}", id);
                return Ok("Product image updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении основного изображения продукта с ID {ProductId}", id);
                return StatusCode(500, "Ошибка при обновлении изображения");
            }
        }

        /// <summary>
        /// Обновляет характеристики продукта
        /// </summary>
        [HttpPut("{id}/characteristics")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdateProductCharacteristics(int id, [FromBody] string characteristics)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                // Проверка, принадлежит ли продукт текущему продавцу
                if (!await IsProductOwnedByCurrentUser(product))
                {
                    return Forbid();
                }

                if (string.IsNullOrEmpty(characteristics))
                {
                    return BadRequest("Product characteristics are required");
                }

                product.Characteristics = characteristics;
                _repository.Update(product);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Обновлены характеристики продукта с ID {ProductId}", id);
                return Ok("Product characteristics updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении характеристик продукта с ID {ProductId}", id);
                return StatusCode(500, "Ошибка при обновлении характеристик");
            }
        }

        /// <summary>
        /// Получает список продуктов текущего продавца
        /// </summary>
        [HttpGet("SellerProducts")]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetSellerProducts()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Forbid();
                }

                // Поскольку в модели Product нет поля SellerId, получаем все продукты
                // В будущем нужно добавить фильтрацию по продавцу
                var products = await _repository.GetAllAsync();

                return Ok(products.Select(p => p.ToDto()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка продуктов продавца");
                return StatusCode(500, "Ошибка при загрузке продуктов");
            }
        }

        /// <summary>
        /// Проверяет, принадлежит ли продукт текущему пользователю
        /// </summary>
        private static async Task<bool> IsProductOwnedByCurrentUser(Product product)
        {
            // Поскольку в модели Product нет поля SellerId,
            // мы не можем проверить принадлежность продукта пользователю
            // В реальном приложении здесь должна быть логика проверки владельца продукта
            
            // Временное решение: разрешаем доступ всем продавцам
            // В будущем нужно добавить поле SellerId в модель Product
            // или реализовать другой механизм проверки владельца
            return await Task.FromResult(true);
        }
    }
}

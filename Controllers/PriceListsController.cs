using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using B2B_API.Models;
using B2B_API.Models.DTO;
using B2B_API.Interfaces;
using B2B_API.Services;
using B2B_API.Data;
using System.Security.Claims;

namespace B2B_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PriceListsController : ControllerBase
    {
        private readonly IGenericRepository<PriceList> _repository;
        private readonly ApplicationDbContext _context;

        public PriceListsController(IGenericRepository<PriceList> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PriceListResponseDto>>> GetPriceLists()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userRoleClaim = User.FindFirst(ClaimTypes.Role);

            // int userId = 0; // Duplicate declaration removed
            string? userRole = userRoleClaim?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Forbid(); // Или BadRequest("Invalid user ID");
            }

            var priceListsDbSet = _context.PriceLists;
            if (priceListsDbSet == null)
            {
                return Ok(new List<PriceListResponseDto>()); // Return empty list if PriceLists is null
            }
            var query = priceListsDbSet
                .Include(p => p.Seller)
                .Include(p => p.AllowedBuyers) // Возможно, здесь была проблема с null
                .Include(p => p.Products)
                    .ThenInclude(pp => pp.Product)
                .AsQueryable();

            if (userRole == "Buyer")
            {
                query = query.Where(p => p.AllowedBuyers.Any(b => b.Id == userId));
            }
            else if (userRole == "Seller")
            {
                query = query.Where(p => p.SellerId == userId);
            }

            var priceLists = await query.ToListAsync();
            return Ok(priceLists.Select(p => p.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PriceListResponseDto>> GetPriceList(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userRoleClaim = User.FindFirst(ClaimTypes.Role);

            // int userId = 0; // Duplicate declaration removed
            string? userRole = userRoleClaim?.Value;


            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Forbid(); // Или BadRequest("Invalid user ID");
            }

            if (_context.PriceLists == null)
            {
                return NotFound("PriceLists DbSet is null"); // Handle null PriceLists DbSet
            }
            var priceList = await _context.PriceLists
                .Include(p => p.Seller)
                .Include(p => p.AllowedBuyers)
                .Include(p => p.Products)
                    .ThenInclude(pp => pp.Product)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (priceList == null)
            {
                return NotFound();
            }

            if (userRole == "Buyer" && !priceList.AllowedBuyers.Any(b => b.Id == userId))
            {
                return Forbid();
            }
            else if (userRole == "Seller" && priceList.SellerId != userId)
            {
                return Forbid();
            }

            return Ok(priceList.ToDto());
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult<PriceListResponseDto>> CreatePriceList(PriceListCreateDto createDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Forbid(); // Или BadRequest("Invalid user ID");
            }

            if (_context.Users == null)
            {
                return Problem("Users DbSet is null", statusCode: 500);
            }
            var seller = await _context.Users.FindAsync(userId);
            if (seller == null)
            {
                return NotFound("Seller not found"); // Продавец не найден
            }

            // Corrected ToEntity call with positional arguments:
            var priceList = createDto.ToEntity(userId, seller);
            // priceList.SellerId = userId; // redundant, but harmless

            await _repository.AddAsync(priceList);
            await _repository.SaveChangesAsync();

            // Загружаем связанные данные для ответа
            if (_context.PriceLists == null)
            {
                return Problem("PriceLists DbSet is null", statusCode: 500);
            }
            priceList = await _context.PriceLists
                .Include(p => p.Seller)
                .Include(p => p.AllowedBuyers)
                .Include(p => p.Products)
                .ThenInclude(pp => pp.Product)
                .FirstOrDefaultAsync(p => p.Id == priceList.Id);

            if (priceList == null)
            {
                return Problem("Price list creation failed", statusCode: 500); // Или BadRequest, в зависимости от логики
            }

            return CreatedAtAction(
                nameof(GetPriceList),
                new { id = priceList.Id },
                priceList.ToDto());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdatePriceList(int id, PriceListUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Forbid(); // Или BadRequest("Invalid user ID");
            }
            var priceList = await _repository.GetByIdAsync(id);

            if (priceList == null)
            {
                return NotFound();
            }

            if (priceList.SellerId != userId)
            {
                return Forbid();
            }

            priceList.UpdateFromDto(updateDto); // Update existing fields

            _repository.Update(priceList);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeletePriceList(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Forbid(); // Или BadRequest("Invalid user ID");
            }
            var priceList = await _repository.GetByIdAsync(id);

            if (priceList == null)
            {
                return NotFound();
            }

            if (priceList.SellerId != userId)
            {
                return Forbid();
            }

            _repository.Remove(priceList);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/products")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> AddProductToPriceList(int id, PriceListProductCreateDto createDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Forbid(); // Или BadRequest("Invalid user ID");
            }
        
            if (_context.PriceListProducts == null)
            {
                return Problem("PriceListProducts DbSet is null", statusCode: 500);
            }
            var priceList = await _repository.GetByIdAsync(id);
            if (priceList == null || priceList.SellerId != userId)
            {
                return Forbid();
            }

            if (_context.Products == null)
            {
                return Problem("Products DbSet is null", statusCode: 500);
            }
            var product = await _context.Products.FindAsync(createDto.ProductId);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            var priceListProduct = new PriceListProduct
            {
                PriceListId = id,
                ProductId = createDto.ProductId,
                SpecialPrice = createDto.SpecialPrice,
                PriceList = priceList, // Assign PriceList entity
                Product = product      // Assign Product entity
            };

            if (_context.PriceListProducts == null)
            {
                return Problem("PriceListProducts DbSet is null", statusCode: 500);
            }
            await _context.PriceListProducts.AddAsync(priceListProduct);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}/products/{productId}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> RemoveProductFromPriceList(int id, int productId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Forbid(); // Или BadRequest("Invalid user ID");
            }
            var priceList = await _repository.GetByIdAsync(id);
            
            if (priceList == null || priceList.SellerId != userId)
            {
                return Forbid();
            }

            if (_context.PriceListProducts == null)
            {
                return Problem("PriceListProducts DbSet is null", statusCode: 500);
            }
            
                        if (_context.PriceListProducts == null)
                        {
                            return Problem("PriceListProducts DbSet is null", statusCode: 500);
                        }
                        var priceListProduct = await _context.PriceListProducts
                            .FirstOrDefaultAsync(p => p.PriceListId == id && p.ProductId == productId);
            if (priceListProduct == null)
            {
                return NotFound();
            }

            _context.PriceListProducts.Remove(priceListProduct);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/buyers/{buyerId}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> AddBuyerToPriceList(int id, int buyerId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Forbid(); // Или BadRequest("Invalid user ID");
            }
            if (_context.PriceLists == null)
            {
                return Problem("PriceLists DbSet is null", statusCode: 500);
            }
            var priceList = await _context.PriceLists
                .Include(p => p.AllowedBuyers)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (priceList == null)
            {
                return NotFound("PriceList not found");
            }

            if (priceList.SellerId != userId)
            {
                return Forbid();
            }

            if (_context.Users == null)
            {
                return Problem("Users DbSet is null", statusCode: 500);
            }
            var buyer = await _context.Users.FindAsync(buyerId);
            if (buyer == null)
            {
                return NotFound("Buyer not found");
            }

            priceList.AllowedBuyers.Add(buyer);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}/buyers/{buyerId}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> RemoveBuyerFromPriceList(int id, int buyerId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Forbid(); // Или BadRequest("Invalid user ID");
            }
            if (_context.PriceLists == null)
            {
                return Problem("PriceLists DbSet is null", statusCode: 500);
            }
            var priceList = await _context.PriceLists
                .Include(p => p.AllowedBuyers)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (priceList == null)
            {
                return NotFound("PriceList not found");
            }

            if (priceList.SellerId != userId)
            {
                return Forbid();
            }

            var buyer = priceList.AllowedBuyers.FirstOrDefault(b => b.Id == buyerId);
            if (buyer == null)
            {
                return NotFound("Buyer not found in this price list");
            }

            priceList.AllowedBuyers.Remove(buyer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
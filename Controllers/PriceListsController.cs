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
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var query = _context.PriceLists
                .Include(p => p.Seller)
                .Include(p => p.AllowedBuyers)
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
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

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
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var priceList = createDto.ToEntity(userId);
            priceList.SellerId = userId;

            await _repository.AddAsync(priceList);
            await _repository.SaveChangesAsync();

            // Загружаем связанные данные для ответа
            priceList = await _context.PriceLists
                .Include(p => p.Seller)
                .Include(p => p.AllowedBuyers)
                .Include(p => p.Products)
                    .ThenInclude(pp => pp.Product)
                .FirstOrDefaultAsync(p => p.Id == priceList.Id);

            return CreatedAtAction(
                nameof(GetPriceList),
                new { id = priceList.Id },
                priceList.ToDto());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdatePriceList(int id, PriceListUpdateDto updateDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var priceList = await _repository.GetByIdAsync(id);

            if (priceList == null)
            {
                return NotFound();
            }

            if (priceList.SellerId != userId)
            {
                return Forbid();
            }

            priceList.UpdateFromDto(updateDto);
            _repository.Update(priceList);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeletePriceList(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
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
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var priceList = await _repository.GetByIdAsync(id);
            
            if (priceList == null || priceList.SellerId != userId)
            {
                return Forbid();
            }

            var priceListProduct = new PriceListProduct
            {
                PriceListId = id,
                ProductId = createDto.ProductId,
                SpecialPrice = createDto.SpecialPrice
            };

            await _context.PriceListProducts.AddAsync(priceListProduct);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}/products/{productId}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> RemoveProductFromPriceList(int id, int productId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var priceList = await _repository.GetByIdAsync(id);
            
            if (priceList == null || priceList.SellerId != userId)
            {
                return Forbid();
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
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
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
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using B2B_API.Models;
using B2B_API.Models.DTO;
using B2B_API.Data;
using B2B_API.Services;
using System.Security.Claims;

namespace B2B_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<UserProfileResponseDto>> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return user.ToProfileDto();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UserProfileUpdateDto updateDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.UpdateFromDto(updateDto);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExists(userId))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        private async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(e => e.Id == id);
        }
    }
} 
using Microsoft.AspNetCore.Mvc;
using B2B_API.Models;
using B2B_API.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace B2B_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IGenericRepository<User> _repository;

        public UsersController(IGenericRepository<User> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _repository.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            await _repository.AddAsync(user);
            await _repository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _repository.Update(user);
            var result = await _repository.SaveChangesAsync();

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _repository.Remove(user);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
} 
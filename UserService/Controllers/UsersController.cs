using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using UserService.DTOs;
using UserService.Models;
using UserService.Services.Interface;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController: ControllerBase
    {
        private readonly IUserService _service;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService service, ILogger<UsersController> logger)
        {
            _service = service;
            _logger = logger;
        }
        /// <summary>
        /// get all users 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            var dto = list.Select(u => new UserProfileDto(u.Id, u.AuthUserId, u.Username, u.Email, u.CreatedAt));
            return Ok(dto);
        }

        /// <summary>
        /// get user by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _service.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(new UserProfileDto(user.Id, user.AuthUserId, user.Username, user.Email, user.CreatedAt));
        }
        /// <summary>
        /// get user by auth Id
        /// </summary>
        /// <param name="authUserId"></param>
        /// <returns></returns>
        [HttpGet("by-auth/{authUserId}")]
        public async Task<IActionResult> GetByAuth(string authUserId)
        {
            var user = await _service.GetByAuthUserIdAsync(authUserId);
            if (user == null) return NotFound();
            return Ok(new UserProfileDto(user.Id, user.AuthUserId, user.Username, user.Email, user.CreatedAt));
        }

      /// <summary>
      /// create new user
      /// </summary>
      /// <param name="dto"></param>
      /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            var profile = new UserProfile
            {
                AuthUserId = dto.AuthUserId,
                Username = dto.Username,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow
            };

            await _service.CreateIfNotExistsAsync(profile);
            _logger.LogInformation("Manual created user profile for {Email}", dto.Email);
            return CreatedAtAction(nameof(GetByAuth), new { authUserId = dto.AuthUserId }, new { dto.AuthUserId });
        }

        /// <summary>
        /// update user data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto dto)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Username = dto.Username;
            existing.Email = dto.Email;

            await _service.UpdateAsync(id, existing);
            _logger.LogInformation("Updated user {Id}", id);

            return Ok("user updated sucessfully");
        }
        /// <summary>
        /// delete user 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _service.DeleteAsync(id);
            _logger.LogInformation(" Deleted user {Id}", id);

            return Ok("user deleted sucessfully");
        }
    }

}


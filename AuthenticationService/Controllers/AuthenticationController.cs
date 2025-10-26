using AuthenticationService.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
namespace AuthenticationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController:ControllerBase
    {
        private readonly AuthService _authService;

        public AuthenticationController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var token = await _authService.RegisterAsync(request.Username, request.Email, request.Password);
            return Ok(new { Token = token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var token = await _authService.LoginAsync(request.Email, request.Password);
            return Ok(new { Token = token });
        }

        public record RegisterRequest(string Username, string Email, string Password);
        public record LoginRequest(string Email, string Password);

    }
}

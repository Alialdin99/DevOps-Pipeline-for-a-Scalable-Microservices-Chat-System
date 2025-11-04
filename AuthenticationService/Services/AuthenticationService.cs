using AuthenticationService.Models;
using AuthenticationService.Repositories;
using AuthenticationService.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Configuration;

using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace AuthenticationService.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        //private readonly string _jwtKey;
        private readonly IConfiguration _config;

        public AuthService(UserRepository userRepository, IPublishEndpoint publishEndpoint, IConfiguration config)
        {
            _userRepository = userRepository;
            _publishEndpoint = publishEndpoint;
           _config = config;
        }

        public async Task<string> RegisterAsync(string username,string email, string password)
        {
            var existing = await _userRepository.GetByEmailAsync(email);
            if (existing != null)
                throw new Exception("Userl Already exists");

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password)
            };

            await _userRepository.CreateAsync(user);

            // Publish "UserRegistered" event to RabbitMQ
            await _publishEndpoint.Publish(new UserRegisteredEvent
            {
                UserId = user.Id,
                Email = user.Email,
                Username = user.Username,
                CreatedAt = user.CreatedAt
            });
            return GenerateJwtToken(user);
        }
        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || user.PasswordHash != HashPassword(password))
                throw new Exception("Invalid credentials");

            return GenerateJwtToken(user);
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _config["Jwt:Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: new[]
                {
                    new Claim("id", user.Id),
                    new Claim("email", user.Email),
                    new Claim("username", user.Username)
                },
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}

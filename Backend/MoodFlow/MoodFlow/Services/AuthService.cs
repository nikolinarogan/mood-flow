using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MoodFlow.Data;
using MoodFlow.DTOs;
using MoodFlow.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MoodFlow.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<bool> VerifyEmailAsync(string token);
        string GenerateJwtToken(User user);
        string GenerateEmailVerificationToken();
        Task<bool> ChangeUsernameAsync(int userId, string newUsername);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
    
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, IEmailService emailService, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                throw new Exception("Email already exists");

            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
                throw new Exception("Username already exists");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            string verificationToken = GenerateEmailVerificationToken();

            var user = new User
            {
                Email = registerDto.Email,
                Username = registerDto.Username,
                Password = passwordHash,
                CreatedDate = DateTime.UtcNow,
                IsEmailVerified = false,
                EmailVerificationToken = verificationToken,
                EmailVerificationTokenExp = DateTime.UtcNow.AddHours(24)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _emailService.SendVerificationEmail(user.Email, user.Username, verificationToken);

            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user),
                Username = user.Username,
                Email = user.Email
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                throw new Exception("Invalid email or password");

            if (!user.IsEmailVerified)
                throw new Exception("Please verify your email before logging in");

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user),
                Username = user.Username,
                Email = user.Email
            };
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == token);

            if (user == null)
                return false;

            if (user.EmailVerificationTokenExp < DateTime.UtcNow)
                return false;

            user.IsEmailVerified = true;
            user.EmailVerificationToken = string.Empty;
            user.EmailVerificationTokenExp = null;

            await _context.SaveChangesAsync();
            return true;
        }

        public string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateEmailVerificationToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "_")
                .Replace("+", "-")
                .Substring(0, 22);
        }

        public async Task<bool> ChangeUsernameAsync(int userId, string newUsername)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            if (string.IsNullOrEmpty(newUsername))
                throw new Exception("New username is required");

            if (await _context.Users.AnyAsync(u => u.Username == newUsername && u.Id != userId))
                throw new Exception("Username already exists");

            user.Username = newUsername;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            if (string.IsNullOrEmpty(currentPassword))
                throw new Exception("Current password is required");

            if (string.IsNullOrEmpty(newPassword))
                throw new Exception("New password is required");

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
                throw new Exception("Invalid current password");

            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.Password = newPasswordHash;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}

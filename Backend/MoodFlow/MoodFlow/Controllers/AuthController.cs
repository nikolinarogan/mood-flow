using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MoodFlow.DTOs;
using MoodFlow.Services;
using System.Security.Claims;
using System.Text.Json;
using MoodFlow.Data;

namespace MoodFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;

        public AuthController(IAuthService authService, ApplicationDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                return Ok(new
                {
                    message = "Registration successful!",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("verify-email")]
        public async Task<ActionResult> VerifyEmail([FromQuery] string token)
        {
            try
            {
                var isVerified = await _authService.VerifyEmailAsync(token);

                if (isVerified)
                {
                    return Ok(new { message = "Email verified successfully! You can now log in." });
                }
                else
                {
                    return BadRequest(new { message = "Invalid or expired verification token." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("change-username")]
        [Authorize]
        public async Task<ActionResult> ChangeUsername([FromBody] AuthChangeUsernameDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                if (string.IsNullOrEmpty(dto.NewUsername))
                {
                    return BadRequest(new { message = "New username is required" });
                }
                
                await _authService.ChangeUsernameAsync(userId, dto.NewUsername);
                return Ok(new { message = "Username updated successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] AuthChangePasswordDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                if (string.IsNullOrEmpty(dto.CurrentPassword))
                {
                    return BadRequest(new { message = "Current password is required" });
                }

                if (string.IsNullOrEmpty(dto.NewPassword))
                {
                    return BadRequest(new { message = "New password is required" });
                }

                await _authService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);
                return Ok(new { message = "Password updated successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("notification-time")]
        public async Task<IActionResult> GetNotificationTime()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();
            return Ok(new NotificationTimeDto { NotificationTime = user.NotificationTime ?? new TimeSpan(9, 0, 0) });
        }

        [Authorize]
        [HttpPost("notification-time")]
        public async Task<IActionResult> SetNotificationTime([FromBody] NotificationTimeDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();
            user.NotificationTime = dto.NotificationTime;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}

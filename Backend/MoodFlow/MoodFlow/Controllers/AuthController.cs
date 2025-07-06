using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MoodFlow.DTOs;
using MoodFlow.Services;
using System.Security.Claims;
using System.Text.Json;

namespace MoodFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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
        public async Task<ActionResult> ChangeUsername([FromBody] JsonElement request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                string newUsername = request.GetProperty("newUsername").GetString() ?? "";
                if (string.IsNullOrEmpty(newUsername))
                {
                    return BadRequest(new { message = "New username is required" });
                }
                
                await _authService.ChangeUsernameAsync(userId, newUsername);
                return Ok(new { message = "Username updated successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] JsonElement request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                string currentPassword = request.GetProperty("currentPassword").GetString() ?? "";
                string newPassword = request.GetProperty("newPassword").GetString() ?? "";

                if (string.IsNullOrEmpty(currentPassword))
                {
                    return BadRequest(new { message = "Current password is required" });
                }

                if (string.IsNullOrEmpty(newPassword))
                {
                    return BadRequest(new { message = "New password is required" });
                }

                await _authService.ChangePasswordAsync(userId, currentPassword, newPassword);
                return Ok(new { message = "Password updated successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

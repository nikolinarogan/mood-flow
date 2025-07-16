using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MoodFlow.Services;
using System.Security.Claims;

namespace MoodFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeditationController : ControllerBase
    {
        private readonly IMeditationService _meditationService;
        private readonly ILogger<MeditationController> _logger;

        public MeditationController(IMeditationService meditationService, ILogger<MeditationController> logger)
        {
            _meditationService = meditationService;
            _logger = logger;
        }

        [HttpGet("exercises")]
        public async Task<ActionResult<IEnumerable<object>>> GetExercises()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int? userId = null;
                
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int parsedUserId))
                {
                    userId = parsedUserId;
                }

                var exercises = await _meditationService.GetExercisesAsync(userId);
                return Ok(exercises);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching meditation exercises");
                return StatusCode(500, new { message = "An error occurred while fetching exercises" });
            }
        }

        [HttpPost("favourite/{exerciseId}")]
        [Authorize]
        public async Task<ActionResult> ToggleFavourite(int exerciseId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var isFavourite = await _meditationService.ToggleFavouriteAsync(userId, exerciseId);
                
                return Ok(new { 
                    isFavourite = isFavourite, 
                    message = isFavourite ? "Added to favourites" : "Removed from favourites" 
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling meditation exercise favourite");
                return StatusCode(500, new { message = "An error occurred while toggling favourite" });
            }
        }

        [HttpGet("favourites")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetFavourites()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var favouriteExercises = await _meditationService.GetFavouritesAsync(userId);
                return Ok(favouriteExercises);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching favourite meditation exercises");
                return StatusCode(500, new { message = "An error occurred while fetching favourites" });
            }
        }
    }
}

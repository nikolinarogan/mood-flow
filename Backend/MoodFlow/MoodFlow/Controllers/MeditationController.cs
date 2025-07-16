using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoodFlow.Data;
using MoodFlow.Models;

namespace MoodFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeditationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MeditationController> _logger;

        public MeditationController(ApplicationDbContext context, ILogger<MeditationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("exercises")]
        public async Task<ActionResult<IEnumerable<MeditationExercise>>> GetExercises()
        {
            try
            {
                var exercises = await _context.MeditationExercises.ToListAsync();
                return Ok(exercises);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching meditation exercises");
                return StatusCode(500, new { message = "An error occurred while fetching exercises" });
            }
        }
    }
}

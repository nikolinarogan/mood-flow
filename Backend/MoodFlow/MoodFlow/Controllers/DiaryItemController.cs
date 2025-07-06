using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MoodFlow.Services;
using MoodFlow.DTOs;
using System.Security.Claims;

namespace MoodFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DiaryItemController : ControllerBase
    {
        private readonly IDiaryItemService _diaryService;
        private readonly ILogger<DiaryItemController> _logger;

        public DiaryItemController(IDiaryItemService diaryService, ILogger<DiaryItemController> logger)
        {
            _diaryService = diaryService;
            _logger = logger;
        }

        [HttpPost("create")]
        public ActionResult Create([FromBody] CreateDiaryItemRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var diaryItem = _diaryService.Create(userId, request.Emotion, request.Grade, request.Note);

                return Ok(new
                {
                    message = "Diary entry created successfully!",
                    data = new DiaryItemResponse
                    {
                        Id = diaryItem.Id,
                        Emotion = diaryItem.Emotion,
                        Grade = diaryItem.Grade,
                        Note = diaryItem.Note,
                        CreatedAt = diaryItem.CreatedAt
                    }
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating diary entry");
                return StatusCode(500, new { message = "An error occurred while creating the diary entry" });
            }
        }

        [HttpGet("today")]
        public ActionResult GetTodayEntry()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var today = DateTime.UtcNow.Date;
                var entry = _diaryService.GetByDate(userId, today);

                if (entry == null)
                {
                    return NotFound(new { message = "No entry found for today" });
                }

                return Ok(new
                {
                    data = new DiaryItemResponse
                    {
                        Id = entry.Id,
                        Emotion = entry.Emotion,
                        Grade = entry.Grade,
                        Note = entry.Note,
                        CreatedAt = entry.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting today's diary entry");
                return StatusCode(500, new { message = "An error occurred while getting the diary entry" });
            }
        }

        [HttpGet("date/{date}")]
        public ActionResult GetEntryByDate(string date)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                if (!DateTime.TryParse(date, out DateTime targetDate))
                {
                    return BadRequest(new { message = "Invalid date format. Use YYYY-MM-DD" });
                }

                var entry = _diaryService.GetByDate(userId, targetDate);

                if (entry == null)
                {
                    return NotFound(new { message = $"No entry found for {date}" });
                }

                return Ok(new
                {
                    data = new DiaryItemResponse
                    {
                        Id = entry.Id,
                        Emotion = entry.Emotion,
                        Grade = entry.Grade,
                        Note = entry.Note,
                        CreatedAt = entry.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting diary entry by date");
                return StatusCode(500, new { message = "An error occurred while getting the diary entry" });
            }
        }

        [HttpGet("all")]
        public ActionResult GetAllEntries()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var entries = _diaryService.GetByUserId(userId);

                return Ok(new
                {
                    data = entries.Select(entry => new DiaryItemResponse
                    {
                        Id = entry.Id,
                        Emotion = entry.Emotion,
                        Grade = entry.Grade,
                        Note = entry.Note,
                        CreatedAt = entry.CreatedAt
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all diary entries");
                return StatusCode(500, new { message = "An error occurred while getting diary entries" });
            }
        }

        [HttpPut("update/{id}")]
        public ActionResult Update(int id, [FromBody] UpdateDiaryItemRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var success = _diaryService.Update(id, request.Emotion, request.Grade, request.Note);

                if (!success)
                {
                    return NotFound(new { message = "Diary entry not found" });
                }

                return Ok(new { message = "Diary entry updated successfully!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating diary entry");
                return StatusCode(500, new { message = "An error occurred while updating the diary entry" });
            }
        }

        [HttpDelete("delete/{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var success = _diaryService.Delete(id);

                if (!success)
                {
                    return NotFound(new { message = "Diary entry not found" });
                }

                return Ok(new { message = "Diary entry deleted successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting diary entry");
                return StatusCode(500, new { message = "An error occurred while deleting the diary entry" });
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoodFlow.Services;
using System.Security.Claims;

namespace MoodFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuoteController : ControllerBase
    {
        private readonly QuoteService _quoteService;

        public QuoteController(QuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        [HttpGet("today")]
        public IActionResult GetTodayQuote()
        {
            var quote = _quoteService.GetOrFetchTodayQuote();
            return Ok(quote);
        }

        [HttpGet("today/refresh")]
        public IActionResult RefreshTodayQuote()
        {
            var quote = _quoteService.RefreshTodayQuote();
            return Ok(quote);
        }

        [HttpGet("debug")]
        public IActionResult DebugQuotes()
        {
            var today = DateTime.UtcNow.Date;
            var quotes = _quoteService.GetAllQuotes();
            var todayQuote = quotes.FirstOrDefault(q => q.Date == today);
            
            return Ok(new
            {
                currentTime = DateTime.UtcNow,
                today = today,
                totalQuotes = quotes.Count,
                todayQuote = todayQuote,
                allQuotes = quotes.Take(5).Select(q => new { q.Id, q.QuoteText, q.Date })
            });
        }

        [HttpGet("all")]
        public IActionResult GetAllQuotes()
        {
            var quotes = _quoteService.GetAllQuotes();
            return Ok(quotes);
        }
        [HttpPost("favourite/{id}")]
        public IActionResult ToggleFavourite(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            _quoteService.ToggleFavourite(id, userId);
            var isFavourited = _quoteService.IsQuoteFavouritedByUser(id, userId);
            return Ok(new { isFavourite = isFavourited });
        }

        [HttpGet("favourites")]
        public IActionResult GetUserFavourites()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            var favouriteIds = _quoteService.GetUserFavouriteQuoteIds(userId);
            var favouriteQuotes = _quoteService.GetQuotesByIds(favouriteIds);
            return Ok(favouriteQuotes);
        }
    }
}

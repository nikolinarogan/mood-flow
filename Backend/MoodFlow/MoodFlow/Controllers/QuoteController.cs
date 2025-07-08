using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoodFlow.Services;

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

        [HttpGet("all")]
        public IActionResult GetAllQuotes()
        {
            var quotes = _quoteService.GetAllQuotes();
            return Ok(quotes);
        }
    }
}

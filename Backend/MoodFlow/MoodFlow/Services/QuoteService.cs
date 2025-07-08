using MoodFlow.Data;
using MoodFlow.Models;
using System.Text.Json;

namespace MoodFlow.Services
{

    public class QuoteService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public QuoteService(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }
        public Quote GetOrFetchTodayQuote()
        {
            var today = DateTime.UtcNow.Date;
            var existing = _context.Quotes.FirstOrDefault(q => q.Date == today);
            if (existing != null)
                return existing;

            
            var client = _httpClientFactory.CreateClient();
            var response = client.GetStringAsync("https://zenquotes.io/api/today").Result;
            var json = JsonDocument.Parse(response);
            var quoteText = json.RootElement[0].GetProperty("q").GetString();
            var author = json.RootElement[0].GetProperty("a").GetString();

            var quote = new Quote
            {
                QuoteText= quoteText,
                Author = author,
                Date = today
            };
            _context.Quotes.Add(quote);
            _context.SaveChanges();
            return quote;
        }
        public List<Quote> GetAllQuotes()
        {
            return _context.Quotes.OrderByDescending(q => q.Date).ToList();
        }
    }
}

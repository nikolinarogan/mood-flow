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
                Date = today,
                IsFavourite = false
            };
            _context.Quotes.Add(quote);
            _context.SaveChanges();
            return quote;
        }

        public Quote RefreshTodayQuote()
        {
            var today = DateTime.UtcNow.Date;
            
            var existing = _context.Quotes.FirstOrDefault(q => q.Date == today);
            if (existing != null)
            {
                _context.Quotes.Remove(existing);
                _context.SaveChanges();
            }

            var client = _httpClientFactory.CreateClient();
            var response = client.GetStringAsync("https://zenquotes.io/api/today").Result;
            var json = JsonDocument.Parse(response);
            var quoteText = json.RootElement[0].GetProperty("q").GetString();
            var author = json.RootElement[0].GetProperty("a").GetString();

            var quote = new Quote
            {
                QuoteText = quoteText,
                Author = author,
                Date = today,
                IsFavourite = false
            };
            _context.Quotes.Add(quote);
            _context.SaveChanges();
            return quote;
        }
        public List<Quote> GetAllQuotes()
        {
            return _context.Quotes.OrderByDescending(q => q.Date).ToList();
        }

        public Quote? GetQuoteById(int id)
        {
            return _context.Quotes.FirstOrDefault(q => q.Id == id);
        }
        public void UpdateQuote(Quote quote)
        {
            _context.Quotes.Update(quote);
            _context.SaveChanges();
        }
        public bool IsQuoteFavouritedByUser(int quoteId, int userId)
        {
            return _context.UserQuotes
                .Any(ufq => ufq.QuoteId == quoteId && ufq.UserId == userId);
        }

        public void ToggleFavourite(int quoteId, int userId)
        {
            var existing = _context.UserQuotes
                .FirstOrDefault(ufq => ufq.QuoteId == quoteId && ufq.UserId == userId);

            if (existing != null)
            {
                _context.UserQuotes.Remove(existing);
            }
            else
            {
                _context.UserQuotes.Add(new UserQuote
                {
                    QuoteId = quoteId,
                    UserId = userId
                });
            }
            _context.SaveChanges();
        }

        public List<int> GetUserFavouriteQuoteIds(int userId)
        {
            return _context.UserQuotes
                .Where(ufq => ufq.UserId == userId)
                .Select(ufq => ufq.QuoteId)
                .ToList();
        }

        public List<Quote> GetQuotesByIds(List<int> ids)
        {
            return _context.Quotes
                .Where(q => ids.Contains(q.Id))
                .ToList();
        }
    }
}

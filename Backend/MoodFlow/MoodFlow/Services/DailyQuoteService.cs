using MoodFlow.Data;
using MoodFlow.Models;
using System.Text.Json;

namespace MoodFlow.Services
{
    public class DailyQuoteService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DailyQuoteService> _logger;

        public DailyQuoteService(IServiceProvider serviceProvider, ILogger<DailyQuoteService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Run immediately on startup to ensure we have today's quote
            _logger.LogInformation("DailyQuoteService starting - fetching initial quote");
            await FetchDailyQuote();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Run every 5 minutes for testing
                    var delay = TimeSpan.FromMinutes(5);
                    _logger.LogInformation($"Next quote fetch scheduled in 5 minutes");
                    
                    await Task.Delay(delay, stoppingToken);
                    
                    await FetchDailyQuote();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in daily quote service");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Wait 5 minutes before retrying
                }
            }
        }

        private async Task FetchDailyQuote()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

            try
            {
                var today = DateTime.UtcNow.Date;

                // Fetch new quote from API
                var client = httpClientFactory.CreateClient();
                var response = await client.GetStringAsync("https://zenquotes.io/api/random");
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

                context.Quotes.Add(quote);
                await context.SaveChangesAsync();
                
                _logger.LogInformation($"Successfully fetched new quote: {quoteText}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch daily quote");
            }
        }
    }
} 
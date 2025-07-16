using MoodFlow.Data;
using MoodFlow.Models;

namespace MoodFlow.Services
{
    public interface IDiaryItemService
    {
        DiaryItem Create(int userId, string emotion, int grade, string? note = null);
        Task<DiaryItem> CreateWithSentimentAnalysis(int userId, int grade, string? note = null);
        DiaryItem? GetByDate(int userId, DateTime date);
        List<DiaryItem> GetByUserId(int userId);
        bool Update(int id, string emotion, int grade, int userId, string? note = null);
        bool Delete(int id, int userId);
        Task<SentimentResponse> AnalyzeNoteSentimentAsync(string note);
    }
    
    public class DiaryItemService : IDiaryItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DiaryItemService> _logger;
        private readonly ISentimentAnalysisService _sentimentAnalysisService;

        public DiaryItemService(ApplicationDbContext context, ILogger<DiaryItemService> logger, ISentimentAnalysisService sentimentAnalysisService)
        {
            _context = context;
            _logger = logger;
            _sentimentAnalysisService = sentimentAnalysisService;
        }

        public DiaryItem Create(int userId, string emotion, int grade, string? note = null)
        {
            if (string.IsNullOrEmpty(emotion))
                throw new ArgumentException("Emotion is required");
            
            if (grade < 1 || grade > 5)
                throw new ArgumentException("Grade must be between 1 and 5");

            var today = DateTime.UtcNow.Date;
            var existingEntry = _context.DiaryItems
                .FirstOrDefault(di => di.UserId == userId && di.CreatedAt.Date == today);

            if (existingEntry != null)
                throw new InvalidOperationException("An entry already exists for today");

            var diaryItem = new DiaryItem
            {
                UserId = userId,
                Emotion = emotion,
                Grade = grade,
                Note = note,
                CreatedAt = DateTime.UtcNow
            };

            _context.DiaryItems.Add(diaryItem);
            _context.SaveChanges();
            
            _logger.LogInformation($"Created diary entry for user {userId}");
            return diaryItem;
        }

        public async Task<DiaryItem> CreateWithSentimentAnalysis(int userId, int grade, string? note = null)
        {
            if (grade < 1 || grade > 5)
                throw new ArgumentException("Grade must be between 1 and 5");

            var today = DateTime.UtcNow.Date;
            var existingEntry = _context.DiaryItems
                .FirstOrDefault(di => di.UserId == userId && di.CreatedAt.Date == today);

            if (existingEntry != null)
                throw new InvalidOperationException("An entry already exists for today");

            var suggestedEmotion = "neutral";
            if (!string.IsNullOrWhiteSpace(note))
            {
                suggestedEmotion = await _sentimentAnalysisService.SuggestEmotionAsync(note);
                _logger.LogInformation($"Suggested emotion '{suggestedEmotion}' for user {userId} based on note");
            }

            var diaryItem = new DiaryItem
            {
                UserId = userId,
                Emotion = suggestedEmotion,
                Grade = grade,
                Note = note,
                CreatedAt = DateTime.UtcNow
            };

            _context.DiaryItems.Add(diaryItem);
            _context.SaveChanges();
            
            _logger.LogInformation($"Created diary entry with sentiment analysis for user {userId}");
            return diaryItem;
        }

        public async Task<SentimentResponse> AnalyzeNoteSentimentAsync(string note)
        {
            if (string.IsNullOrWhiteSpace(note))
            {
                return new SentimentResponse
                {
                    SentimentScore = 0.5,
                    SentimentLabel = "neutral",
                    Confidence = 0.0,
                    DetectedEmotion = "neutral"
                };
            }

            return await _sentimentAnalysisService.AnalyzeSentimentAsync(note);
        }

        public DiaryItem? GetByDate(int userId, DateTime date)
        {
            return _context.DiaryItems
                .FirstOrDefault(di => di.UserId == userId && di.CreatedAt.Date == date.Date);
        }

        public List<DiaryItem> GetByUserId(int userId)
        {
            return _context.DiaryItems
                .Where(di => di.UserId == userId)
                .OrderByDescending(di => di.CreatedAt)
                .ToList();
        }

        public bool Update(int id, string emotion, int grade, int userId, string? note = null)
        {
            var diaryItem = _context.DiaryItems.Find(id);
            if (diaryItem == null)
                return false;
            if(userId != diaryItem.UserId)
                return false;

            diaryItem.Emotion = emotion;
            diaryItem.Grade = grade;
            diaryItem.Note = note;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id, int userId)
        {
            var diaryItem = _context.DiaryItems.Find(id);
            if (diaryItem == null)
                return false;
            if(userId != diaryItem.UserId)
                return false;

            _context.DiaryItems.Remove(diaryItem);
            _context.SaveChanges();
            return true;
        }
    }
}

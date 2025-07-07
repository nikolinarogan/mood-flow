namespace MoodFlow.DTOs
{
    public class CreateDiaryItemWithSentimentRequest
    {
        public int Grade { get; set; }
        public string? Note { get; set; }
    }

    public class SentimentAnalysisRequest
    {
        public string Text { get; set; } = string.Empty;
    }

    public class SentimentAnalysisResponse
    {
        public double SentimentScore { get; set; }
        public string SentimentLabel { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public string DetectedEmotion { get; set; } = string.Empty;
    }

    public class EmotionSuggestionResponse
    {
        public string SuggestedEmotion { get; set; } = string.Empty;
    }
} 
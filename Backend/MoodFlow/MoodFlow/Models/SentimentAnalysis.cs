namespace MoodFlow.Models
{
    // Sentiment Analysis Models
    public class SentimentRequest
    {
        public string Text { get; set; } = string.Empty;
    }

    public class SentimentResponse
    {
        public double SentimentScore { get; set; }
        public string SentimentLabel { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public string DetectedEmotion { get; set; } = string.Empty;
    }

    // Hugging Face API Response
    public class HuggingFaceResponse
    {
        public List<HuggingFaceLabel>? Labels { get; set; }
    }

    public class HuggingFaceLabel
    {
        public string? Label { get; set; }
        public double? Score { get; set; }
    }

    // Text Analytics API Response (Azure Cognitive Services - Free Tier)
    public class TextAnalyticsResponse
    {
        public List<TextAnalyticsDocument>? Documents { get; set; }
    }

    public class TextAnalyticsDocument
    {
        public string? Id { get; set; }
        public SentimentAnalysisResult? Sentiment { get; set; }
    }

    public class SentimentAnalysisResult
    {
        public string? Sentiment { get; set; }
        public SentimentConfidenceScores? ConfidenceScores { get; set; }
    }

    public class SentimentConfidenceScores
    {
        public double Positive { get; set; }
        public double Neutral { get; set; }
        public double Negative { get; set; }
    }
} 
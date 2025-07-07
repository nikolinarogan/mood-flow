using System.Text;
using System.Text.Json;
using MoodFlow.Models;

namespace MoodFlow.Services
{
    public interface ISentimentAnalysisService
    {
        Task<SentimentResponse> AnalyzeSentimentAsync(string text);
        Task<string> SuggestEmotionAsync(string text);
    }

    public class SentimentAnalysisService : ISentimentAnalysisService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SentimentAnalysisService> _logger;
        private readonly IConfiguration _configuration;

        public SentimentAnalysisService(HttpClient httpClient, ILogger<SentimentAnalysisService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<SentimentResponse> AnalyzeSentimentAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new SentimentResponse
                {
                    SentimentScore = 0.5,
                    SentimentLabel = "neutral",
                    Confidence = 0.0,
                    DetectedEmotion = "neutral"
                };
            }

            try
            {
                // Try Hugging Face API first (completely free)
                var huggingFaceResult = await AnalyzeWithHuggingFaceAsync(text);
                if (huggingFaceResult != null)
                {
                    return huggingFaceResult;
                }

                // Fallback to Azure Text Analytics (free tier available)
                var azureResult = await AnalyzeWithAzureAsync(text);
                if (azureResult != null)
                {
                    return azureResult;
                }

                // Final fallback - simple keyword-based analysis
                return AnalyzeWithKeywords(text);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing sentiment for text: {Text}", text);
                return AnalyzeWithKeywords(text);
            }
        }

        public async Task<string> SuggestEmotionAsync(string text)
        {
            var sentiment = await AnalyzeSentimentAsync(text);
            
            // Map sentiment to emotion based on score and keywords
            return MapSentimentToEmotion(sentiment, text);
        }

        private async Task<SentimentResponse?> AnalyzeWithHuggingFaceAsync(string text)
        {
            try
            {
                var huggingFaceToken = _configuration["HuggingFace:ApiToken"];
                if (string.IsNullOrEmpty(huggingFaceToken))
                {
                    _logger.LogWarning("Hugging Face API token not configured");
                    return null;
                }

                var request = new
                {
                    inputs = text
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", huggingFaceToken);

                var response = await _httpClient.PostAsync(
                    "https://api-inference.huggingface.co/models/cardiffnlp/twitter-roberta-base-sentiment-latest",
                    content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var huggingFaceResponse = JsonSerializer.Deserialize<List<HuggingFaceLabel>>(responseContent);

                    if (huggingFaceResponse != null && huggingFaceResponse.Any())
                    {
                        var topResult = huggingFaceResponse.OrderByDescending(x => x.Score).First();
                        
                        return new SentimentResponse
                        {
                            SentimentScore = MapHuggingFaceLabelToScore(topResult.Label),
                            SentimentLabel = MapHuggingFaceLabelToSentiment(topResult.Label),
                            Confidence = topResult.Score ?? 0.0,
                            DetectedEmotion = MapSentimentToEmotion(new SentimentResponse 
                            { 
                                SentimentScore = MapHuggingFaceLabelToScore(topResult.Label) 
                            }, text)
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error with Hugging Face API");
            }

            return null;
        }

        private async Task<SentimentResponse?> AnalyzeWithAzureAsync(string text)
        {
            try
            {
                var azureKey = _configuration["AzureTextAnalytics:ApiKey"];
                var azureEndpoint = _configuration["AzureTextAnalytics:Endpoint"];

                if (string.IsNullOrEmpty(azureKey) || string.IsNullOrEmpty(azureEndpoint))
                {
                    _logger.LogWarning("Azure Text Analytics not configured");
                    return null;
                }

                var request = new
                {
                    documents = new[]
                    {
                        new
                        {
                            id = "1",
                            text = text,
                            language = "en"
                        }
                    }
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", azureKey);

                var response = await _httpClient.PostAsync(
                    $"{azureEndpoint}/text/analytics/v3.2-preview.1/sentiment",
                    content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var azureResponse = JsonSerializer.Deserialize<TextAnalyticsResponse>(responseContent);

                    if (azureResponse?.Documents?.Any() == true)
                    {
                        var document = azureResponse.Documents.First();
                        var sentiment = document.Sentiment;

                        if (sentiment != null)
                        {
                            return new SentimentResponse
                            {
                                SentimentScore = MapAzureSentimentToScore(sentiment.Sentiment),
                                SentimentLabel = sentiment.Sentiment?.ToLower() ?? "neutral",
                                Confidence = GetHighestConfidence(sentiment.ConfidenceScores),
                                DetectedEmotion = MapSentimentToEmotion(new SentimentResponse 
                                { 
                                    SentimentScore = MapAzureSentimentToScore(sentiment.Sentiment) 
                                }, text)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error with Azure Text Analytics API");
            }

            return null;
        }

        private SentimentResponse AnalyzeWithKeywords(string text)
        {
            var lowerText = text.ToLower();
            
            // Simple keyword-based sentiment analysis
            var positiveWords = new[] { "happy", "great", "excellent", "wonderful", "amazing", "love", "good", "nice", "beautiful", "fantastic", "excited", "joy", "pleased", "satisfied", "grateful", "blessed", "fortunate", "lucky", "thrilled", "delighted" };
            var negativeWords = new[] { "sad", "terrible", "awful", "horrible", "bad", "hate", "angry", "frustrated", "disappointed", "upset", "worried", "anxious", "stressed", "depressed", "miserable", "hopeless", "desperate", "furious", "irritated", "annoyed" };
            var neutralWords = new[] { "okay", "fine", "alright", "normal", "usual", "routine", "ordinary", "average", "standard", "regular", "typical", "common", "neutral", "indifferent", "unconcerned" };

            var positiveCount = positiveWords.Count(word => lowerText.Contains(word));
            var negativeCount = negativeWords.Count(word => lowerText.Contains(word));
            var neutralCount = neutralWords.Count(word => lowerText.Contains(word));

            var totalWords = positiveCount + negativeCount + neutralCount;
            
            if (totalWords == 0)
            {
                return new SentimentResponse
                {
                    SentimentScore = 0.5,
                    SentimentLabel = "neutral",
                    Confidence = 0.3,
                    DetectedEmotion = "neutral"
                };
            }

            var sentimentScore = (positiveCount - negativeCount) / (double)totalWords;
            sentimentScore = (sentimentScore + 1) / 2; // Normalize to 0-1

            return new SentimentResponse
            {
                SentimentScore = sentimentScore,
                SentimentLabel = sentimentScore > 0.6 ? "positive" : sentimentScore < 0.4 ? "negative" : "neutral",
                Confidence = Math.Min(0.8, totalWords * 0.1),
                DetectedEmotion = MapSentimentToEmotion(new SentimentResponse { SentimentScore = sentimentScore }, text)
            };
        }

        private double MapHuggingFaceLabelToScore(string? label)
        {
            return label?.ToLower() switch
            {
                "positive" => 0.8,
                "negative" => 0.2,
                "neutral" => 0.5,
                _ => 0.5
            };
        }

        private string MapHuggingFaceLabelToSentiment(string? label)
        {
            return label?.ToLower() switch
            {
                "positive" => "positive",
                "negative" => "negative",
                "neutral" => "neutral",
                _ => "neutral"
            };
        }

        private double MapAzureSentimentToScore(string? sentiment)
        {
            return sentiment?.ToLower() switch
            {
                "positive" => 0.8,
                "negative" => 0.2,
                "neutral" => 0.5,
                _ => 0.5
            };
        }

        private double GetHighestConfidence(SentimentConfidenceScores? scores)
        {
            if (scores == null) return 0.0;
            return Math.Max(Math.Max(scores.Positive, scores.Neutral), scores.Negative);
        }

        private string MapSentimentToEmotion(SentimentResponse sentiment, string text)
        {
            var lowerText = text.ToLower();
            
            // Emotion-specific keywords
            if (lowerText.Contains("excited") || lowerText.Contains("thrilled") || lowerText.Contains("pumped"))
                return "excited";
            if (lowerText.Contains("happy") || lowerText.Contains("joy") || lowerText.Contains("delighted"))
                return "happy";
            if (lowerText.Contains("calm") || lowerText.Contains("peaceful") || lowerText.Contains("relaxed"))
                return "calm";
            if (lowerText.Contains("sad") || lowerText.Contains("depressed") || lowerText.Contains("melancholy"))
                return "sad";
            if (lowerText.Contains("anxious") || lowerText.Contains("worried") || lowerText.Contains("nervous"))
                return "anxious";
            if (lowerText.Contains("angry") || lowerText.Contains("furious") || lowerText.Contains("irritated"))
                return "angry";
            if (lowerText.Contains("tired") || lowerText.Contains("exhausted") || lowerText.Contains("sleepy"))
                return "tired";

            // Map sentiment score to emotion
            return sentiment.SentimentScore switch
            {
                > 0.7 => "happy",
                > 0.5 => "calm",
                > 0.3 => "neutral",
                > 0.1 => "sad",
                _ => "sad"
            };
        }
    }
} 
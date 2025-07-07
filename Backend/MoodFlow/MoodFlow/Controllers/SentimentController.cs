using Microsoft.AspNetCore.Mvc;
using MoodFlow.Models;
using MoodFlow.Services;

namespace MoodFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SentimentController : ControllerBase
    {
        private readonly ISentimentAnalysisService _sentimentAnalysisService;
        private readonly ILogger<SentimentController> _logger;

        public SentimentController(ISentimentAnalysisService sentimentAnalysisService, ILogger<SentimentController> logger)
        {
            _sentimentAnalysisService = sentimentAnalysisService;
            _logger = logger;
        }

        [HttpPost("analyze")]
        public async Task<ActionResult<SentimentResponse>> AnalyzeSentiment([FromBody] SentimentRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Text))
                {
                    return BadRequest("Text is required");
                }

                var result = await _sentimentAnalysisService.AnalyzeSentimentAsync(request.Text);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing sentiment");
                return StatusCode(500, "Error analyzing sentiment");
            }
        }

        [HttpPost("suggest-emotion")]
        public async Task<ActionResult<string>> SuggestEmotion([FromBody] SentimentRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Text))
                {
                    return BadRequest("Text is required");
                }

                var suggestedEmotion = await _sentimentAnalysisService.SuggestEmotionAsync(request.Text);
                return Ok(new { suggestedEmotion });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error suggesting emotion");
                return StatusCode(500, "Error suggesting emotion");
            }
        }

        [HttpPost("analyze-note")]
        public async Task<ActionResult<SentimentResponse>> AnalyzeNote([FromBody] SentimentRequest request)
        {
            try
            {
                var result = await _sentimentAnalysisService.AnalyzeSentimentAsync(request.Text ?? "");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing note sentiment");
                return StatusCode(500, "Error analyzing note sentiment");
            }
        }
    }
} 
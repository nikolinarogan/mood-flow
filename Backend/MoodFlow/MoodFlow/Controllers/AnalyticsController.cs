using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MoodFlow.Services;
using System.Security.Claims;
using MoodFlow.Models;

namespace MoodFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IDiaryItemService _diaryService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(IDiaryItemService diaryService, ILogger<AnalyticsController> logger)
        {
            _diaryService = diaryService;
            _logger = logger;
        }

        [HttpGet("summary")]
        public ActionResult GetAnalyticsSummary()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var entries = _diaryService.GetByUserId(userId);
                
                if (!entries.Any())
                {
                    return Ok(new
                    {
                        totalEntries = 0,
                        averageMood = 0,
                        bestDay = "N/A",
                        mostCommonEmotion = "N/A",
                        monthlyStats = new
                        {
                            entries = 0,
                            averageMood = 0,
                            bestDay = "N/A",
                            lowestMood = 0,
                            highestMood = 0
                        },
                        weeklyMoodData = new int?[4][],
                        insights = new[]
                        {
                            new
                            {
                                icon = "📝",
                                title = "Start Your Journey",
                                description = "Begin tracking your mood to see insights and patterns emerge."
                            }
                        }
                    });
                }

                // Calculate summary statistics
                var totalEntries = entries.Count;
                var averageMood = entries.Average(e => e.Grade);
                var bestEntry = entries.OrderByDescending(e => e.Grade).First();
                var bestDay = bestEntry.CreatedAt.ToLocalTime().ToString("MM/dd/yyyy");

                // Calculate most common emotion
                var emotionCounts = entries.GroupBy(e => e.Emotion)
                    .Select(g => new { Emotion = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .First();
                var mostCommonEmotion = emotionCounts.Emotion;

                // Calculate monthly statistics
                var now = DateTime.Now;
                var monthEntries = entries.Where(e => 
                    e.CreatedAt.Month == now.Month && 
                    e.CreatedAt.Year == now.Year).ToList();

                var monthlyStats = new
                {
                    entries = monthEntries.Count,
                    averageMood = monthEntries.Any() ? monthEntries.Average(e => e.Grade) : 0,
                    bestDay = monthEntries.Any() ? monthEntries.OrderByDescending(e => e.Grade).First().CreatedAt.ToLocalTime().ToString("MM/dd/yyyy") : "N/A",
                    lowestMood = monthEntries.Any() ? monthEntries.Min(e => e.Grade) : 0,
                    highestMood = monthEntries.Any() ? monthEntries.Max(e => e.Grade) : 0
                };

                // Calculate weekly mood data (last 4 weeks)
                var weeklyMoodData = CalculateWeeklyMoodData(entries);

                // Generate insights
                var insights = GenerateInsights(entries);

                return Ok(new
                {
                    totalEntries,
                    averageMood = Math.Round(averageMood, 1),
                    bestDay,
                    mostCommonEmotion,
                    monthlyStats,
                    weeklyMoodData,
                    insights
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting analytics summary");
                return StatusCode(500, new { message = "An error occurred while getting analytics" });
            }
        }

        private int?[][] CalculateWeeklyMoodData(List<DiaryItem> entries)
        {
            var weeks = new int?[4][];
            var now = DateTime.Now;
            var startDate = now.AddDays(-28); // 4 weeks back

            for (int week = 0; week < 4; week++)
            {
                weeks[week] = new int?[7];
                for (int day = 0; day < 7; day++)
                {
                    var date = startDate.AddDays(week * 7 + day);
                    var entry = entries.FirstOrDefault(e => 
                        e.CreatedAt.Date == date.Date);
                    
                    weeks[week][day] = entry?.Grade;
                }
            }

            return weeks;
        }

        private object[] GenerateInsights(List<DiaryItem> entries)
        {
            var insights = new List<object>();

            if (!entries.Any())
            {
                return new[]
                {
                    new
                    {
                        icon = "📝",
                        title = "Start Your Journey",
                        description = "Begin tracking your mood to see insights and patterns emerge."
                    }
                };
            }

            // Mood trend insight
            var recentEntries = entries.OrderByDescending(e => e.CreatedAt).Take(7).ToList();
            if (recentEntries.Count >= 2)
            {
                var recentAvg = recentEntries.Average(e => e.Grade);
                var olderEntries = entries.OrderByDescending(e => e.CreatedAt).Skip(7).Take(7).ToList();
                
                if (olderEntries.Count >= 2)
                {
                    var olderAvg = olderEntries.Average(e => e.Grade);
                    
                    if (recentAvg > olderAvg + 1)
                    {
                        insights.Add(new
                        {
                            icon = "📈",
                            title = "Improving Mood",
                            description = "Your mood has been improving over the past week!"
                        });
                    }
                    else if (recentAvg < olderAvg - 1)
                    {
                        insights.Add(new
                        {
                            icon = "📉",
                            title = "Mood Decline",
                            description = "Consider what might be affecting your mood recently."
                        });
                    }
                }
            }

            // Consistency insight
            var consistency = entries.Count / 30.0; // entries per day
            if (consistency > 0.8)
            {
                insights.Add(new
                {
                    icon = "🎯",
                    title = "Great Consistency",
                    description = "You're tracking your mood regularly - keep it up!"
                });
            }
            else if (consistency < 0.3)
            {
                insights.Add(new
                {
                    icon = "💡",
                    title = "Build the Habit",
                    description = "Try to track your mood daily for better insights."
                });
            }

            // Emotion insight
            var mostCommonEmotion = entries.GroupBy(e => e.Emotion)
                .OrderByDescending(g => g.Count())
                .First().Key;
            
            insights.Add(new
            {
                icon = "😊",
                title = "Frequent Emotion",
                description = $"\"{mostCommonEmotion}\" appears most often in your entries."
            });

            return insights.ToArray();
        }
    }
} 
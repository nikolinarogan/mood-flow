using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoodFlow.Data;
using Microsoft.EntityFrameworkCore;

namespace MoodFlow.Services
{
    public class DailyReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DailyReminderService> _logger;
        private readonly TimeSpan _reminderTime = new TimeSpan(2, 12, 0); // 2:10 AM (for testing)

        public DailyReminderService(IServiceProvider serviceProvider, ILogger<DailyReminderService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Daily Reminder Service started");
            _logger.LogInformation($"Reminder time set to: {_reminderTime:hh\\:mm}");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.UtcNow;
                    var nextReminder = GetNextReminderTime(now);

                    _logger.LogInformation($"Current time: {now:yyyy-MM-dd HH:mm:ss}");
                    _logger.LogInformation($"Next reminder scheduled for: {nextReminder:yyyy-MM-dd HH:mm:ss}");

                    // Wait until next reminder time
                    var delay = nextReminder - now;
                    if (delay > TimeSpan.Zero)
                    {
                        _logger.LogInformation($"Waiting {delay.TotalMinutes:F1} minutes until next reminder");
                        await Task.Delay(delay, stoppingToken);
                    }

                    if (!stoppingToken.IsCancellationRequested)
                    {
                        await SendDailyReminders();
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Daily Reminder Service stopped");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Daily Reminder Service");
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }
        }

        private DateTime GetNextReminderTime(DateTime now)
        {
            var todayReminder = now.Date.Add(_reminderTime);
            
            // If it's past the reminder time today, schedule for tomorrow
            if (now >= todayReminder)
            {
                return todayReminder.AddDays(1);
            }
            
            return todayReminder;
        }

        private async Task SendDailyReminders()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            try
            {
                var today = DateTime.UtcNow.Date;
                
                // Get all users
                var allUsers = await dbContext.Users.ToListAsync();
                
                if (allUsers.Count == 0)
                {
                    return;
                }
                
                var usersToRemind = new List<object>();

                foreach (var user in allUsers)
                {
                    try
                    {
                        // Check if user has an entry for today's date
                        var hasEntryToday = await dbContext.DiaryItems
                            .AnyAsync(d => d.UserId == user.Id && d.CreatedAt.Date == today);

                        if (hasEntryToday)
                        {
                            continue;
                        }

                        await emailService.SendDailyReminder(user.Email, user.Username);
                        _logger.LogInformation($"Sent reminder to {user.Email}");
                        
                        usersToRemind.Add(new { user = user.Username, email = user.Email });
                        
                        // Small delay between emails
                        await Task.Delay(1000);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to send reminder to {user.Email}");
                    }
                }

                _logger.LogInformation($"Daily reminder process completed. Sent reminders to {usersToRemind.Count} users.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during daily reminder process");
            }
        }
    }
} 
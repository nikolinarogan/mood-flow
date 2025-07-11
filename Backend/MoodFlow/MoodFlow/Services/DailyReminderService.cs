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

        public DailyReminderService(IServiceProvider serviceProvider, ILogger<DailyReminderService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Daily Reminder Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var now = DateTime.UtcNow;
                    var today = now.Date;

                    var allUsers = await dbContext.Users.ToListAsync();
                    foreach (var user in allUsers)
                    {
                        var notificationTime = user.NotificationTime ?? new TimeSpan(9, 0, 0); // default 9:00 AM
                        var userReminderDateTime = today.Add(notificationTime);

                        _logger.LogInformation($"Next notification for {user.Email} scheduled at {userReminderDateTime:yyyy-MM-dd HH:mm} UTC. Now: {now:yyyy-MM-dd HH:mm} UTC.");
                        _logger.LogInformation($"User {user.Email}: now={now:HH:mm}, notificationTime={notificationTime}, userReminderDateTime={userReminderDateTime:HH:mm}");

                        // Debug: Log all diary entries for this user
                        var entriesToday = await dbContext.DiaryItems
                            .Where(d => d.UserId == user.Id)
                            .ToListAsync();
                        foreach (var entry in entriesToday)
                        {
                            _logger.LogInformation($"Entry for {user.Email}: CreatedAt={entry.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC");
                        }

                        // If current time is within 5 minute window of user's notification time
                        if (now >= userReminderDateTime && now < userReminderDateTime.AddMinutes(5))
                        {
                            // Check if user has an entry for today's date
                            var hasEntryToday = entriesToday.Any(d => d.CreatedAt.Date == today);
                            if (!hasEntryToday)
                            {
                                try
                                {
                                    _logger.LogInformation($"Sending reminder to {user.Email} at {now:HH:mm}");
                                    await emailService.SendDailyReminder(user.Email, user.Username);
                                    _logger.LogInformation($"Sent reminder to {user.Email}");
                                    await Task.Delay(1000, stoppingToken); // Small delay between emails
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, $"Failed to send reminder to {user.Email}");
                                }
                            }
                            else
                            {
                                _logger.LogInformation($"User {user.Email} already has a diary entry for today.");
                            }
                        }
                        else
                        {
                            _logger.LogInformation($"User {user.Email}: Not time to send reminder yet.");
                        }
                    }

                    // Wait 1 minute before checking again
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Daily Reminder Service stopped");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Daily Reminder Service");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }
    }
} 
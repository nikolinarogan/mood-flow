using System.Net.Mail;
using System.Net;

namespace MoodFlow.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmail(string email, string username, string token);
        Task SendDailyReminder(string email, string username);
    }
    public class EmailService : IEmailService
    {
       private readonly IConfiguration _configuration;
       private readonly ILogger<EmailService> _logger;

       public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendVerificationEmail(string email, string username, string token)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSetting");
                var fromName = smtpSettings["FromName"];
                var fromEmail = smtpSettings["FromEmail"];
                var smtpHost = smtpSettings["Host"];
                var smtpPort = int.Parse(smtpSettings["Port"]);
                var smtpUsername = smtpSettings["Username"];
                var smtpPassword = smtpSettings["Password"];

                if (string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
                {
                    _logger.LogWarning("SMTP credentials not configured. Email verification will be skipped.");
                    return; 
                }

                if (string.IsNullOrEmpty(fromEmail))
                {
                    _logger.LogWarning("FromEmail not configured. Email verification will be skipped.");
                    return;
                }

                var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = true
                };

                var verificationLink = $"{_configuration["AppUrl"]}/api/auth/verify-email?token={token}";

                var subject = "Verify Your Email - Mood Flow";
                var body = $@"
                    <h2>Welcome to Mood Flow, {username}!</h2>
                    <p>Thank you for registering. Please click the link below to verify your email address:</p>
                    <p><a href='{verificationLink}'>Verify Email Address</a></p>
                    <p>This link will expire in 24 hours.</p>
                    <p>Best regards,<br>Mood Flow Team</p>";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Verification email sent to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send verification email to {email}");
                throw new Exception("Failed to send verification email");
            }
        }

        public async Task SendDailyReminder(string email, string username)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSetting");
                var fromName = smtpSettings["FromName"];
                var fromEmail = smtpSettings["FromEmail"];
                var smtpHost = smtpSettings["Host"];
                var smtpPort = int.Parse(smtpSettings["Port"]);
                var smtpUsername = smtpSettings["Username"];
                var smtpPassword = smtpSettings["Password"];

                if (string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
                {
                    _logger.LogWarning("SMTP credentials not configured. Daily reminder will be skipped.");
                    return;
                }

                if (string.IsNullOrEmpty(fromEmail))
                {
                    _logger.LogWarning("FromEmail not configured. Daily reminder will be skipped.");
                    return;
                }

                var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = true
                };

                var appUrl = _configuration["AppUrl"];
                var subject = "Time to Track Your Mood - Mood Flow";
                var body = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 10px; text-align: center;'>
                            <h1 style='margin: 0; font-size: 24px;'>🌊 Mood Flow</h1>
                        </div>
                        
                        <div style='padding: 20px; background: #f8f9fa; border-radius: 10px; margin-top: 20px;'>
                            <h2 style='color: #2c3e50; margin-bottom: 15px;'>Hi {username}! 👋</h2>
                            
                            <p style='color: #555; line-height: 1.6; margin-bottom: 20px;'>
                                It's 5:00 PM and we noticed you haven't tracked your mood today. 
                                Taking a moment to reflect on your emotions can help you understand your patterns and improve your well-being.
                            </p>
                            
                            <div style='background: white; padding: 15px; border-radius: 8px; border-left: 4px solid #3498db; margin: 20px 0;'>
                                <h3 style='color: #2c3e50; margin: 0 0 10px 0;'>💡 Why track your mood?</h3>
                                <ul style='color: #555; margin: 0; padding-left: 20px;'>
                                    <li>Identify patterns in your emotional well-being</li>
                                    <li>Track progress over time</li>
                                    <li>Gain insights into what affects your mood</li>
                                    <li>Build better emotional awareness</li>
                                </ul>
                            </div>
                            
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{appUrl}' 
                                   style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); 
                                          color: white; 
                                          padding: 12px 30px; 
                                          text-decoration: none; 
                                          border-radius: 25px; 
                                          display: inline-block; 
                                          font-weight: bold;
                                          box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);'>
                                    📝 Track My Mood Now
                                </a>
                            </div>
                            
                            <p style='color: #7f8c8d; font-size: 14px; text-align: center; margin-top: 20px;'>
                                It only takes 2 minutes to reflect on your day!
                            </p>
                        </div>
                        
                        <div style='text-align: center; margin-top: 20px; color: #7f8c8d; font-size: 12px;'>
                            <p>You're receiving this because you haven't logged your mood today.</p>
                            <p>© 2024 Mood Flow. All rights reserved.</p>
                        </div>
                    </div>";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Daily reminder email sent to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send daily reminder email to {email}");
                throw new Exception("Failed to send daily reminder email");
            }
        }
    }
}

using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Logging;

namespace MoodFlow.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmail(string email, string username, string token);
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
        }
    }

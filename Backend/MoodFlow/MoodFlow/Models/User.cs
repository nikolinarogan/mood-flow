using System.ComponentModel.DataAnnotations;

namespace MoodFlow.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(70)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsEmailVerified { get; set; } = false;
        public string EmailVerificationToken { get; set; } = string.Empty;
        public DateTime? EmailVerificationTokenExp { get; set; } 
        public TimeSpan? NotificationTime { get; set; }


        public User() { }

       public User(int id, string username, string email, string password, DateTime createdDate, DateTime? lastLoginAt, bool isEmailVerified, string emailVerificationToken, DateTime? emailVerificationTokenExp)
        {
            Id = id;
            Username = username;
            Email = email;
            Password = password;
            CreatedDate = createdDate;
            LastLoginAt = lastLoginAt;
            IsEmailVerified = isEmailVerified;
            EmailVerificationToken = emailVerificationToken;
            EmailVerificationTokenExp = emailVerificationTokenExp;
        }
    }
}

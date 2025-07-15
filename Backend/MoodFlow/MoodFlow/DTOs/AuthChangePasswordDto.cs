using System.ComponentModel.DataAnnotations;

namespace MoodFlow.DTOs
{
    public class AuthChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
} 
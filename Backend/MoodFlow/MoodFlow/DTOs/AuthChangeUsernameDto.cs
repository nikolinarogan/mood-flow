using System.ComponentModel.DataAnnotations;

namespace MoodFlow.DTOs
{
    public class AuthChangeUsernameDto
    {
        [Required]
        public string NewUsername { get; set; } = string.Empty;
    }
} 
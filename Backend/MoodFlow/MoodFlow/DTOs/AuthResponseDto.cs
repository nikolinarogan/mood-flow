namespace MoodFlow.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}

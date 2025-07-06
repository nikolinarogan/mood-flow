namespace MoodFlow.DTOs
{
    public class CreateDiaryItemRequest
    {
        public string Emotion { get; set; } = string.Empty;
        public int Grade { get; set; }
        public string? Note { get; set; }
    }

    public class UpdateDiaryItemRequest
    {
        public string Emotion { get; set; } = string.Empty;
        public int Grade { get; set; }
        public string? Note { get; set; }
    }

    public class DiaryItemResponse
    {
        public int Id { get; set; }
        public string Emotion { get; set; } = string.Empty;
        public int Grade { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 
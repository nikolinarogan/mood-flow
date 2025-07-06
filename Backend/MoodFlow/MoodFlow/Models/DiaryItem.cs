namespace MoodFlow.Models
{
    public class DiaryItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Emotion { get; set; }

        public string? Note { get; set; }
        public int Grade { get; set; }
        public DateTime CreatedAt { get; set; }

        public DiaryItem() { }
        public DiaryItem(int id, int userId, string emotion, string? note, int grade, DateTime createdAt)
        {
            Id = id;
            UserId = userId;
            Emotion = emotion;
            Note = note;
            Grade = grade;
            CreatedAt = createdAt;
        }
        public DiaryItem(int userId, string emotion, string? note, int grade, DateTime createdAt)
        {
            UserId = userId;
            Emotion = emotion;
            Note = note;
            Grade = grade;
            CreatedAt = createdAt;
        }
    }
}

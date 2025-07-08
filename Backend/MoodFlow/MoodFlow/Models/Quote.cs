namespace MoodFlow.Models
{
    public class Quote
    {
        public int Id { get; set; }
        public string QuoteText { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public bool IsFavourite { get; set; } = false;

        public Quote() { }
    }
}

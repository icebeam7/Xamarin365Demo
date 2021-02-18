namespace Xamarin365Demo.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Customer { get; set; }
        public string Text { get; set; }
        public string LanguageCode { get; set; }
        public string Language { get; set; }
        public int Rating { get; set; }
        public double SentimentScore { get; set; }
    }
}

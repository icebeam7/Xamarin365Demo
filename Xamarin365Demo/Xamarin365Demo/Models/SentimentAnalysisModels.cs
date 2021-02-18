using System.Collections.Generic;

namespace Xamarin365Demo.Models
{
    public class DocumentSentiment
    {
        public string Id { get; set; }
        public double Score { get; set; }
    }

    public class SentimentAnalysisResult
    {
        public List<DocumentSentiment> Documents { get; set; }
        public object Errors { get; set; }
    }
}

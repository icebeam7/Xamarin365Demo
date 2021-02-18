using System;
using System.Collections.Generic;

namespace Xamarin365Demo.Models
{
    public class DetectedLanguage
    {
        public string Name { get; set; }
        public string Iso6391Name { get; set; }
        public double Score { get; set; }
    }

    public class DocumentLanguage
    {
        public string Id { get; set; }
        public List<DetectedLanguage> DetectedLanguages { get; set; }
    }

    public class LanguageDetectionResult
    {
        public List<DocumentLanguage> Documents { get; set; }
        public object Errors { get; set; }
    }
}

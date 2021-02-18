using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

using Xamarin365Demo.Models;

using Newtonsoft.Json;
using Xamarin365Demo.Helpers;

namespace Xamarin365Demo.Services
{
    public class TextAnalyticsService
    {
        private static HttpClient CreateHttpClient(string url)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Constants.TextAnalyticsKey);

            return client;
        }

        private static readonly HttpClient client = CreateHttpClient(Constants.TextAnalyticsEndpoint);

        private static List<DocumentRequest> PrepareDocumentsForLanguage(List<Review> reviews)
        {
            var documents = new List<DocumentRequest>();

            foreach (var review in reviews)
            {
                var document = new DocumentRequest()
                {
                    Id = review.Id.ToString(),
                    Text = review.Text
                };

                documents.Add(document);
            }

            return documents;
        }


        private static List<DocumentRequest> PrepareDocumentsForSentiment(List<Review> reviews)
        {
            var documents = new List<DocumentRequest>();

            foreach (var review in reviews)
            {
                var document = new DocumentRequest()
                {
                    Id = review.Id.ToString(),
                    Text = review.Text,
                    Language = review.LanguageCode
                };

                documents.Add(document);
            }

            return documents;
        }

        public async static Task<List<Review>> AnalyzeText(List<Review> reviews)
        {
            var documents_LD = PrepareDocumentsForLanguage(reviews);
            var jsonDocuments_LD = JsonConvert.SerializeObject(new { documents = documents_LD });

            var content_LD = new StringContent(jsonDocuments_LD);
            content_LD.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response_LD = await client.PostAsync(Constants.LanguageDetectionServiceUrl, content_LD);

            if (response_LD.IsSuccessStatusCode)
            {
                var json_LD = await response_LD.Content.ReadAsStringAsync();
                var result_LD = JsonConvert.DeserializeObject<LanguageDetectionResult>(json_LD);

                ProcessLanguageResult(result_LD, reviews);

                // Prepare documents for sentiment ------- Process SentimentResult
                var documents_SA = PrepareDocumentsForSentiment(reviews);
                var jsonDocuments_SA = JsonConvert.SerializeObject(new { documents = documents_SA });

                var content_SA = new StringContent(jsonDocuments_SA);
                content_SA.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response_SA = await client.PostAsync(Constants.SentimentAnalysisServiceUrl, content_SA);

                if (response_SA.IsSuccessStatusCode)
                {
                    var json_SA = await response_SA.Content.ReadAsStringAsync();
                    var result_SA = JsonConvert.DeserializeObject<SentimentAnalysisResult>(json_SA);

                    ProcessSentimentResult(result_SA, reviews);
                    return reviews;
                }
            }

            return new List<Review>();
        }

        private static void ProcessLanguageResult(LanguageDetectionResult result, List<Review> reviews)
        {
            foreach (var document in result.Documents)
            {
                var review = reviews.SingleOrDefault(x => x.Id == int.Parse(document.Id));

                if (review != null)
                {
                    var languageInfo = document.DetectedLanguages.FirstOrDefault();

                    if (languageInfo != null)
                    {
                        review.Language = languageInfo.Name;
                        review.LanguageCode = languageInfo.Iso6391Name;
                    }
                }
            }
        }

        private static void ProcessSentimentResult(SentimentAnalysisResult result, List<Review> reviews)
        {
            foreach (var document in result.Documents)
            {
                var review = reviews.SingleOrDefault(x => x.Id == int.Parse(document.Id));

                if (review != null)
                    review.SentimentScore = document.Score * 100;
            }
        }
    }
}

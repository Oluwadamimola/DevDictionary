using System.Net.Http;
using System.Text.Json;
using DevDictionary.Models;

namespace DevDictionary.Services
{
    public interface IDictionaryService
    {
        Task<DefinitionResult> LookupAsync(string term);
    }

    public class DictionaryService : IDictionaryService
    {
        private readonly HttpClient _httpClient;

        public DictionaryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<DefinitionResult> LookupAsync(string term)
        {
            term = term?.Trim() ?? "";
            
            if (string.IsNullOrWhiteSpace(term))
            {
                return new DefinitionResult
                {
                    Success = false,
                    ErrorMessage = "No term provided."
                };
            }

            Console.WriteLine($"🔍 Looking up: {term}");

            // Try multiple sources
            var mdn = await SearchMDN(term);
            if (mdn.Success) return mdn;

            var wiki = await SearchWikipedia(term);
            if (wiki.Success) return wiki;

            return new DefinitionResult
            {
                Success = false,
                Term = term,
                ErrorMessage = $"No definition found for '{term}'."
            };
        }

        private async Task<DefinitionResult> SearchMDN(string term)
        {
            try
            {
                var url = $"https://developer.mozilla.org/api/v1/search?q={Uri.EscapeDataString(term)}";
                Console.WriteLine($"📡 Searching MDN: {url}");
                
                var json = await _httpClient.GetStringAsync(url);
                using var doc = JsonDocument.Parse(json);
                
                if (doc.RootElement.TryGetProperty("documents", out var documents))
                {
                    var first = documents.EnumerateArray().FirstOrDefault();

                    if (first.ValueKind == JsonValueKind.Object && 
                        first.TryGetProperty("summary", out var summary) &&
                        first.TryGetProperty("mdn_url", out var mdnUrl))
                    {
                        var summaryText = summary.GetString();
                        var urlText = mdnUrl.GetString();

                        if (!string.IsNullOrEmpty(summaryText) && !string.IsNullOrEmpty(urlText))
                        {
                            Console.WriteLine("✅ Found on MDN");
                            return new DefinitionResult
                            {
                                Success = true,
                                Term = term,
                                Definition = summaryText,
                                Source = "MDN Web Docs",
                                Url = $"https://developer.mozilla.org{urlText}"
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ MDN search failed: {ex.Message}");
            }

            return new DefinitionResult { Success = false };
        }

        private async Task<DefinitionResult> SearchWikipedia(string term)
        {
            try
            {
                var url = $"https://en.wikipedia.org/api/rest_v1/page/summary/{Uri.EscapeDataString(term)}";
                Console.WriteLine($"📡 Searching Wikipedia: {url}");
                
                var json = await _httpClient.GetStringAsync(url);
                using var doc = JsonDocument.Parse(json);
                
                if (doc.RootElement.TryGetProperty("extract", out var extract) &&
                    doc.RootElement.TryGetProperty("content_urls", out var contentUrls))
                {
                    var extractText = extract.GetString();
                    var pageUrl = contentUrls.GetProperty("desktop").GetProperty("page").GetString();

                    if (!string.IsNullOrEmpty(extractText) && !string.IsNullOrEmpty(pageUrl))
                    {
                        Console.WriteLine("✅ Found on Wikipedia");
                        return new DefinitionResult
                        {
                            Success = true,
                            Term = term,
                            Definition = extractText,
                            Source = "Wikipedia",
                            Url = pageUrl
                        };
                    }
                }
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"⚠️ Term not found on Wikipedia");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Wikipedia search failed: {ex.Message}");
            }

            return new DefinitionResult { Success = false };
        }
    }
}

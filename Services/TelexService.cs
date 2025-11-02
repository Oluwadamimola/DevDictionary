using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DevDictionary.Models;
using Microsoft.Extensions.Configuration;

namespace DevDictionary.Services
{
    public interface ITelexService
    {
        Task SendMessageAsync(string channelId, string text, string? replyToMessageId = null);
    }

    public class TelexService : ITelexService
    {
        private readonly HttpClient _httpClient;
        private readonly string _telexApiBase;
        private readonly IConfiguration _configuration;

        public TelexService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            // Get Telex API endpoint from configuration
            _telexApiBase = _configuration["Telex:ApiBase"] ?? "https://api.telex.im/v1/messages";
            
            Console.WriteLine($"📡 Telex API Base: {_telexApiBase}");
        }

        public async Task SendMessageAsync(string channelId, string text, string? replyToMessageId = null)
        {
            try
            {
                Console.WriteLine($"📤 Sending message to channel: {channelId}");
                
                var payload = new TelexResponse
                {
                    ChannelId = channelId,
                    Text = text,
                    ReplyToMessageId = replyToMessageId
                };

                var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                });
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine($"📝 Payload: {json}");

                var response = await _httpClient.PostAsync(_telexApiBase, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ Message sent successfully to Telex");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Failed to send message: {response.StatusCode}");
                    Console.WriteLine($"Error details: {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception sending message: {ex.Message}");
            }
        }
    }
}
using DevDictionary.Models;
using DevDictionary.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevDictionary.Controllers
{
    [ApiController]
    [Route("telex")]
    public class TelexController : ControllerBase
    {
        private readonly IDictionaryService _dictionaryService;
        private readonly ITelexService _telexService;

        public TelexController(IDictionaryService dictionaryService, ITelexService telexService)
        {
            _dictionaryService = dictionaryService;
            _telexService = telexService;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> ReceiveEvent([FromBody] TelexWebhookEvent evt)
        {
            Console.WriteLine(" Received webhook event");
            
            // Log the event for debugging
            if (evt?.Message?.Text != null)
            {
                Console.WriteLine($" Message: {evt.Message.Text}");
                Console.WriteLine($" User: {evt.User?.Username ?? "Unknown"}");
                Console.WriteLine($" Channel: {evt.Channel?.Name ?? "Unknown"}");
            }

            // Validate required fields
            if (evt?.Message?.Text == null || evt.Channel?.Id == null || evt.Message.Id == null)
            {
                Console.WriteLine(" Missing required fields in webhook event");
                return Ok(); 
            }

            string userMessage = evt.Message.Text.Trim();
            string channelId = evt.Channel.Id;
            string replyId = evt.Message.Id;

            // Extract search term (remove common phrases)
            string searchTerm = CleanInputText(userMessage);
            
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                await _telexService.SendMessageAsync(
                    channelId, 
                    " Please provide a term to look up. Example: 'What is REST API?'",
                    replyId
                );
                return Ok();
            }

            Console.WriteLine($"🔍 Searching for: {searchTerm}");

            // Lookup definition
            var result = await _dictionaryService.LookupAsync(searchTerm);

            // Format response
            string responseText = FormatResponse(result, searchTerm);

            // Send message back to Telex
            await _telexService.SendMessageAsync(channelId, responseText, replyId);

            return Ok();
        }

        
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new 
            { 
                status = "healthy",
                service = "DevDictionary Bot",
                timestamp = DateTime.UtcNow
            });
        }

        private string CleanInputText(string input)
        {
            input = input.ToLower().Trim();

            // Remove common question phrases
            string[] removePhrases = 
            {
                "what is", "what's", "whats", 
                "explain", "define", "meaning of", 
                "tell me about", "what does", 
                "?", "!"
            };

            foreach (var phrase in removePhrases)
            {
                input = input.Replace(phrase, " ");
            }

            // Clean up extra spaces
            while (input.Contains("  "))
            {
                input = input.Replace("  ", " ");
            }

            return input.Trim();
        }

        private string FormatResponse(DefinitionResult result, string searchTerm)
        {
            if (result.Success && !string.IsNullOrEmpty(result.Definition))
            {
                // Truncate long definitions
                var definition = result.Definition.Length > 500 
                    ? result.Definition.Substring(0, 497) + "..." 
                    : result.Definition;

                return $@"📖 **{result.Term ?? searchTerm}**

                {definition}

                _Source: {result.Source}_
                {(string.IsNullOrEmpty(result.Url) ? "" : $"🔗 {result.Url}")}";
            }
            else
            {
                return $" Sorry, I couldn't find a definition for **{searchTerm}**.\n\nTry:\n• Checking the spelling\n• Using a more common term\n• Asking about a specific technology";
            }
        }
    }
}
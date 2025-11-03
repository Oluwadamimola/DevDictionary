using DevDictionary.Models.A2A;
using DevDictionary.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevDictionary.Controllers
{
    [ApiController]
    [Route("a2a")]
    public class A2AController : ControllerBase
    {
        private readonly IDictionaryService _dictionaryService;

        public A2AController(IDictionaryService dictionaryService)
        {
            _dictionaryService = dictionaryService;
        }

        [HttpPost("agent/devdictionary")]
        public async Task<IActionResult> HandleA2ARequest([FromBody] A2ARequest request)
        {
            Console.WriteLine(" Received A2A request");
            Console.WriteLine($"Method: {request.Method}");
            Console.WriteLine($"ID: {request.Id}");

            try
            {
                // Validate JSON-RPC version
                if (request.JsonRpc != "2.0")
                {
                    return Ok(CreateErrorResponse(request.Id, -32600, "Invalid JSON-RPC version"));
                }

                // Validate method
                if (request.Method != "message/send")
                {
                    return Ok(CreateErrorResponse(request.Id, -32601, $"Method not found: {request.Method}"));
                }

                // Extract user message
                var userMessage = request.Params?.Message?.Parts?.FirstOrDefault()?.Text;
                
                if (string.IsNullOrWhiteSpace(userMessage))
                {
                    return Ok(CreateErrorResponse(request.Id, -32602, "No message text provided"));
                }

                Console.WriteLine($" User message: {userMessage}");

                // Clean and lookup term
                string searchTerm = CleanInputText(userMessage);
                Console.WriteLine($" Searching for: {searchTerm}");

                var result = await _dictionaryService.LookupAsync(searchTerm);

                // Format response
                string responseText = FormatResponse(result, searchTerm);

                // Create A2A response
                var a2aResponse = CreateSuccessResponse(request.Id, responseText, userMessage);

                Console.WriteLine(" Sending A2A response");
                return Ok(a2aResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error: {ex.Message}");
                return Ok(CreateErrorResponse(request.Id, -32603, $"Internal error: {ex.Message}"));
            }
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                status = "healthy",
                service = "DevDictionary A2A Agent",
                protocol = "JSON-RPC 2.0",
                timestamp = DateTime.UtcNow
            });
        }

        private A2AResponse CreateSuccessResponse(string? requestId, string responseText, string originalMessage)
        {
            return new A2AResponse
            {
                JsonRpc = "2.0",
                Id = requestId,
                Result = new A2AResult
                {
                    Message = new A2AResponseMessage
                    {
                        Kind = "message",
                        Role = "assistant",
                        Parts = new List<A2APart>
                        {
                            new A2APart
                            {
                                Kind = "text",
                                Text = responseText
                            }
                        }
                    },
                    Artifacts = new List<object>(),
                    History = new List<A2AHistoryItem>
                    {
                        new A2AHistoryItem
                        {
                            Role = "user",
                            Content = originalMessage
                        },
                        new A2AHistoryItem
                        {
                            Role = "assistant",
                            Content = responseText
                        }
                    },
                    Status = "completed"
                }
            };
        }

        private A2AResponse CreateErrorResponse(string? requestId, int code, string message)
        {
            return new A2AResponse
            {
                JsonRpc = "2.0",
                Id = requestId,
                Error = new A2AError
                {
                    Code = code,
                    Message = message
                }
            };
        }

        private string CleanInputText(string input)
        {
            input = input.ToLower().Trim();

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

            while (input.Contains("  "))
            {
                input = input.Replace("  ", " ");
            }

            return input.Trim();
        }

        private string FormatResponse(Models.DefinitionResult result, string searchTerm)
        {
            if (result.Success && !string.IsNullOrEmpty(result.Definition))
            {
                var definition = result.Definition.Length > 500 
                    ? result.Definition.Substring(0, 497) + "..." 
                    : result.Definition;

                return $@" **{result.Term ?? searchTerm}**

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
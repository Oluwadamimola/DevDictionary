using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace DevDictionary.Models.A2A
{
    // Incoming JSON-RPC 2.0 Request
    public class A2ARequest
    {
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; } = "2.0";

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("method")]
        public string? Method { get; set; }

        [JsonPropertyName("params")]
        public A2AParams? Params { get; set; }
    }

    public class A2AParams
    {
        [JsonPropertyName("message")]
        public A2AMessage? Message { get; set; }

        [JsonPropertyName("configuration")]
        public A2AConfiguration? Configuration { get; set; }
    }

    public class A2AMessage
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = "message";

        [JsonPropertyName("role")]
        public string Role { get; set; } = "user";

        [JsonPropertyName("parts")]
        public List<A2APart>? Parts { get; set; }

        [JsonPropertyName("messageId")]
        public string? MessageId { get; set; }

        [JsonPropertyName("taskId")]
        public string? TaskId { get; set; }
    }

    public class A2APart
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = "text";

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

    public class A2AConfiguration
    {
        [JsonPropertyName("blocking")]
        public bool Blocking { get; set; } = true;
    }

    // Outgoing JSON-RPC 2.0 Response
    public class A2AResponse
    {
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; } = "2.0";

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("result")]
        public A2AResult? Result { get; set; }

        [JsonPropertyName("error")]
        public A2AError? Error { get; set; }
    }

    public class A2AResult
    {
        [JsonPropertyName("message")]
        public A2AResponseMessage Message { get; set; } = new();

        [JsonPropertyName("artifacts")]
        public List<object> Artifacts { get; set; } = new();

        [JsonPropertyName("history")]
        public List<A2AHistoryItem> History { get; set; } = new();

        [JsonPropertyName("status")]
        public string Status { get; set; } = "completed";
    }

    public class A2AResponseMessage
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = "message";

        [JsonPropertyName("role")]
        public string Role { get; set; } = "assistant";

        [JsonPropertyName("parts")]
        public List<A2APart> Parts { get; set; } = new();
    }

    public class A2AHistoryItem
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = "";

        [JsonPropertyName("content")]
        public string Content { get; set; } = "";
    }

    public class A2AError
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = "";

        [JsonPropertyName("data")]
        public object? Data { get; set; }
    }
}
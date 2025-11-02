namespace DevDictionary.Models
{
    public class TelexWebhookEvent
    {
        public string? Type { get; set; }  
        public TelexMessage? Message { get; set; }
        public TelexChannel? Channel { get; set; }
        public TelexUser? User { get; set; }
    }

    public class TelexMessage
    {
        public string? Id { get; set; }
        public string? Text { get; set; }
        public long Timestamp { get; set; }
    }

    public class TelexChannel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class TelexUser
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
    }
}
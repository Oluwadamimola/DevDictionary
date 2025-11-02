namespace DevDictionary.Models
{
    public class DefinitionResult
    {
        public bool Success { get; set; }
        public string? Term { get; set; }
        public string? Definition { get; set; }
        public string? Source { get; set; }
        public string? Url { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
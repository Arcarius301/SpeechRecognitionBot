namespace SpeechRecognitionBot.Models
{
    internal class SpeechResult
    {
        [JsonProperty("result")]
        public List<Result>? Results { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;
    }
}

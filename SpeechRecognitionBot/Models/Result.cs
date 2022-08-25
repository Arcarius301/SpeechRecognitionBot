namespace SpeechRecognitionBot.Models
{
    internal class Result
    {
        [JsonProperty("conf")]
        public float Conf { get; set; }
        [JsonProperty("end")]
        public float End { get; set; }
        [JsonProperty("start")]
        public float Start { get; set; }
        [JsonProperty("word")]
        public string Word { get; set; }
    }
}

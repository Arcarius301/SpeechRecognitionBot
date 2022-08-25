namespace SpeechRecognitionBot.Services.Abstractions
{
    internal interface ISpeechService
    {
        public Task RunAsync();
        public Task<string> Recognize(Stream stream);
    }
}

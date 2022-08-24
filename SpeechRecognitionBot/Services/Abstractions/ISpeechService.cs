namespace SpeechRecognitionBot.Services.Abstractions
{
    internal interface ISpeechService
    {
        public Task RunAsync();
        public Task Recognize(Stream stream);
    }
}

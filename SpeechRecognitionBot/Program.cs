namespace SpeechRecognitionBot
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var start = new Startup();
            await start.Run();
        }
    }
}
namespace SpeechRecognitionBot
{
    internal class Application
    {
        private readonly IDiscordBotService _discordBotService;
        public Application(IDiscordBotService discordBotService)
        {
            _discordBotService = discordBotService;
        }

        public async Task Run()
        {
            var list = new List<Task>();
            list.Add(Task.Run(async () => await _discordBotService.RunAsync()));
            await Task.WhenAll(list);
        }
    }
}

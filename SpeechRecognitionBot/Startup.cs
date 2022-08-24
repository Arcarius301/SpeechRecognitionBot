namespace SpeechRecognitionBot
{
    internal class Startup
    {
        public async Task Run()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();

            var serviceProvider = new ServiceCollection()
                .AddOptions()
                .Configure<DiscordOptions>(config.GetSection("DiscordOptions"))
                .AddSingleton<IDiscordBotService, DiscordBotService>()
                .AddSingleton<ISpeechService, SpeechService>()
                .AddSingleton<Application>()
                .BuildServiceProvider();

            var app = serviceProvider.GetService<Application>();
            await app!.Run();
        }
    }
}

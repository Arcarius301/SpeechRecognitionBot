using Microsoft.Extensions.Options;

namespace SpeechRecognitionBot.Services
{
    internal class DiscordBotService : IDiscordBotService
    {
        private readonly DiscordOptions _discordOptions;
        private readonly DiscordClient _discordClient;

        public DiscordBotService(IOptions<DiscordOptions> options)
        {
            _discordOptions = options.Value;
            _discordClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = options.Value.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildVoiceStates
            });

            var commands = _discordClient.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" }
            });
            _discordClient.UseVoiceNext(new VoiceNextConfiguration()
            {
                EnableIncoming = true
            });

            commands.RegisterCommands<VoiceModule>();
        }

        public async Task RunAsync()
        {
            await _discordClient.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}

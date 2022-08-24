using DSharpPlus;
using DSharpPlus.VoiceNext.EventArgs;
using Emzi0767.Utilities;

namespace SpeechRecognitionBot.Commands
{
    internal class VoiceModule : BaseCommandModule
    {
        private readonly ISpeechService _speechService = new SpeechService();
        private static List<byte> _data = new List<byte>();
        private static long _time = 0;

        [Command("join")]
        public async Task JoinCommand(CommandContext ctx, DiscordChannel? channel = null)
        {
            channel ??= ctx.Member?.VoiceState.Channel;
            if (channel is not null)
            {
                await channel.ConnectAsync();
            }
        }

        [Command("play")]
        public async Task PlayCommand(CommandContext ctx, string path)
        {
            var vnext = ctx.Client.GetVoiceNext();
            var connection = vnext.GetConnection(ctx.Guild);

            var transmit = connection.GetTransmitSink();

            var pcm = ConvertAudioToPcm(path);
            await pcm.CopyToAsync(transmit);
            await pcm.DisposeAsync();
        }

        [Command("leave")]
        public async Task LeaveCommand(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNext();
            var connection = vnext.GetConnection(ctx.Guild);

            connection.Disconnect();
            await Task.CompletedTask;
        }

        [Command("start")]
        public async Task StartCommand(CommandContext ctx, DiscordChannel? channel = null)
        {
            channel ??= ctx.Member?.VoiceState.Channel;
            if (channel is not null)
            {
                var connection = await channel.ConnectAsync();

                Directory.CreateDirectory("Output");
                connection.VoiceReceived += VoiceReceiveHandler;
            }
        }

        [Command("stop")]
        public async Task StopCommand(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNext();

            var connection = vnext.GetConnection(ctx.Guild);
            connection.VoiceReceived -= VoiceReceiveHandler;
            connection.Dispose();

            await WriteAudioAsync(_data);
        }

        private Stream ConvertAudioToPcm(string filePath)
        {
            var ffmpeg = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $@"-i ""{filePath}"" -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            });

            return ffmpeg!.StandardOutput.BaseStream;
        }

        private async Task VoiceReceiveHandler(VoiceNextConnection connection, VoiceReceiveEventArgs args)
        {
            // Console.WriteLine($"[Voice] Duration: {args.AudioDuration} ms. User: {args.User} Format: {args.AudioFormat} SSRC: {args.SSRC}");
            var time = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            ulong id = 385980824639504384;

            if (((time - _time) < 100) && args.User is not null && args.User.Id == id)
            {
                _data.AddRange(args.PcmData.Span.ToArray());
            }
            else if (((time - _time) >= 100) && args.User is not null && args.User.Id == id)
            {
                MemoryStream stream = new MemoryStream(_data.ToArray());
                await _speechService.Recognize(stream);
                // await WriteAudioAsync(_data);
                _data.Clear();
            }

            _time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            //var ffmpeg = Process.Start(new ProcessStartInfo
            //{
            //    FileName = "ffmpeg",
            //    Arguments = $@"-ac 2 -f s16le -ar 48000 -i pipe:0 -ac 2 -ar 44100 Output/{fileName}.wav",
            //    RedirectStandardInput = true
            //});

            //await ffmpeg!.StandardInput.BaseStream.WriteAsync(args.PcmData);
            //ffmpeg.Dispose();
        }

        private async Task WriteAudioAsync(List<byte> data)
        {
            var fileName = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var ffmpeg = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $@"-ac 1 -f s16le -ar 48000 -i pipe:0 -ac 1 -ar 44100 Output/{fileName}.wav",
                RedirectStandardInput = true
            });

            ffmpeg!.StandardInput.BaseStream.Write(data.ToArray());
            ffmpeg.Dispose();
        }
    }
}

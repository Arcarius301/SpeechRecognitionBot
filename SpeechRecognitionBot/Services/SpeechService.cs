using System.Reflection;
using System.Security.Principal;
using Vosk;

namespace SpeechRecognitionBot.Services
{
    internal class SpeechService : ISpeechService
    {
        private readonly Model _model;
        private readonly VoskRecognizer _voskRecognizer;
        public SpeechService()
        {
            _model = new Model("Models/vosk-model-small-ru-0.22");
            Vosk.Vosk.SetLogLevel(0);
            _voskRecognizer = new VoskRecognizer(_model, 48000.0f);
            _voskRecognizer.SetMaxAlternatives(0);
            _voskRecognizer.SetWords(true);
        }

        public async Task RunAsync()
        {
        }

        public async Task<string> Recognize(Stream stream)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (_voskRecognizer.AcceptWaveform(buffer, bytesRead))
                {
                    // Console.WriteLine(rec.Result());
                }
                else
                {
                    // Console.WriteLine(rec.PartialResult());
                }
            }

            var result = JsonConvert.DeserializeObject<SpeechResult>(_voskRecognizer.FinalResult()) !;

            Console.WriteLine(result.Text);
            sw.Stop();

            Console.WriteLine("Elapsed={0}", sw.Elapsed);
            return result.Text;
        }
    }
}

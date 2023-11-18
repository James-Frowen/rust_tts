using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RustTTS
{
    public class TTS : IDisposable
    {
        private IntPtr ptr;
        private bool disposed;

        public TTS()
        {
            ptr = create();
        }
        ~TTS()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (disposed)
                return;

            destroy(ptr);
            disposed = true;
        }

        public void Speak(float speed, string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            speak(ptr, speed, bytes, bytes.Length);
        }

        public bool IsPlaying()
        {
            return is_playing(ptr) == 1;
        }

        public static Task SpeakAsync(float speed, string text)
        {
            return Task.Run(async () =>
            {
                using (var tts = new TTS())
                {
                    tts.Speak(speed, text);

                    while (tts.IsPlaying())
                    {
                        await Task.Delay(100);
                    }
                }
            });
        }

        [DllImport("rust_tts")]
        private static extern IntPtr create();
        [DllImport("rust_tts")]
        private static extern void destroy(IntPtr ptr);
        [DllImport("rust_tts")]
        private static extern uint speak(IntPtr ptr, float speed, byte[] text_ptr, int text_length);
        [DllImport("rust_tts")]
        private static extern uint is_playing(IntPtr ptr);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Audio.OpenAL;

namespace ProjectEvlly.src.Engine.Sound
{
    public class AudioClip : IDisposable
    {
        private int Handler;

        public AudioClip(string path, string filename)
        {
            var soundFile = new WavLoader(path, filename);

            Handler = AL.GenBuffer();
            AL.BufferData(Handler, soundFile._Format, soundFile._Data, soundFile._Data.Length, soundFile._Rate);
        }

        public int GetHandler()
        {
            return Handler;
        }

        public void Dispose()
        {
            AL.DeleteBuffer(Handler);
        }
    }
}

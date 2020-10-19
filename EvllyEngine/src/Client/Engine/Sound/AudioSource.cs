using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvllyEngine;
using OpenTK;
using OpenTK.Audio.OpenAL;

namespace ProjectEvlly.src.Engine.Sound
{
    /// <summary>
    /// AudioSource is for playing a AudioClip, with position
    /// </summary>
    public class AudioSource : IDisposable
    {
        private int Handler;

        public AudioSource(string soundName, bool loop = false, float Gain = 1.0f, float ConeOuterGain = 0.0f, float RolloffFactor = 1.0f)
        {
            Handler = AL.GenSource();
            AL.Source(Handler, ALSourcei.Buffer, AssetsManager.GetSound(soundName));

            AL.Source(Handler, ALSourceb.Looping, loop);
            AL.Source(Handler, ALSourcef.Gain, Gain);
            AL.Source(Handler, ALSourcef.ConeOuterGain, ConeOuterGain);
            AL.Source(Handler, ALSourcef.RolloffFactor, RolloffFactor);

            AL.Source(Handler, ALSource3f.Position, 0.0f, 0.0f, 0.0f);

            Play();
        }

        public AudioSource(Vector3 StartPosition,string soundName, bool loop = false, float Gain = 1.0f, float ConeOuterGain = 0.0f, float RolloffFactor = 1.0f)
        {
            Handler = AL.GenSource();
            AL.Source(Handler, ALSourcei.Buffer, AssetsManager.GetSound(soundName));

            AL.Source(Handler, ALSourceb.Looping, loop);
            AL.Source(Handler, ALSourcef.Gain, Gain);
            AL.Source(Handler, ALSourcef.ConeOuterGain, ConeOuterGain);
            AL.Source(Handler, ALSourcef.RolloffFactor, RolloffFactor);

            AL.Source(Handler, ALSource3f.Position, StartPosition.X, StartPosition.Y, StartPosition.Z);

            Play();
        }

        public void UpdatePosition(Vector3 position)
        {
            AL.Source(Handler, ALSource3f.Position, position.X, position.Y, position.Z);
            //AL.Source(Handler, ALSource3f.Velocity, x, y, z);
        }

        public void Play()
        {
            AL.SourcePlay(Handler);
        }

        public void Pause()
        {
            AL.SourcePause(Handler);
        }

        public void Stop()
        {
            AL.SourceStop(Handler);
        }

        public void Loop(bool isloop)
        {
            AL.Source(Handler, ALSourceb.Looping, isloop);
        }

        public void Volume(float volume)
        {
            AL.Source(Handler, ALSourcef.Gain, volume);
        }

        public void ConeOuterGain(float value)
        {
            AL.Source(Handler, ALSourcef.ConeOuterGain, value);
        }

        public void RolloffFactor(float value)
        {
            AL.Source(Handler, ALSourcef.RolloffFactor, value);
        }

        public void Dispose()
        {
            AL.SourceStop(Handler);
            AL.DeleteSource(Handler);
        }
    }
}

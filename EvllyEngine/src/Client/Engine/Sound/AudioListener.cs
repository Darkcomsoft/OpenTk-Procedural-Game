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
    /// AudioListener is the listener off audio system, Recomended use with Camera
    /// </summary>
    public class AudioListener : IDisposable
    {
        public AudioListener()
        {
            AL.Listener(ALListener3f.Position, 0.0f, 0.0f, 0.0f);
        }

        public AudioListener(Vector3 defaultPosition)
        {
            AL.Listener(ALListener3f.Position, defaultPosition.X, defaultPosition.Y, defaultPosition.Z);
        }

        public void UpdatePosition(Vector3 position)
        {
            AL.Listener(ALListener3f.Position, position.X, position.Y, position.Z);
            //AL.Listener(ALListener3f.Velocity, 1, 1, 1);
        }

        public void Dispose()
        {
           
        }
    }
}

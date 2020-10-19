using EvllyEngine;
using OpenTK.Graphics;
using ProjectEvlly.src.Engine.Render;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.game
{
    public class SkyTime : System.IDisposable
    {
        private Sky _Sky;

        public SkyTime()
        {
            _Sky = new Sky(AssetsManager.GetShader("Sky"));
            Environment.AmbienceColor = Color4.FloralWhite;
            Environment.FogColor = Color4.SkyBlue;
        }

        public void Tick()
        {
            _Sky.Tick();
        }

        public void TickRender()
        {
            _Sky.Draw();
        }

        public void Dispose()
        {
            _Sky.Dispose();
        }
    }

    public struct DataTime
    {
        
    }
}

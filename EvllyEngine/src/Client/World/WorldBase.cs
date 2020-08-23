using EvllyEngine;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.World
{
    public class WorldBase
    {
        public string WorldName;
        public static readonly int Seed;

        public WorldBase()
        {
#if Client
            Game.Client.TickEvent += Tick;
            Game.Client.DrawUpdate += Draw;
            Game.Client.TransparentDrawUpdate += DrawT;
#elif Server
            Server.Tick += Tick;
#endif
        }

        public virtual void Tick()
        {
            
        }

        public virtual void Draw(FrameEventArgs e)
        {

        }

        public virtual void DrawT(FrameEventArgs e)
        {

        }

        public virtual void OnDisposeWorld()
        {

        }

        public void DisposeWorld()
        {
            OnDisposeWorld();
#if Client
            Game.Client.TickEvent -= Tick;
            Game.Client.DrawUpdate -= Draw;
            Game.Client.TransparentDrawUpdate -= DrawT;
#elif Server
            Server.Tick -= Tick;
#endif
        }
    }
}

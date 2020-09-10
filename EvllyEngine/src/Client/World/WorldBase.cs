using EvllyEngine;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.World
{
    public class WorldBase : IDisposable
    {
        public string WorldName;

        public WorldBase()
        {
#if Client
            Game.Client.TickEvent += Tick;
#elif Server
            Server.Tick += Tick;
#endif
        }

        public virtual void Tick()
        {
            
        }

        public virtual void OnDisposeWorld()
        {

        }

        public void Dispose()
        {
            OnDisposeWorld();
#if Client
            Game.Client.TickEvent -= Tick;
#elif Server
            Server.Tick -= Tick;
#endif
        }
    }
}

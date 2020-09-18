using EvllyEngine;
using OpenTK;
using ProjectEvlly.src.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.World
{
    public class WorldBase : ScriptBase
    {
        public string WorldName;

        public WorldBase()
        {
#if Client
            TickSystem.AddTick(this);
#elif Server
            Server.Tick += Tick;
#endif
        }

        public virtual void OnDisposeWorld()
        {

        }

        public override void Dispose()
        {
            OnDisposeWorld();
#if Client
            TickSystem.RemoveTick(this);
#elif Server
            Server.Tick -= Tick;
#endif
        }
    }
}

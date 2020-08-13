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
           
        }

        public virtual void Tick()
        {
            
        }

        public virtual void Draw(FrameEventArgs e)
        {

        }

        public virtual void OnDisposeWorld()
        {

        }

        public void DisposeWorld()
        {
            int c = 0;

            OnDisposeWorld();
        }
    }
}

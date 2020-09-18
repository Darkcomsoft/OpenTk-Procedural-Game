using EvllyEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.Engine.Render
{
    public abstract class RenderEntityBase : IDisposable
    {
        public Transform transform;

        public float ViewBoxWitdh;
        public float ViewBoxHeight;

        public virtual void TickRender(float time)
        {

        }

        public virtual void Dispose()
        {
            transform = null;
        }
    }
}

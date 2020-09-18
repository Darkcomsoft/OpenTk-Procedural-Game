using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.Engine
{
    public abstract class ScriptBase : IDisposable
    {
        public virtual void Tick()
        {

        }

        public virtual void Dispose()
        {

        }
    }
}

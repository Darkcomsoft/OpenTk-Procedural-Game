using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public class GameObject
    {
        public Transform _transform;
        public virtual void OnStart()
        {

        }

        public virtual void OnUpdate()
        {

        }

        /// <summary>
        /// Draw only opaque model
        /// </summary>
        public virtual void OnDrawO()
        {

        }
        /// <summary>
        /// Draw only Transparency model
        /// </summary>
        public virtual void OnDrawT()
        {

        }

        public virtual void OnDestroy()
        {

        }
    }
}

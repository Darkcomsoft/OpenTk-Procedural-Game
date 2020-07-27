using EvllyEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly
{
    public class Entity
    {
        public Transform transform;

        public virtual void OnStart()
        {

        }

        public virtual void OnUpdate()
        {

        }

        /// <summary>
        /// Draw only opaque model
        /// </summary>
        public virtual void OnDrawOpaque()
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

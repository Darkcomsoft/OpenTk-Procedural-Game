using EvllyEngine;
using OpenTK;
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

        public Entity()
        {
            Game.Instance.UpdateFrame += OnUpdate;
            Game.Instance.DrawUpdate += OnDrawOpaque;
            Game.Instance.TransparentDrawUpdate += OnDrawT;

            transform = new Transform();
            OnConstruc();
            OnStart();
        }

        public virtual void OnConstruc()
        {

        }

        public virtual void OnStart()
        {

        }

        public virtual void OnUpdate(object obj, FrameEventArgs e)
        {

        }

        /// <summary>
        /// Draw only opaque model
        /// </summary>
        public virtual void OnDrawOpaque(FrameEventArgs e)
        {

        }
        /// <summary>
        /// Draw only Transparency model
        /// </summary>
        public virtual void OnDrawT(FrameEventArgs e)
        {

        }

        public virtual void OnDestroy()
        {
            Game.Instance.UpdateFrame -= OnUpdate;
            Game.Instance.DrawUpdate -= OnDrawOpaque;
            Game.Instance.TransparentDrawUpdate -= OnDrawT;
        }
    }
}

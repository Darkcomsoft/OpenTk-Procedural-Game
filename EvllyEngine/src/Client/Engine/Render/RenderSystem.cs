using EvllyEngine;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.Engine.Render
{
    public class RenderSystem
    {
        public static RenderSystem Instance;

        private List<RenderEntityBase> renderEntityBases = new List<RenderEntityBase>();
        private List<RenderEntityBase> renderEntityBasesT = new List<RenderEntityBase>();
        private Queue<RenderEntityBase> toRemove = new Queue<RenderEntityBase>();

        public RenderSystem()
        {
            Instance = this;
        }

        public void Tick(float time)
        {
            while (toRemove.Count > 0)
            {
                RenderEntityBase entity = toRemove.Dequeue();
                entity.Destroy();
            }
        }

        public void RenderTick(float time)
        {
            //Frustum.Instance.CalculateFrustum(Camera.Main._projection, Camera.Main.viewMatrix);

            foreach (var item in renderEntityBases)
            {
                /*if (Frustum.Instance.VolumeVsFrustum(item.transform.Position.X, item.transform.Position.Y, item.transform.Position.Z, item.transform.Size.X, item.transform.Size.Y, item.transform.Size.Z))
                {
                    item.TickRender(time);
                }*/

                item.TickRender(time);
            }

            foreach (var item in renderEntityBasesT)
            {
                /*if (Frustum.Instance.VolumeVsFrustum(item.transform.Position.X, item.transform.Position.Y, item.transform.Position.Z, item.transform.Size.X, item.transform.Size.Y, item.transform.Size.Z))
                {
                    item.TickRender(time);
                }*/
                if (Vector3.Distance(item.transform.Position, Camera.Main._transformParent.Position) <= 100)
                {
                    item.TickRender(time);
                }
            }
        }

        public void Dispose()
        {
            foreach (var item in renderEntityBases)
            {
                item.Destroy();
            }

            foreach (var item in renderEntityBasesT)
            {
                item.Destroy();
            }
            renderEntityBases.Clear();
            renderEntityBasesT.Clear();

            renderEntityBases = null;
            renderEntityBasesT = null;
        }

        public static void AddRenderItem(RenderEntityBase entityRender)
        {
            RenderSystem.Instance.renderEntityBases.Add(entityRender);
        }

        public static void RemoveRenderItem(RenderEntityBase entityRender)
        {
            RenderSystem.Instance.renderEntityBases.Remove(entityRender);
            RenderSystem.Instance.toRemove.Enqueue(entityRender);
        }

        public static void AddRenderItemT(RenderEntityBase entityRender)
        {
            RenderSystem.Instance.renderEntityBasesT.Add(entityRender);
        }

        public static void RemoveRenderItemT(RenderEntityBase entityRender)
        {
            RenderSystem.Instance.renderEntityBasesT.Remove(entityRender);
            RenderSystem.Instance.toRemove.Enqueue(entityRender);
        }
    }
}

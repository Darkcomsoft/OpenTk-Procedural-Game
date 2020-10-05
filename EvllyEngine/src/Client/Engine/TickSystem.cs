using EvllyEngine;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectEvlly.src.Engine.Render;

namespace ProjectEvlly.src.Engine
{
    public class TickSystem : IDisposable
    {
        public static TickSystem Instance;

        private List<ScriptBase> TickList = new List<ScriptBase>();

        private List<RenderEntityBase> renderEntityBases = new List<RenderEntityBase>();
        private List<RenderEntityBase> renderEntityBasesT = new List<RenderEntityBase>();
        private Queue<RenderEntityBase> toRemove = new Queue<RenderEntityBase>();

        public TickSystem()
        {
            Instance = this;
        }

        public void Tick(float time)
        {
            for (int i = 0; i < TickList.Count; i++)
            {
                if (TickList[i] != null)
                {
                    TickList[i].Tick();
                }
            }

            while (toRemove.Count > 0)
            {
                RenderEntityBase entity = toRemove.Dequeue();
                if (renderEntityBases.Contains(entity))
                {
                    renderEntityBases.Remove(entity);
                }
                else
                {
                    renderEntityBasesT.Remove(entity);
                }
                entity.Dispose();
            }
        }

        public void RenderTick(float time)
        {
            if (Camera.Main != null)
            {
                Frustum.Instance.CalculateFrustum(Camera.Main._projection, Camera.Main.viewMatrix);

                for (int i = 0; i < renderEntityBases.Count; i++)
                {
                    if (renderEntityBases[i] != null)
                    {
                        /*if (Frustum.Instance.VolumeVsFrustum(renderEntityBases[i].transform.Position.X, renderEntityBases[i].transform.Position.Y, renderEntityBases[i].transform.Position.Z, renderEntityBases[i].ViewBoxWitdh, renderEntityBases[i].ViewBoxHeight, renderEntityBases[i].ViewBoxWitdh))
                        {
                            renderEntityBases[i].TickRender(time);
                        }*/

                        renderEntityBases[i].TickRender(time);
                    }
                }

                for (int i = 0; i < renderEntityBasesT.Count; i++)
                {
                    if (renderEntityBasesT[i] != null)
                    {
                        /*if (Frustum.Instance.VolumeVsFrustum(renderEntityBasesT[i].transform.Position.X, renderEntityBasesT[i].transform.Position.Y, renderEntityBasesT[i].transform.Position.Z, renderEntityBasesT[i].ViewBoxWitdh, renderEntityBasesT[i].ViewBoxHeight, renderEntityBasesT[i].ViewBoxWitdh))
                        {
                            renderEntityBases[i].TickRender(time);
                        }*/

                        if (Vector3.Distance(renderEntityBasesT[i].transform.Position, Camera.Main._transformParent.Position) <= 100)
                        {
                            renderEntityBasesT[i].TickRender(time);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            if (toRemove != null)
            {
                while (toRemove.Count > 0)
                {
                    RenderEntityBase entity = toRemove.Dequeue();

                    if (renderEntityBases.Contains(entity))
                    {
                        renderEntityBases.Remove(entity);
                    }
                    else if (renderEntityBasesT.Contains(entity))
                    {
                        renderEntityBasesT.Remove(entity);
                    }

                    if (entity != null)
                    {
                        entity.Dispose();
                    }
                }

                toRemove.Clear();
            }

            /*foreach (var item in renderEntityBases)
            {
                item.Dispose();
            }

            foreach (var item in renderEntityBasesT)
            {
                item.Dispose();
            }*/

            if (renderEntityBases != null)
            {
                renderEntityBases.Clear();
            }

            if (renderEntityBasesT != null)
            {
                renderEntityBasesT.Clear();
            }

            if (TickList != null)
            {
                TickList.Clear();
            }

            toRemove = null;
            renderEntityBases = null;
            renderEntityBasesT = null;
            TickList = null;
        }

        public static void AddRenderItem(RenderEntityBase entityRender)
        {
            TickSystem.Instance.renderEntityBases.Add(entityRender);
        }

        public static void RemoveRenderItem(RenderEntityBase entityRender)
        {
            //TickSystem.Instance.toRemove.Enqueue(entityRender);
            TickSystem.Instance.renderEntityBases.Remove(entityRender);
        }

        public static void InstaRemoveRenderItem(RenderEntityBase entityRender)
        {
            TickSystem.Instance.renderEntityBases.Remove(entityRender);
        }

        public static void AddRenderItemT(RenderEntityBase entityRender)
        {
            TickSystem.Instance.renderEntityBasesT.Add(entityRender);
        }

        public static void RemoveRenderItemT(RenderEntityBase entityRender)
        {
            //TickSystem.Instance.toRemove.Enqueue(entityRender);
            TickSystem.Instance.renderEntityBasesT.Remove(entityRender);
        }

        public static void InstaRemoveRenderItemT(RenderEntityBase entityRender)
        {
            TickSystem.Instance.renderEntityBasesT.Remove(entityRender);
        }

        public static void AddTick(ScriptBase scriptBase)
        {
            TickSystem.Instance.TickList.Add(scriptBase);
        }

        public static void RemoveTick(ScriptBase scriptBase)
        {
            TickSystem.Instance.TickList.Remove(scriptBase);
        }
    }
}

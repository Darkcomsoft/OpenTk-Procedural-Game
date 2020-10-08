using EvllyEngine;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectEvlly.src.Engine.Render;
using ProjectEvlly.src.World;

namespace ProjectEvlly.src.Engine
{
    public class TickSystem : IDisposable
    {
        public static TickSystem Instance;

        private List<ScriptBase> TickList = new List<ScriptBase>();
        private List<RenderEntityBase> renderEntityBases = new List<RenderEntityBase>();
        private List<RenderEntityBase> renderEntityBasesT = new List<RenderEntityBase>();

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
                        if (Frustum.Instance.VolumeVsFrustum(renderEntityBases[i].transform.Position.X, renderEntityBases[i].transform.Position.Y, renderEntityBases[i].transform.Position.Z, renderEntityBases[i].ViewBoxWitdh, renderEntityBases[i].ViewBoxHeight, renderEntityBases[i].ViewBoxWitdh))
                        {
                            renderEntityBases[i].TickRender(time);
                        }
                    }
                }

                for (int i = 0; i < renderEntityBasesT.Count; i++)
                {
                    if (renderEntityBasesT[i] != null)
                    {
                        if (Frustum.Instance.VolumeVsFrustum(renderEntityBasesT[i].transform.Position.X, renderEntityBasesT[i].transform.Position.Y, renderEntityBasesT[i].transform.Position.Z, renderEntityBasesT[i].ViewBoxWitdh, renderEntityBasesT[i].ViewBoxHeight, renderEntityBasesT[i].ViewBoxWitdh))
                        {
                            if (renderEntityBasesT[i].GetType() != typeof(WaterMeshRender))
                            {
                                if (Vector3.Distance(renderEntityBasesT[i].transform.Position, Camera.Main._transformParent.Position) <= 30)
                                {
                                    renderEntityBasesT[i].TickRender(time);
                                }
                            }
                            else
                            {
                                if (Vector3.Distance(renderEntityBasesT[i].transform.Position, Camera.Main._transformParent.Position) <= 100)
                                {
                                    renderEntityBasesT[i].TickRender(time);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
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
            TickSystem.Instance.renderEntityBases.Remove(entityRender);
        }

        public static void AddRenderItemT(RenderEntityBase entityRender)
        {
            TickSystem.Instance.renderEntityBasesT.Add(entityRender);
        }

        public static void RemoveRenderItemT(RenderEntityBase entityRender)
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

using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectEvlly;
using ProjectEvlly.src;

namespace EvllyEngine
{
    public class Dimension
    {
        public string _DimensionID;

        private List<Entity> Entitys = new List<Entity>();
        private Queue<Entity> ToUnloadEntitys = new Queue<Entity>();

        public Dimension()
        {
            Debug.Log("Start Dimensionnnnnn");
            Game.Game.TickEvent += OnUpdate;
            Game.Game.DrawUpdate += Draw;
        }

        public virtual void OnUpdate()
        {
            while (ToUnloadEntitys.Count > 0)
            {
                Entity entity = ToUnloadEntitys.Dequeue();

                entity.OnDestroy();
                entity = null;
            }
        }

        public virtual void Draw(FrameEventArgs e)
        {
           
        }

        public virtual void OnUnloadDimension()
        {

        }

        public void AddEntity(Entity entity)
        {
            Entitys.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            if (!ToUnloadEntitys.Contains(entity))
            {
                ToUnloadEntitys.Enqueue(entity);
                Entitys.Remove(entity);
            }
        }

        public void UnloadDimension()
        {
            int c = 0;

            OnUnloadDimension();

            for (int i = 0; i < Entitys.Count; i++)
            {
                Entitys[i].OnDestroy();
                Entitys[i] = null;
                c++;
            }

            while (ToUnloadEntitys.Count > 0)
            {
                Entity entity = ToUnloadEntitys.Dequeue();

                entity.OnDestroy();
                entity = null;
            }

            Entitys.Clear();
            ToUnloadEntitys.Clear();

            Entitys = null;
            ToUnloadEntitys = null;

            Game.Game.TickEvent -= OnUpdate;
            Game.Game.DrawUpdate -= Draw;

            ///Garbage Collector
            Debug.Log("Destroyed : " + c + " GameObject's");
            Debug.Log("Unload Dimension: " + _DimensionID);
            long last = GC.GetTotalMemory(false);
            GC.Collect();
            Debug.Log("GC: Collected, Cleared: " + last + " (BYTES)");
        }
    }
}
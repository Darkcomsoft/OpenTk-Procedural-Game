using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectEvlly;

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
            Engine.Instance.UpdateFrame += OnUpdate;
            //Engine.Instance.DrawUpdate += Draw;
        }

        public virtual void OnUpdate(object obj, FrameEventArgs e)
        {
            while (ToUnloadEntitys.Count > 0)
            {
                Entity entity = ToUnloadEntitys.Dequeue();

                entity.OnDestroy();
                entity = null;
            }
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

        public void OnUnloadDimension()
        {
            int c = 0;

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

            Engine.Instance.UpdateFrame -= OnUpdate;
            //Engine.Instance.DrawUpdate -= Draw;

            ///Garbage Collector
            Debug.Log("Destroyed : " + c + " GameObject's");
            Debug.Log("Unload Dimension: " + _DimensionID);
            long last = GC.GetTotalMemory(false);
            GC.Collect();
            Debug.Log("GC: Collected, Cleared: " + last + " (BYTES)");
        }
    }
}
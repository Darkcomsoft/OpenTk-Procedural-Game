using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.World
{
    /// <summary>
    /// This class is for manage the worlds, it can be used by the server and client
    /// </summary>
    public class WorldManager : IDisposable
    {
        public Dictionary<int, WorldBase> CurrentWorlds;

        public WorldManager()
        {
            CurrentWorlds = new Dictionary<int, WorldBase>();
        }

        public void Dispose()
        {
            foreach (var item in CurrentWorlds)
            {
                item.Value.Dispose();
            }

            CurrentWorlds.Clear();
        }

        public void LoadWorld(WorldBase world)
        {
            CurrentWorlds.Add(world.GetType().GetHashCode(), world);
        }

        public void UnLoadWorld(WorldBase world)
        {
            CurrentWorlds.Remove(world.GetType().GetHashCode());
            world.Dispose();
        }

        public void UnloadAll()
        {
            foreach (var item in CurrentWorlds)
            {
                item.Value.Dispose();
            }

            CurrentWorlds.Clear();
        }
    }
}

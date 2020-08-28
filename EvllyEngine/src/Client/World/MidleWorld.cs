using OpenTK;
using ProjectEvlly;
using ProjectEvlly.src;
using ProjectEvlly.src.Net;
using ProjectEvlly.src.save;
using ProjectEvlly.src.UI;
using ProjectEvlly.src.Utility;
using ProjectEvlly.src.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public class MidleWorld : WorldBase
    {
        public static MidleWorld instance;

        public static int ChunkSize = 16;
        public int renderDistance = 50;
        public bool WorldRuning { get; private set; }
        public Vector3 PlayerPos;

        private Dictionary<Vector3, Chunk> chunkMap = new Dictionary<Vector3, Chunk>();

        public static FastNoise globalNoise;

#if Client
#elif Server
#endif

        public MidleWorld(string _worldName)
        {
            instance = this;

#if Client
            
#elif Server

#endif

            WorldName = _worldName;

            globalNoise = new FastNoise(0);
            globalNoise.SetFrequency(0.005f);

            //LoadTheWorld if has a save
            if (SaveManager.LoadWorld())//Have a Save
            {

            }
            else//Dont have a save
            {
                
            }
        }

    public void SpawnPlayer(CharSaveInfo charSaveInfo)
        {
#if Client
            if (Network.IsServer)//is singleplayer
            {
                Network.SpawnEntity(new PlayerEntity());
            }
            else
            {
                Network.SpawnEntity(new PlayerEntity());
            }
#endif
        }

        public override void Tick()
        {
#if Client
#elif Server
#endif
            CheckViewDistance();
            base.Tick();
        }

        public override void Draw()
        {
            /*foreach (var item in chunkMap)
            {
                item.Value.Draw(e);
            }*/
            base.Draw();
        }

        public Vector2 GetChunkCoordFromVector3(Vector3 pos)
        {
            int x = (int)Mathf.FloorToInt(pos.X / ChunkSize);
            int z = (int)Mathf.FloorToInt(pos.Z / ChunkSize);
            return new Vector2(x, z);
        }

        Queue<Vector3> ToRemove = new Queue<Vector3>();

        public void CheckViewDistance()
        {
            Vector3 PlayerP = new Vector3((int)(Mathf.Round(PlayerPos.X / ChunkSize) * ChunkSize), (int)(Mathf.Round(PlayerPos.Y / ChunkSize) * ChunkSize), (int)(Mathf.Round(PlayerPos.Z / ChunkSize) * ChunkSize));
            int minX = (int)PlayerP.X - renderDistance;
            int maxX = (int)PlayerP.X + renderDistance;

            int minY = (int)PlayerP.Y - renderDistance;
            int maxY = (int)PlayerP.Y + renderDistance;

            int minZ = (int)PlayerP.Z - renderDistance;
            int maxZ = (int)PlayerP.Z + renderDistance;

            while (ToRemove.Count > 0)
            {
                Vector3 vec = ToRemove.Dequeue();
                chunkMap.Remove(vec);
            }

            foreach (var item in chunkMap)
            {
                if (item.Value.transform.Position.X > maxX || item.Value.transform.Position.X < minX || item.Value.transform.Position.Y > maxY || item.Value.transform.Position.Y < minY || item.Value.transform.Position.Z > maxZ || item.Value.transform.Position.Z < minZ)
                {
                    if (chunkMap.ContainsKey(item.Value.transform.Position))
                    {
                        chunkMap[item.Value.transform.Position].OnDestroy();

                        ToRemove.Enqueue(item.Value.transform.Position);
                    }
                }
            }

            for (int x = minX; x < maxX; x += ChunkSize)
            {
                for (int y = minY; y < maxY; y += ChunkSize)
                {
                    for (int z = minZ; z < maxZ; z += ChunkSize)
                    {
                        Vector3 vector = new Vector3(x, y, z);

                        if (!chunkMap.ContainsKey(vector))
                        {
                            chunkMap.Add(vector, new Chunk(vector));
                        }
                    }
                }
            }
        }

        public override void OnDisposeWorld()
        {
            foreach (var item in chunkMap.Values)
            {
                chunkMap[item.transform.Position].OnDestroy();
            }

            chunkMap.Clear();
            ToRemove.Clear();

            base.OnDisposeWorld();
        }


        public Block GetTileAt(int x, int z)
        {
            Chunk chunk = GetChunkAt(x, z);

            if (chunk != null)
            {
                return chunk.Blocks[x - (int)chunk.transform.Position.X, z - (int)chunk.transform.Position.Z];
            }
            return new Block();
        }

        public Block GetTileAt(Vector3 pos)
        {
            Chunk chunk = GetChunkAt((int)pos.X, (int)pos.Z);

            if (chunk != null)
            {
                lock (chunk.Blocks)
                    return chunk.Blocks[(int)pos.X - (int)chunk.transform.Position.X, (int)pos.Z - (int)chunk.transform.Position.Z];
            }
            return new Block();
        }

        public Block GetTileAt(float x, float z)
        {
            int mx = (int)Mathf.FloorToInt(x);
            int mz = (int)Mathf.FloorToInt(z);

            Chunk chunk = GetChunkAt(mx, mz);

            if (chunk != null)
            {
                return chunk.Blocks[mx - (int)chunk.transform.Position.X, mz - (int)chunk.transform.Position.Z];
            }
            return new Block();
        }

        public Chunk GetChunkAt(int xx, int zz)
        {
            Vector3 chunkpos = new Vector3(Mathf.FloorToInt(xx / ChunkSize) * ChunkSize, 0, Mathf.FloorToInt(zz / ChunkSize) * ChunkSize);
            if (chunkMap.ContainsKey(chunkpos))
            {
                return chunkMap[chunkpos];
            }
            else
            {
                return null;
            }
        }
    }
}

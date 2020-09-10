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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EvllyEngine
{
    public class MidleWorld : WorldBase
    {
        public static int ChunkSize = 10;
        public int renderDistanceXZ = 100;
        public int renderDistanceY = 30;
        public bool WorldRuning { get; private set; }
        private bool CanDestroyWorld = false;
        public Vector3 PlayerPos;

        private object LockChunkMap;
        private object LockToUpdate;

        private Dictionary<Vector3, Chunk> chunkMap = new Dictionary<Vector3, Chunk>();
        private Queue<Vector3> ToRemove = new Queue<Vector3>();
        private Queue<Vector3> ToUpdate = new Queue<Vector3>();

        private Thread WorldGeneratorThread;
        private int ThreadSleepTime = 60;

        public static FastNoise globalNoise;
        public static FastNoise biomeNoise;

#if Client
#elif Server
#endif

        public MidleWorld(string _worldName)
        {
#if Client
            Game.MidleWorld = this;
#elif Server

#endif
            LockChunkMap = new object();
            LockToUpdate = new object();

            WorldName = _worldName;

            globalNoise = new FastNoise(GlobalData.Seed);
            globalNoise.SetFrequency(0.005f);

            biomeNoise = new FastNoise(GlobalData.Seed);
            biomeNoise.SetFrequency(0.05f);
            biomeNoise.SetGradientPerturbAmp(30f);
            biomeNoise.SetCellularNoiseLookup(new FastNoise());
            biomeNoise.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Manhattan);
            biomeNoise.SetCellularReturnType(FastNoise.CellularReturnType.NoiseLookup);

            //LoadTheWorld if has a save
            if (SaveManager.LoadWorld())//Have a Save
            {

            }
            else//Dont have a save
            {
                
            }

            WorldRuning = true;
            WorldGeneratorThread = new Thread(new ThreadStart(WorldLooping));
            WorldGeneratorThread.Name = "WorldGeneratorLoop";
            WorldGeneratorThread.Start();
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
            while (ToRemove.Count > 0)
            {
                lock (LockChunkMap)
                {
                    Vector3 vec = ToRemove.Dequeue();
                    if (chunkMap.ContainsKey(vec))
                    {
                        chunkMap[vec].Dispose();
                        chunkMap.Remove(vec);
                    }
                }
            }
            base.Tick();
        }

        private void WorldLooping()//this is a other thread looping
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);

            while (WorldRuning)
            {
                try
                {
                    CheckViewDistance();
                    Thread.Sleep(ThreadSleepTime);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error On WorldLoop: (" + ex.Message + ") --- (StackTrace: " + ex.StackTrace + ")");
                }
            }

            CanDestroyWorld = true;
        }

        static void MyHandler(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                Debug.LogError("Unhadled domain exception:\n\n" + ex.Message);
            }
            catch (Exception exc)
            {
                try
                {
                    Debug.LogError("Fatal exception happend inside UnhadledExceptionHandler: \n\n" + exc.Message);
                }
                finally
                {
                    Environment.Exit(1);
                }
            }
        }

        public void CheckViewDistance()
        {
            Vector3 PlayerP = new Vector3((int)(Mathf.Round(PlayerPos.X / ChunkSize) * ChunkSize), (int)(Mathf.Round(PlayerPos.Y / ChunkSize) * ChunkSize), (int)(Mathf.Round(PlayerPos.Z / ChunkSize) * ChunkSize));
            int minX = (int)PlayerP.X - renderDistanceXZ;
            int maxX = (int)PlayerP.X + renderDistanceXZ;

            /*int minY = (int)PlayerP.Y - renderDistanceY;
            int maxY = (int)PlayerP.Y + renderDistanceY;*/

            int minZ = (int)PlayerP.Z - renderDistanceXZ;
            int maxZ = (int)PlayerP.Z + renderDistanceXZ;

            lock (LockToUpdate)
            {
                while (ToUpdate.Count > 0)
                {
                    lock (LockChunkMap)
                    {
                        Vector3 chunk = ToUpdate.Dequeue();
                        if (chunkMap.ContainsKey(chunk))
                        {
                            chunkMap[chunk].UpdateMesh();
                        }
                    }
                }
            }

            lock (LockChunkMap)
            {
                foreach (var item in chunkMap)
                {
                    if (item.Value.transform.Position.X > maxX || item.Value.transform.Position.X < minX || item.Value.transform.Position.Z > maxZ || item.Value.transform.Position.Z < minZ)
                    {
                        if (chunkMap.ContainsKey(item.Value.transform.Position))
                        {
                            ToRemove.Enqueue(item.Value.transform.Position);
                        }
                    }
                }
            }

            for (int x = minX; x < maxX; x += ChunkSize)
            {
                for (int z = minZ; z < maxZ; z += ChunkSize)
                {
                    Vector3 vector = new Vector3(x, 0, z);
                    lock (LockChunkMap)
                    {
                        if (!chunkMap.ContainsKey(vector))
                        {
                            chunkMap.Add(vector, new Chunk(vector));
                        }
                        else
                        {
                            /*switch (chunkMap[vector].GetStatus)
                            {
                                case ChunkState.noload:
                                    break;
                                case ChunkState.nogameLogic:
                                    break;
                                case ChunkState.noEntity:
                                    break;
                                case ChunkState.AllGameLogic:
                                    break;
                                default:
                                    break;
                            }*/
                        }
                    }
                }
            }
        }

        public override void OnDisposeWorld()
        {
            WorldRuning = false;

            Debug.Log("Wait for the world loop stop....");
            while (CanDestroyWorld != true) { }//wait to other thread loop finishe and destroy the thread
            Debug.Log("Yayyyyyy we are free to go (:");

            foreach (var item in chunkMap.Values)
            {
                chunkMap[item.transform.Position].Dispose();
            }

            chunkMap.Clear();
            ToRemove.Clear();

            base.OnDisposeWorld();
        }

        public void UpDateChunk(Vector3 chunk)
        {
            ToUpdate.Enqueue(chunk);
        }

        public Vector2 GetChunkCoordFromVector3(Vector3 pos)
        {
            int x = (int)Mathf.FloorToInt(pos.X / ChunkSize);
            int z = (int)Mathf.FloorToInt(pos.Z / ChunkSize);
            return new Vector2(x, z);
        }

        public Block GetTileAt(int x, int z)
        {
            Chunk chunk = GetChunkAt(x, z);
            
            if (chunk != null)
            {
                return chunk.GetBlocksMap[x - (int)chunk.transform.Position.X, z - (int)chunk.transform.Position.Z];
            }
            return new Block();
        }

        public Block GetTileAt(Vector3 pos)
        {
            Chunk chunk = GetChunkAt((int)pos.X, (int)pos.Z);

            if (chunk != null)
            {
                return chunk.GetBlocksMap[(int)pos.X - (int)chunk.transform.Position.X, (int)pos.Z - (int)chunk.transform.Position.Z];
            }
            return new Block();
        }

        public Block GetTileAt(float x, float z)
        {
            int mx = Mathf.FloorToInt(x);
            int mz = Mathf.FloorToInt(z);

            Chunk chunk = GetChunkAt(mx,  mz);

            if (chunk != null)
            {
                return chunk.GetBlocksMap[mx - (int)chunk.transform.Position.X, mz - (int)chunk.transform.Position.Z];
            }
            return new Block();
        }

        public Chunk GetChunkAt(int xx, int zz)
        {
            lock (LockChunkMap)
            {
                Vector3 chunkpos = new Vector3(Mathf.FloorToInt(xx / (float)ChunkSize) * ChunkSize, 0, Mathf.FloorToInt(zz/ (float)ChunkSize) * ChunkSize);

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
}

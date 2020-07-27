using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public class World : ScriptBase
    {
        public static World instance;
        public static int ChunkSize = 16;
        public int renderDistance = 50;
        public bool WorldRuning { get; private set; }
        public Vector3 PlayerPos { get; private set; }

        private Dictionary<Vector3, Chunk> chunkMap = new Dictionary<Vector3, Chunk>();

        public Shader Shader;
        public static FastNoise globalNoise;

        public override void Start()
        {
            instance = this;

            globalNoise = new FastNoise(0);
            globalNoise.SetFrequency(0.005f);

            Shader = new Shader(AssetsManager.instance.GetShader("Default"));
            Shader.AddTexture(new Texture(AssetsManager.instance.GetTexture("TileAtlas", "png")));
            /*GameObject obj = GameObject.Instantiate(new Vector3(50, 0,50), Quaternion.Identity, 1);
            //obj.name = "Chunk - X:" + vector.x + " Y:" + vector.y + " Z:" + vector.z;

            Chunk chunkScript = new Chunk();
            obj.AddScript(chunkScript);

            chunkScript.StartUpChunk();*/

            base.Start();
        }

        public override void Update()
        {
            PlayerPos = Camera.Main.gameObject._transform.Position;
            CheckViewDistance();
            base.Update();
        }

        public Vector2 GetChunkCoordFromVector3(Vector3 pos)
        {

            int x = (int)FloorToInt(pos.X / ChunkSize);
            int z = (int)FloorToInt(pos.Z / ChunkSize);
            return new Vector2(x, z);

        }
        public static int FloorToInt(float value) => (int)Math.Floor(value);
        public static float Round(float value) => (float)Math.Round(value);

        List<Vector3> ToRemove = new List<Vector3>();

        public void CheckViewDistance()
        {
            Vector3 PlayerP = new Vector3((int)(Round(PlayerPos.X / ChunkSize) * ChunkSize), 0, (int)(Round(PlayerPos.Z / ChunkSize) * ChunkSize));
            int minX = (int)PlayerP.X - renderDistance;
            int maxX = (int)PlayerP.X + renderDistance;

            int minZ = (int)PlayerP.Z - renderDistance;
            int maxZ = (int)PlayerP.Z + renderDistance;

            for (int z = minZ; z < maxZ; z += ChunkSize)
            {
                for (int x = minX; x < maxX; x += ChunkSize)
                {
                    Vector3 vector = new Vector3(x, 0, z);

                    if (!chunkMap.ContainsKey(vector))
                    {
                        GameObject obj = GameObject.Instantiate(new Vector3(vector.X, 0, vector.Z), Quaternion.Identity, 1);
                        //obj.name = "Chunk - X:" + vector.x + " Y:" + vector.y + " Z:" + vector.z;

                        Chunk chunkScript = new Chunk();
                        obj.AddScript(chunkScript);

                        chunkScript.StartUpChunk();

                        lock (chunkMap)
                        {
                            chunkMap.Add(vector, chunkScript);
                        }
                    }
                }
            }

            lock (chunkMap)
            {
                foreach (var item in chunkMap.Values)
                {
                    if (item.chunkPosition.X > maxX || item.chunkPosition.X < minX || item.chunkPosition.Z > maxZ || item.chunkPosition.Z < minZ)
                    {
                        if (chunkMap.ContainsKey(item.chunkPosition))
                        {
                            //chunkMap[item.chunkPosition].ClearChunk();
                            GameObject.Destroy(chunkMap[item.chunkPosition].gameObject);
                            ToRemove.Add(item.chunkPosition);
                        }
                    }
                }
            }

            for (int i = 0; i < ToRemove.Count; i++)
            {
                chunkMap.Remove(ToRemove[i]);
                ToRemove.Remove(ToRemove[i]);
            }
        }

        public override void OnDestroy()
        {
            foreach (var item in chunkMap.Values)
            {
                GameObject.Destroy(chunkMap[item.chunkPosition].gameObject);
            }
            chunkMap.Clear();
            ToRemove.Clear();

            Shader.Delete();

            base.OnDestroy();
        }

        public Block GetTileAt(int x, int z)
        {
            Chunk chunk = GetChunkAt(x, z);

            if (chunk != null)
            {

                return chunk.Blocks[x - (int)chunk.chunkPosition.X, z - (int)chunk.chunkPosition.Z];
            }
            return new Block();
        }

        public Block GetTileAt(Vector3 pos)
        {
            Chunk chunk = GetChunkAt((int)pos.X, (int)pos.Z);

            if (chunk != null)
            {
                lock (chunk.Blocks)
                    return chunk.Blocks[(int)pos.X - (int)chunk.chunkPosition.X, (int)pos.Z - (int)chunk.chunkPosition.Z];
            }
            return new Block();
        }

        public Block GetTileAt(float x, float z)
        {
            int mx = (int)Math.Floor(x);
            int mz = (int)Math.Floor(z);

            Chunk chunk = GetChunkAt(mx, mz);

            if (chunk != null)
            {
                return chunk.Blocks[mx - (int)chunk.chunkPosition.X, mz - (int)chunk.chunkPosition.Z];
            }
            return new Block();
        }

        public Chunk GetChunkAt(int xx, int zz)
        {
            Vector3 chunkpos = new Vector3((int)Math.Floor(xx / (float)ChunkSize) * ChunkSize, 0, (int)Math.Floor(zz / (float)ChunkSize) * ChunkSize);
            lock (chunkMap)
            {
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

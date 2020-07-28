﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public class World
    {
        public static World instance;
        public static int ChunkSize = 16;
        public int renderDistance = 50;
        public bool WorldRuning { get; private set; }
        public Vector3 PlayerPos;

        private Dictionary<Vector3, Chunk> chunkMap = new Dictionary<Vector3, Chunk>();

        public Shader Shader;
        public static FastNoise globalNoise;

        public World()
        {
            instance = this;

            globalNoise = new FastNoise(0);
            globalNoise.SetFrequency(0.005f);

            Shader = new Shader(AssetsManager.instance.GetShader("Default"));
            Shader.AddTexture(new Texture(AssetsManager.instance.GetTexture("TileAtlas", "png")));
        }

        public void UpdateWorld()
        {
            CheckViewDistance();
        }

        public void Draw(FrameEventArgs e)
        {
            /*foreach (var item in chunkMap)
            {
                item.Value.Draw(e);
            }*/
        }

        public Vector2 GetChunkCoordFromVector3(Vector3 pos)
        {
            int x = (int)FloorToInt(pos.X / ChunkSize);
            int z = (int)FloorToInt(pos.Z / ChunkSize);
            return new Vector2(x, z);
        }

        public static int FloorToInt(float value) => (int)Math.Floor(value);
        public static float Round(float value) => (float)Math.Round(value);

        Queue<Vector3> ToRemove = new Queue<Vector3>();

        public void CheckViewDistance()
        {
            Vector3 PlayerP = new Vector3((int)(Round(PlayerPos.X / ChunkSize) * ChunkSize), 0, (int)(Round(PlayerPos.Z / ChunkSize) * ChunkSize));
            int minX = (int)PlayerP.X - renderDistance;
            int maxX = (int)PlayerP.X + renderDistance;

            int minZ = (int)PlayerP.Z - renderDistance;
            int maxZ = (int)PlayerP.Z + renderDistance;

            while (ToRemove.Count > 0)
            {
                Vector3 vec = ToRemove.Dequeue();
                chunkMap.Remove(vec);
            }

            lock (chunkMap)
            {
                foreach (var item in chunkMap)
                {
                    if (item.Value.transform.Position.X > maxX || item.Value.transform.Position.X < minX || item.Value.transform.Position.Z > maxZ || item.Value.transform.Position.Z < minZ)
                    {
                        if (chunkMap.ContainsKey(item.Value.transform.Position))
                        {
                            chunkMap[item.Value.transform.Position].OnDestroy();

                            ToRemove.Enqueue(item.Value.transform.Position);
                        }
                    }
                }
            }

            for (int z = minZ; z < maxZ; z += ChunkSize)
            {
                for (int x = minX; x < maxX; x += ChunkSize)
                {
                    Vector3 vector = new Vector3(x, 0, z);

                    if (!chunkMap.ContainsKey(vector))
                    {
                        lock (chunkMap)
                        {
                            chunkMap.Add(vector, new Chunk(vector));
                        }
                    }
                }
            }
        }

        public void OnDestroy()
        {
            foreach (var item in chunkMap.Values)
            {
                chunkMap[item.transform.Position].OnDestroy();
            }

            chunkMap.Clear();
            ToRemove.Clear();

            Shader.Delete();
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
            int mx = (int)Math.Floor(x);
            int mz = (int)Math.Floor(z);

            Chunk chunk = GetChunkAt(mx, mz);

            if (chunk != null)
            {
                return chunk.Blocks[mx - (int)chunk.transform.Position.X, mz - (int)chunk.transform.Position.Z];
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

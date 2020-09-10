using EvllyEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.World.Biomes
{
    public static class OakForest
    {
        public static BiomeData GetBiome(int x, int z, Chunk chunk)
        {
            FastNoise noiseFast = new FastNoise(GlobalData.Seed);
            noiseFast.SetFrequency(0.005f);

            float noise = noiseFast.GetPerlin(x, z);
            System.Random rand = new Random((int)chunk.GetSeed * chunk.GetHashCode() * x - z);

            if (noise <= 0.15f)
            {
                return new BiomeData(TypeBlock.Grass, BlockVariant.none, TreeType.none);
            }
            else if (noise > 0.15f && noise < 0.2f)
            {
                return new BiomeData(TypeBlock.Grass, BlockVariant.none, TreeType.none);
            }
            else
            {
                if (rand.Next(0, 20) == 1)
                {
                    return new BiomeData(TypeBlock.Grass, BlockVariant.none, TreeType.Oak);
                }
                else
                {
                    return new BiomeData(TypeBlock.Grass, BlockVariant.none, TreeType.none);
                }
            }
        }
    }
}
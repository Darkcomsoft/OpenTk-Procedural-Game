using EvllyEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.World.Biomes
{
    public static class SnowForest
    {
        public static BiomeData GetBiome(int x, int z, Chunk chunk)
        {
            FastNoise noiseFast2 = new FastNoise(GlobalData.Seed);
            System.Random rand = new Random((int)chunk.GetSeed * chunk.GetHashCode() * x - z);

            noiseFast2.SetFrequency(0.05f);

            float noise = noiseFast2.GetPerlinFractal(x, z);

            if (rand.Next(0, 100) <= 5)
            {
                return new BiomeData(noise, TypeBlock.Snow, BlockVariant.none, TreeType.PineSnow);
            }
            else
            {
                return new BiomeData(noise, TypeBlock.Snow, BlockVariant.none, TreeType.none);
            }
        }
    }
}
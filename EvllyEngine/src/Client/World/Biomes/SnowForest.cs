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
            FastNoise noiseFast = new FastNoise(GlobalData.Seed);
            FastNoise noiseFast2 = new FastNoise(GlobalData.Seed);
           System.Random rand = new Random(GlobalData.Seed - (int)chunk.GetSeed * chunk.GetHashCode() * x + z - chunk.transform.Position.GetHashCode());

            noiseFast.SetFrequency(0.009f);
            noiseFast2.SetFrequency(0.009f);

            float noise =  noiseFast.GetPerlinFractal(x, z) * 100;

            if (rand.Next(0, 100) <= 5)
            {
                return new BiomeData(noise * noiseFast2.GetPerlinFractal(x, z), TypeBlock.Snow, BlockVariant.none, TreeType.PineSnow);
            }
            else
            {
                return new BiomeData(noise * noiseFast2.GetPerlinFractal(x, z), TypeBlock.Snow, BlockVariant.none, TreeType.none);
            }
        }
    }
}
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
            FastNoise noiseFast2 = new FastNoise(GlobalData.Seed);
            System.Random rand = new Random((int)chunk.GetSeed * chunk.GetHashCode() * x - z);

            noiseFast2.SetFrequency(0.05f);

            float noise = noiseFast2.GetPerlinFractal(x, z);

            if (noise <= 0.15f)
            {
                return new BiomeData(noise, TypeBlock.Grass, BlockVariant.none, TreeType.none);
            }
            else if (noise > 0.15f && noise < 0.2f)
            {
                return new BiomeData(noise, TypeBlock.Grass, BlockVariant.none, TreeType.none);
            }
            else
            {
                if (rand.Next(0, 20) <= 5)
                {
                    return new BiomeData(noise, TypeBlock.Grass, BlockVariant.none, TreeType.Oak);
                }
                else
                {
                    return new BiomeData(noise, TypeBlock.Grass, BlockVariant.none, TreeType.none);
                }
            }
        }
    }
}
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
        public static BiomeData GetBiome(int x, int z)
        {
            float noise = new FastNoise(GlobalData.Seed).GetPerlin(x, z);
            System.Random rand = new Random(x * z * z + z / x + x);

            if (noise <= 0.15f)
            {
                return new BiomeData(TypeBlock.Dirt, BlockVariant.none, TreeType.none);
            }
            else if (noise > 0.15f && noise < 0.2f)
            {
                return new BiomeData(TypeBlock.Dirt, BlockVariant.none, TreeType.none);
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
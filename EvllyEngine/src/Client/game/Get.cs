using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.game
{
    public static class Get
    {
        public static Color4 TileColors(BiomeType biome)
        {
            switch (biome)
            {
                case BiomeType.Bench:
                    return new Color4(0, 0, 0, 0);
                case BiomeType.Desert:
                    return new Color4(0, 0, 0, 0);
                case BiomeType.Savanna:
                    return new Color4(0, 0, 0, 0);
                case BiomeType.TropicalRainforest:
                    return new Color4(70, 175, 66, 255);
                case BiomeType.Grassland:
                    return new Color4(70, 175, 66, 255);
                case BiomeType.Woodland:
                    return new Color4(70, 175, 66, 255);
                case BiomeType.SeasonalForest:
                    return new Color4(70, 175, 66, 255);
                case BiomeType.TemperateRainforest:
                    return new Color4(70, 175, 66, 255);
                case BiomeType.BorealForest:
                    return new Color4(70, 175, 66, 255);
                case BiomeType.Tundra:
                    return new Color4(226, 118, 68, 255);
                case BiomeType.Ice:
                    return new Color4(0, 0, 0, 0);
                default:
                    return new Color4(0, 0, 0, 0);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src
{
    /// <summary>
    /// this is for setting global data like the SEED of the world generator, and other things like version of the game 
    /// </summary>
    public static class GlobalData
    {
        public const string AppName = "ProjectEvlly";
        public const string Version = "InDev V:0.0.0";

        public static int Seed;

        #region BiomesValues
        //Temperatura
        public static float ColdestValue = 0.0f;
        public static float ColderValue = 0.2f;
        public static float ColdValue = 0.5f;
        public static float WarmValue = 0.8f;
        public static float WarmerValue = 1.0f;

        //Umidade
        public static float DryerValue = 0.0f;
        public static float DryValue = 0.2f;
        public static float WetValue = 0.5f;
        public static float WetterValue = 0.8f;
        public static float WettestValue = 1.0f;

        public static BiomeType[,] BiomeTable = new BiomeType[6, 6] {   
        //COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
        { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
        { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYER
        { BiomeType.Ice, BiomeType.Tundra, BiomeType.Woodland,     BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //DRY
        { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //WET
        { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.SeasonalForest,      BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest },  //WETTER
        { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.TemperateRainforest, BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest }   //WETTEST
        };
        #endregion
    }
}
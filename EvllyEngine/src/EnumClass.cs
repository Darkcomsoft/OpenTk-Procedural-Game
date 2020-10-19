using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src
{
    public enum WorldType : byte
    {
        MidleWorld, CaveWorld
    }

    public enum TypeBlock : byte
    {
        Air, Grass, Dirt, Sand, Snow
    }

    public enum TreeType : byte
    {
        none, Oak, Pine, PineSnow
    }

    public enum BlockVariant : byte
    {
        none
    }

    public enum ChunkState : byte
    {
        noload, nogameLogic, noEntity, AllGameLogic
    }

    public enum GuiElementType : byte
    {
        panel, panelImage, panelDragDrop, Buttom, Lable, InputText, Slider, CheckBox
    }

    public enum HeatType
    {
        Coldest,
        Colder,
        Cold,
        Warm,
        Warmer,
        Warmest
    }

    public enum MoistureType
    {
        Wettest,
        Wetter,
        Wet,
        Dry,
        Dryer,
        Dryest
    }

    public enum BiomeType : byte
    {
        None,
        Bench,
        Desert,
        Savanna,
        TropicalRainforest,
        Grassland,
        Woodland,
        SeasonalForest,
        TemperateRainforest,
        BorealForest,
        Tundra,
        Ice,


        Ocean
    }
}

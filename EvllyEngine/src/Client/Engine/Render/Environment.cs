using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.Engine.Render
{
    public static class Environment
    {
        public static bool Enabled = true;
        public static float Density = 0.014f;
        public static float Distance = 3.5f;
        /// <summary>
        /// the fog color use vector4 But is the same of the (OpenTK.Color4)
        /// </summary>
        public static Color4 FogColor = new Color4(0, 0.7490196f, 1, 1);

        public static Color4 SkyColor = Color4.DeepSkyBlue;
        public static Color4 SkyHorizonColor = Color4.SkyBlue;

        public static Color4 AmbienceColor = Color4.White;

        public static void GetFogColorFromViewDirection()
        {

        }
    }
}

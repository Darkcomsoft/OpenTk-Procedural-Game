using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.Utility
{
    public static class Mathf
    {
        public static int FloorToInt(float value) => (int)Math.Floor(value);
        public static float Round(float value) => (float)Math.Round(value);
    }
}

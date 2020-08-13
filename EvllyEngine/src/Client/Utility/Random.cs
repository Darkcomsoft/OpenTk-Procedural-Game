using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.Utility
{
    public static class Random
    {
        public static System.Random _random = new System.Random("RNGPadraoCy8v4t498y438y9v3498y34v98yv3498y564654jklbvcflnjkvc536487j053e4tr6870.,.,.,,.".GetHashCode());

        public static void SetSeed(string seedString)
        {
            _random = new System.Random(seedString.GetHashCode());
        }

        public static void SetSeed(int seedInt)
        {
            _random = new System.Random(seedInt);
        }

        public static float Range(float MinValue, float MaxValue)
        {
            // Perform arithmetic in double type to avoid overflowing
            double range = (double)MaxValue - (double)MinValue;
            double sample = _random.NextDouble();
            double scaled = (sample * range) + MinValue;
            return (float)scaled;
        }

        public static float Range(float MinValue, float MaxValue, int Seed)
        {
            _random = new System.Random(Seed);

            // Perform arithmetic in double type to avoid overflowing
            double range = (double)MaxValue - (double)MinValue;
            double sample = _random.NextDouble();
            double scaled = (sample * range) + MinValue;
            return (float)scaled;
        }

        public static int Range(int MinValue, int MaxValue)
        {
            return _random.Next(MinValue, MaxValue);
        }

        public static int Rand()
        {
            return _random.Next();
        }

        public static float Randf()
        {
            return (float)_random.NextDouble();
        }
    }
}

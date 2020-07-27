using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.Utility
{
    public static class GCollector
    {
        public static void Collect()
        {
            System.Diagnostics.Debug.WriteLine("GC: Starting Collecting!!!");
            GC.Collect();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public static class Debug
    {
        public static void Log(string menssage)
        {
            System.Diagnostics.Debug.WriteLine("LOG: " + menssage);
        }

        public static void LogWarning(string menssage)
        {
            System.Diagnostics.Debug.WriteLine("WARNING: " + menssage);
        }

        public static void LogError(string menssage)
        {
            System.Diagnostics.Debug.Fail(menssage);
        }

        public static void LogException(string menssage)
        {
            throw new Exception(menssage);
        }
    }
}

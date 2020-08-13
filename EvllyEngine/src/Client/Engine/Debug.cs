using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public static class Debug
    {
        public static void Log(string menssage)
        {
#if Debug && Client
            System.Diagnostics.Debug.WriteLine("LOG: " + menssage);
#elif Debug && Server
            Server.Instance.WriteLine("LLOG: " + menssage);
#endif
        }

        public static void LogWarning(string menssage)
        {
#if Debug && Client
            System.Diagnostics.Debug.WriteLine("WARNING: " + menssage);
#elif Debug && Server
            Server.Instance.WriteLine("WARNING: " + menssage);
#endif
        }

        public static void LogError(string menssage)
        {
#if Debug && Client
            System.Diagnostics.Debug.Fail(menssage);
#elif Debug && Server
            Server.Instance.WriteLine("ERROR: " + menssage);
#endif
        }

        public static void LogException(string menssage)
        {
#if Debug && Client
            throw new Exception(menssage);
#elif Debug && Server
            Server.Instance.WriteLine("Exception: " + menssage);
#endif
        }
    }
}

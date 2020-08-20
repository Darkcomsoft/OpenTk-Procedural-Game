using EvllyEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src
{
    public static class Commands
    {
        /// <summary>
        /// (values[0]) is for the command all the others index's, is for the values passe in next lines in console
        /// </summary>
        /// <param name="values"></param>
        public static void ReadInputCommand(string[] values)
        {
            if (string.Equals(values[0], "Help", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Ayyyy you dont need help, you know everything (: (this command for now is disabled)");
            }
            else if (string.Equals(values[0], "Stop", StringComparison.OrdinalIgnoreCase))
            {
                Commands.Stop();
            }
            else if (string.Equals(values[0], "Quit", StringComparison.OrdinalIgnoreCase))
            {
                Commands.ExitApplication();
            }
            else
            {
                Debug.Log("Don't found this command : " + values[0]);
            }
        }

        /// <summary>
        /// Close the client game
        /// </summary>
        public static void ExitApplication()
        {
#if Client
            if (Window.Instance != null)
            {
                Window.Instance.Exit();
            }
#elif Server
            Debug.Log("Sorry Server cant Call this!");
#endif
        }

        /// <summary>
        /// Stop the server
        /// </summary>
        public static void Stop()
        {
#if Server
            if (Server.Instance != null)
            {
                Server.Instance.Stop();
            }
#elif Client
            Debug.Log("Sorry Client cant Call this!");
#endif
        }
    }
}

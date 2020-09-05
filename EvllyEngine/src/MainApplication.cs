using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProjectEvlly.src;

namespace EvllyEngine
{
    class MainApplication
    {
        public static void Main()
        {
#if Client
            using (Window game = new Window(1000, 600, GlobalData.AppName + " : " + GlobalData.Version))
            {
                game.RenderFrame += (sender, e) =>
                {
                    Thread.Sleep(10);
                };
                
                game.Run();
            }
#endif

#if Server
            using (Server server = new Server())
            {
                server.Run();
            }
#endif
        }
    }
}

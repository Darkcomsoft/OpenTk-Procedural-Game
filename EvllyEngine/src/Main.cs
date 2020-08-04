using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EvllyEngine
{
    class MainApplication
    {
        public static void Main()
        {
            using (Window game = new Window(1000, 600, "ProjectEvlly"))
            {
                game.RenderFrame += (sender, e) =>
                {
                    Thread.Sleep(10);
                };
                
                game.Run();
            }
        }
    }
}

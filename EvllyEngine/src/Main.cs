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
            using (Engine game = new Engine(1000, 600, "ProjectEvlly"))
            {
                game.RenderFrame += (sender, e) =>
                {
                    Thread.Sleep(15);
                };
                game.Run(60);
            }
        }
    }
}

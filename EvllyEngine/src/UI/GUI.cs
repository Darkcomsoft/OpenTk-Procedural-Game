using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.UI
{
    public class GUI
    {
        public GUI()
        {

        }

        public static void Panel(Rectangle rectangle)
        {
            GL.Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
    }
}

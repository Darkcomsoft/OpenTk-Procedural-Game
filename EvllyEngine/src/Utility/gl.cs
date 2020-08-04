using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ProjectEvlly.src.Utility
{
    /// <summary>
    /// Or custom class to call GL commands, but is the same thing using normal (OpenGL.GL) Class
    /// </summary>
    public static class gl
    {
        public static void ClearColor(Color4 color)
        {
#if Client
            GL.ClearColor(color);
#endif
        }

        public static void Clear()
        {
#if Client
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
#endif
        }

        public static void Clear(ClearBufferMask mask)
        {
#if Client
            GL.Clear(mask);
#endif
        }

        public static void Enable(EnableCap cap)
        {
#if Client
            GL.Enable(cap);
#endif
        }

        public static void Disable(EnableCap cap)
        {
#if Client
            GL.Enable(cap);
#endif
        }

        public static void Viewport(int x, int y, int width, int height)
        {
#if Client
            GL.Viewport(x, y, width, height);
#endif
        }
    }
}

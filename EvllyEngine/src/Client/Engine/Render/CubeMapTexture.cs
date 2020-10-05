using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ProjectEvlly.src.Engine.Render
{
    public class CubeMapTexture : IDisposable
    {
        private int handler;

        public PixelInternalFormat _PixelInternalFormat = PixelInternalFormat.Rgba;
        public PixelFormat _PixelFormat = PixelFormat.Bgra;

        public CubeMapTexture(ImageFile[] imageFile)
        {
            GL.Enable(EnableCap.TextureCubeMap);

            handler = GL.GenTexture();

            Use();

            for (int i = 0; i < imageFile.Length; i++)
            {
                GL.TexImage2D(targets[i], 0, _PixelInternalFormat, imageFile[i]._width, imageFile[i]._height, 0, _PixelFormat, PixelType.UnsignedByte, imageFile[i]._ImgData);
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureParameterName.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureParameterName.ClampToEdge);
        }

        public void Use(TextureUnit unit = TextureUnit.Texture0) 
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.TextureCubeMap, handler);
        }

        public void Dispose()
        {
            GL.DeleteTexture(handler);
        }

        TextureTarget[] targets =
        {
            TextureTarget.TextureCubeMapNegativeX, TextureTarget.TextureCubeMapNegativeY,
            TextureTarget.TextureCubeMapNegativeZ, TextureTarget.TextureCubeMapPositiveX,
            TextureTarget.TextureCubeMapPositiveY, TextureTarget.TextureCubeMapPositiveZ
        };
    }
}

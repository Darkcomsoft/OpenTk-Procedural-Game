using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace EvllyEngine
{
    public class Texture : IDisposable
    {
        public int Handle;
        public int _Width;
        public int _Height;
        public byte[] _imagePixel;

        public TextureTarget _TextureTarget = TextureTarget.Texture2D;
        public PixelInternalFormat _PixelInternalFormat = PixelInternalFormat.Rgba;
        public PixelFormat _PixelFormat = PixelFormat.Bgra;


        public Texture(ImageFile imageData, TextureUnit textureUnit = TextureUnit.Texture0)
        {
            _Width = imageData._width;
            _Height = imageData._height;

            Handle = GL.GenTexture();

            Use(textureUnit);

            GL.TexImage2D(_TextureTarget, 0, _PixelInternalFormat, _Width, _Height, 0, _PixelFormat, PixelType.UnsignedByte, imageData._ImgData);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public Texture(ImageFile imageData, TextureMinFilter textureMinFilter, TextureMagFilter textureMagFilter)
        {
            _Width = imageData._width;
            _Height = imageData._height;

            Handle = GL.GenTexture();

            Use();

            GL.TexImage2D(_TextureTarget, 0, _PixelInternalFormat, _Width, _Height, 0, _PixelFormat, PixelType.UnsignedByte, imageData._ImgData);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)textureMinFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)textureMagFilter);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void Use(TextureUnit textunit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(textunit);
            GL.BindTexture(_TextureTarget, Handle);
        }

        public void Dispose()
        {
            GL.DeleteTexture(Handle);
            _imagePixel = null;
        }

        public byte[] GetimageData { get { return _imagePixel; } }
        public int GetWidth { get { return _Width; } }
        public int GetHeight { get { return _Height; } }
    }
}

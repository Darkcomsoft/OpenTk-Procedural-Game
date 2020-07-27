using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using System.Drawing.Imaging;
using System.Xml;

namespace EvllyEngine
{
    public class AssetsManager
    {
        public static AssetsManager instance;
        private Mesh Engine_Error;
        private Dictionary<AssetType, ImageFile> Texture = new Dictionary<AssetType, ImageFile>();
        private Dictionary<AssetType, Mesh> Model = new Dictionary<AssetType, Mesh>();
        private Dictionary<AssetType, ShaderFile> Shader = new Dictionary<AssetType, ShaderFile>();

        public AssetsManager() 
        {
            instance = this;
            Engine_Error = LoadModel("Assets/Models/", "error");
        }

        public void UnloadAll()
        {
            foreach (var item in Model)
            {
                item.Value.Clear();
            }

            Texture.Clear();
            Model.Clear();
            Shader.Clear();

            Texture = null;
            Model = null;
            Shader = null;
        }

        public static ShaderFile LoadShader(string path, string file)
        {
            string vertshader = string.Concat(path, file, ".vert");
            string fragshader = string.Concat(path, file, ".frag");

            if (!File.Exists(vertshader) || !File.Exists(fragshader))
            {
                Debug.LogError("Shader Files Can't be found!");
                throw new Exception("Shader Files Can't be found!");
            }

            return new ShaderFile(File.ReadAllText(vertshader), File.ReadAllText(fragshader));
        }

        public static ImageFile LoadImage(string path, string file, string extensio)
        {
            if (!File.Exists(string.Concat(path, file, "." + extensio)))
            {
                Debug.LogError("Texture Files Can't be found At: " + string.Concat(path, file, "." + extensio));
                throw new Exception("Texture Files Can't be found At: " + string.Concat(path, file, "." + extensio));
            }
           
            using (var image = new Bitmap(string.Concat(path, file, "." + extensio)))
            {
                image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                var data = image.LockBits( new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                return new ImageFile(data.Scan0, data.Width, data.Height);
            }
        }

        public static Mesh LoadModel(string path, string file)
        {
            if (!File.Exists(string.Concat(path, file, ".dae")))
            {
                Debug.LogError("Model(COLLADA) Files Can't be found At: " + string.Concat(path, file, ".dae"));
                throw new Exception("Model(COLLADA) Files Can't be found At: " + string.Concat(path, file, ".dae"));
            }

            ColladaProcessor processor = new ColladaProcessor(string.Concat(path, file, ".dae"));

            return processor.Load();
        }

        public ImageFile GetTexture(string TextureName, string fileextensio)
        {
            if (Texture.TryGetValue(new AssetType(TextureName, fileextensio), out ImageFile texture))
            {
                return texture;
            }
            else
            {
                ImageFile tex = LoadImage("Assets/Texture/", TextureName, fileextensio);
                Texture.Add(new AssetType(TextureName, fileextensio), tex);
                return tex;
            }
        }
        public Mesh GetMesh(string MeshName)
        {
            if (Model.TryGetValue(new AssetType(MeshName, ".dae"), out Mesh _mesh))
            {
                return _mesh;
            }
            else
            {
                Mesh mesh = LoadModel("Assets/Models/", MeshName);
                Model.Add(new AssetType(MeshName, ".dae"), mesh);
                return mesh;
            }
        }
        public ShaderFile GetShader(string MeshName)
        {
            if (Shader.TryGetValue(new AssetType(MeshName, "shader"), out ShaderFile _shader))
            {
                return _shader;
            }
            else
            {
                ShaderFile shader = LoadShader("Assets/Shaders/", MeshName);
                Shader.Add(new AssetType(MeshName, "shader"), shader);
                return shader;
            }
        }

        public Mesh GetErrorMesh { get { return Engine_Error; } }
    }
}

public struct AssetType
{
    public string name;
    public string FileExtension;
    private string textureName;
    private string filename;

    public AssetType(string textureName, string filename) : this()
    {
        this.textureName = textureName;
        this.filename = filename;
    }
}

public struct ShaderFile
{
    public string _vertshader;
    public string _fragshader;

    public ShaderFile(string vertshader, string fragshader)
    {
        _vertshader = vertshader;
        _fragshader = fragshader;
    }
}

public struct ImageFile
{
    public int _width;
    public int _height;
    public IntPtr _ImgData;

    public ImageFile(IntPtr scan0, int width, int height) : this()
    {
        this._ImgData = scan0;
        this._width = width;
        this._height = height;
    }
}
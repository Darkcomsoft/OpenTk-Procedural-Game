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
using ProjectEvlly.src.Utility;
using OpenTK;
using System.Runtime.CompilerServices;
using System.Threading;
using ProjectEvlly;

namespace EvllyEngine
{
    public class AssetsManager
    {
        public static AssetsManager instance;
        private Mesh Engine_Error;

        private Dictionary<string, Texture> _Textures;
        private Dictionary<string, Shader> _Shaders;
        private Dictionary<string, Mesh> _Models;

        private Dictionary<string, float[]> _Tiles;

        public AssetsManager() 
        {
            instance = this;

            _Textures = new Dictionary<string, Texture>();
            _Shaders = new Dictionary<string, Shader>();
            _Models = new Dictionary<string, Mesh>();

            _Tiles = new Dictionary<string, float[]>();
        }

        public void LoadAssets()
        {
            SplashScreen.SetState("Loading Error Model", SplashScreenStatus.Loading);
            Engine_Error = LoadModel("Assets/Models/", "error");

            //Load Mesh
            SplashScreen.SetState("Loading Models", SplashScreenStatus.Loading);
            _Models.Add("oak", LoadModel("Assets/Models/", "oak"));
            _Models.Add("Cube", LoadModel("Assets/Models/", "Cube"));
            _Models.Add("SkySphere", LoadModel("Assets/Models/", "SkySphere"));

            //Load Textures
            SplashScreen.SetState("Loading Textures", SplashScreenStatus.Loading);
            _Textures.Add("devTexture", new Texture(AssetsManager.LoadImage("Assets/Texture/", "devTexture", "jpg")));
            _Textures.Add("devTexture2", new Texture(AssetsManager.LoadImage("Assets/Texture/", "devTexture2", "png")));
            _Textures.Add("TileAtlas", new Texture(AssetsManager.LoadImage("Assets/Texture/", "TileAtlas", "png")));
            _Textures.Add("SpritesTreeHigt", new Texture(AssetsManager.LoadImage("Assets/Texture/", "SpritesTreeHigt", "png")));
            _Textures.Add("Water", new Texture(AssetsManager.LoadImage("Assets/Texture/", "Water", "png")));

            //Load Shaders
            SplashScreen.SetState("Loading Shaders", SplashScreenStatus.Loading);
            _Shaders.Add("Default", new Shader(AssetsManager.LoadShader("Assets/Shaders/", "Default")));
            _Shaders.Add("UI", new Shader(AssetsManager.LoadShader("Assets/Shaders/", "UI")));

            //Load TileUvs
            SplashScreen.SetState("Loading Tile UVs", SplashScreenStatus.Loading);
            AddTileUv("Grass", new Vector2(0.15f, 0.066667f), new Vector2(0.15f, 0f), new Vector2(0.2f, 0.066667f), new Vector2(0.2f, 0f));
            AddTileUv("Dirt", new Vector2(0.55f, 0.066667f), new Vector2(0.55f, 0f), new Vector2(0.6f, 0.066667f), new Vector2(0.6f, 0f));
        }

        public void UnloadAll()
        {
            foreach (var item in _Textures)
            {
                item.Value.Delete();
            }

            foreach (var item in _Shaders)
            {
                item.Value.Delete();
            }

            foreach (var item in _Models)
            {
                item.Value.Clear();
            }

            _Textures.Clear();
            _Models.Clear();
            _Shaders.Clear();

            Engine_Error.Clear();
            Engine_Error = null;

            _Textures = null;
            _Models = null;
            _Shaders = null;

            GCollector.Collect();
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
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                
                //image.MakeTransparent();
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

        public static Texture GetTexture(string TextureName)
        {
            if (AssetsManager.instance._Textures.TryGetValue(TextureName, out Texture texture))
            {
                return texture;
            }
            else
            {
                throw new Exception("Dont Exist this Assets: " + TextureName);
            }
        }
        public static Mesh GetMesh(string MeshName)
        {
            if (AssetsManager.instance._Models.TryGetValue(MeshName, out Mesh _mesh))
            {
                return _mesh;
            }
            else
            {
                throw new Exception("Dont Exist this Assets: " + MeshName);
            }
        }
        public static Shader GetShader(string ShaderName)
        {
            if (AssetsManager.instance._Shaders.TryGetValue(ShaderName, out Shader texture))
            {
                return texture;
            }
            else
            {
                throw new Exception("Dont Exist this Assets: " + ShaderName);
            }
        }

        public static void UseTexture(string TextureName)
        {
            if (AssetsManager.instance._Textures.TryGetValue(TextureName, out Texture texture))
            {
                texture.Use();
            }
        }

        public static void UseShader(string ShaderName)
        {
            if (AssetsManager.instance._Shaders.TryGetValue(ShaderName, out Shader texture))
            {
                texture.Use();
            }
        }

        public static void ShaderSet(string ShaderName, string name, Matrix4 matrix)
        {
            if (AssetsManager.instance._Shaders.TryGetValue(ShaderName, out Shader texture))
            {
                texture.SetMatrix4(name, matrix);
            }
        }

        public void AddTileUv(string tileName,Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4)
        {
            float[] uvs = new float[8];

            uvs[0] = point1.X;
            uvs[1] = point1.Y;

            uvs[2] = point2.X;
            uvs[3] = point2.Y;

            uvs[4] = point3.X;
            uvs[5] = point3.Y;

            uvs[6] = point4.X;
            uvs[7] = point4.Y;

            _Tiles.Add(tileName, uvs);
        }

        public static float[] GetTileUV(string tileName)
        {
            if (AssetsManager.instance._Tiles.TryGetValue(tileName, out float[] arraylist))
            {
                return arraylist;
            }
            else
            {
                throw new Exception("Dont Found this Tile UV: " + tileName);
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
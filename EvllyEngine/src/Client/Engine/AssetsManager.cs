using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using ProjectEvlly.src.Utility;
using OpenTK;
using ProjectEvlly;
using ProjectEvlly.src.UI.Font;
using ProjectEvlly.src.Engine.Render;
using ProjectEvlly.src.Engine.Sound;
using OpenTK.Graphics;

namespace EvllyEngine
{
    public class AssetsManager : IDisposable
    {
        public static AssetsManager instance;

        private Dictionary<string, Texture> _Textures;
        private Dictionary<string, CubeMapTexture> _CubeTextures;
        private Dictionary<string, Shader> _Shaders;
        private Dictionary<string, Mesh> _Models;
        private Dictionary<string, AudioClip> _Sounds;
        private Dictionary<string, FontType> _Fonts;

        private Dictionary<string, Vector2[]> _Tiles;

        public AssetsManager() 
        {
            instance = this;

            _Textures = new Dictionary<string, Texture>();
            _CubeTextures = new Dictionary<string, CubeMapTexture>();
            _Shaders = new Dictionary<string, Shader>();
            _Models = new Dictionary<string, Mesh>();
            _Sounds = new Dictionary<string, AudioClip>();
            _Fonts = new Dictionary<string, FontType>();

            _Tiles = new Dictionary<string, Vector2[]>();

            LoadAssets();
        }

        private void LoadAssets()
        {
            SplashScreen.SetState("Loading Models Trees", SplashScreenStatus.Loading);
            _Models.Add("oak", LoadModel("Assets/Models/Trees/", "oak"));
            _Models.Add("Pine01", LoadModel("Assets/Models/Trees/", "Pine01"));

            SplashScreen.SetState("Loading Others Models", SplashScreenStatus.Loading);
            _Models.Add("Cube", LoadModel("Assets/Models/", "Cube"));
            _Models.Add("SkyCube", LoadModel("Assets/Models/", "SkyCube"));
            _Models.Add("SwordMetal", LoadModel("Assets/Models/", "SwordMetal"));
            _Models.Add("SkySphere", LoadModel("Assets/Models/", "SkySphere"));
            _Models.Add("Ship01", LoadModel("Assets/Models/", "Ship01"));
            _Models.Add("Table", LoadModel("Assets/Models/Teste/", "Table"));

            SplashScreen.SetState("Loading Textures", SplashScreenStatus.Loading);
            _Textures.Add("devTexture", new Texture(LoadImage("Assets/Texture/", "devTexture", "jpg")));
            _Textures.Add("devTexture2", new Texture(LoadImage("Assets/Texture/", "devTexture2", "png")));
            _Textures.Add("TileAtlas", new Texture(LoadImage("Assets/Texture/", "TileAtlas", "png")));
            _Textures.Add("SpritesTreeHigt", new Texture(LoadImage("Assets/Texture/", "SpritesTreeHigt", "png")));
            _Textures.Add("SpritesTreeHigt_Snow", new Texture(LoadImage("Assets/Texture/", "SpritesTreeHigt_Snow", "png")));
            _Textures.Add("MetalSword", new Texture(LoadImage("Assets/Texture/", "MetalSword", "png")));
            _Textures.Add("Water", new Texture(LoadImage("Assets/Texture/", "Water", "png")));
            _Textures.Add("Water2", new Texture(LoadImage("Assets/Texture/", "Water2", "png")));
            _Textures.Add("Cloud", new Texture(LoadImage("Assets/Texture/", "Cloud", "png")));
            _Textures.Add("Wood02", new Texture(LoadImage("Assets/Texture/", "Wood02", "png")));
            _Textures.Add("Wood01", new Texture(LoadImage("Assets/Texture/", "Wood01", "png")));

            SplashScreen.SetState("Loading Sounds", SplashScreenStatus.Loading);
            _Sounds.Add("Teste", new AudioClip("Assets/Sound/", "Teste"));

            SplashScreen.SetState("Loading CubeMaps", SplashScreenStatus.Loading);
            _CubeTextures.Add("Example", new CubeMapTexture(LoadCubeImages("Assets/Texture/CubeMap/", "Example", "png")));

            SplashScreen.SetState("Loading Images", SplashScreenStatus.Loading);
            _Textures.Add("Darkcomsoft", new Texture(LoadImage("Assets/Images/", "Darkcomsoft", "png")));
            _Textures.Add("VaKLogoYellow", new Texture(LoadImage("Assets/Images/", "VaKLogoYellow", "png")));
            _Textures.Add("BackGround", new Texture(LoadImage("Assets/Images/", "BackGround", "png")));

            SplashScreen.SetState("Loading GUI Assets", SplashScreenStatus.Loading);
            _Textures.Add("Buttom", new Texture(LoadImage("Assets/UI/", "Buttom", "png")));
            _Textures.Add("SidePanel", new Texture(LoadImage("Assets/UI/", "SidePanel", "png")));
            _Textures.Add("Panel01", new Texture(LoadImage("Assets/UI/", "Panel01", "png")));

            SplashScreen.SetState("Loading Shaders", SplashScreenStatus.Loading);
            _Shaders.Add("Default", new Shader(LoadShader("Assets/Shaders/", "Default")));
            _Shaders.Add("TerrainDefault", new Shader(LoadShader("Assets/Shaders/", "TerrainDefault")));
            _Shaders.Add("UI", new Shader(LoadShader("Assets/Shaders/", "UI")));
            _Shaders.Add("Water", new Shader(LoadShader("Assets/Shaders/", "Water")));
            _Shaders.Add("GUI", new Shader(LoadShader("Assets/Shaders/", "GUI")));
            _Shaders.Add("Sky", new Shader(LoadShader("Assets/Shaders/", "Sky")));
            _Shaders.Add("Cloud", new Shader(LoadShader("Assets/Shaders/", "Cloud")));
            _Shaders.Add("Font", new Shader(LoadShader("Assets/Shaders/", "Font")));

            SplashScreen.SetState("Setting-UP Tile UVs", SplashScreenStatus.Loading);
            AddTileUv("Grass", new Vector2(0.15f, 0.066667f), new Vector2(0.15f, 0f), new Vector2(0.2f, 0.066667f), new Vector2(0.2f, 0f));
            AddTileUv("Dirt", new Vector2(0.55f, 0.066667f), new Vector2(0.55f, 0f), new Vector2(0.6f, 0.066667f), new Vector2(0.6f, 0f));
            AddTileUv("Sand", new Vector2(0.8f, 0.066667f), new Vector2(0.8f, 0f), new Vector2(0.85f, 0.066667f), new Vector2(0.85f, 0f));
            AddTileUv("Water", new Vector2(0.2f, 0.066667f), new Vector2(0.2f, 0f), new Vector2(0.25f, 0.066667f), new Vector2(0.25f, 0f));
            AddTileUv("Snow", new Vector2(0.7f, 0.066667f), new Vector2(0.7f, 0f), new Vector2(0.75f, 0.066667f), new Vector2(0.75f, 0f));

            SplashScreen.SetState("Loading Fonts", SplashScreenStatus.Loading);
            _Fonts.Add("PixelFont", LoadFont("Assets/UI/Fonts/", "PixelFont"));
            _Fonts.Add("PixelFont2", LoadFont("Assets/UI/Fonts/", "PixelFont2"));
        }

        public void Dispose()
        {
            foreach (var item in _Textures)
            {
                item.Value.Dispose();
            }

            foreach (var item in _CubeTextures)
            {
                item.Value.Dispose();
            }

            foreach (var item in _Shaders)
            {
                item.Value.Delete();
            }

            foreach (var item in _Models)
            {
                item.Value.Dispose();
            }

            foreach (var item in _Fonts)
            {
                item.Value.Dispose();
            }

            foreach (var item in _Sounds)
            {
                item.Value.Dispose();
            }

            _Sounds.Clear();
            _Textures.Clear();
            _Models.Clear();
            _Shaders.Clear();
            _Fonts.Clear();
            _Tiles.Clear();

            _Sounds = null;
            _Textures = null;
            _Models = null;
            _Shaders = null;
            _Tiles = null;
            _Fonts = null;

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
        public static ImageFile[] LoadCubeImages(string path, string file, string extensio)
        {
            List<ImageFile> files = new List<ImageFile>();

            string finalPath = path + file + "/";

            for (int i = 0; i < 6; i++)
            {
                string finalFileName = file;
                RotateFlipType rotate = RotateFlipType.RotateNoneFlipNone;
                if (i == 0)
                {
                    finalFileName = file + "_rt";
                }
                else if (i == 1)
                {
                    finalFileName = file + "_dn";
                }
                else if (i == 2)
                {
                    finalFileName = file + "_bk";
                }
                else if (i == 3)
                {
                    finalFileName = file + "_lf";
                }
                else if (i == 4)
                {
                    finalFileName = file + "_up";
                    rotate = RotateFlipType.Rotate90FlipNone;
                }
                else if (i == 5)
                {
                    finalFileName = file + "_ft";
                }

                if (!File.Exists(string.Concat(finalPath, finalFileName, "." + extensio)))
                {
                    Debug.LogError("Texture Files Can't be found At: " + string.Concat(finalPath, finalFileName, "." + extensio));
                    throw new Exception("Texture Files Can't be found At: " + string.Concat(finalPath, finalFileName, "." + extensio));
                }

                using (var image = new Bitmap(string.Concat(finalPath, finalFileName, "." + extensio)))
                {
                    image.RotateFlip(rotate);

                    //image.MakeTransparent();
                    var data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                    files.Add(new ImageFile(data.Scan0, data.Width, data.Height));
                }
            }

            return files.ToArray();
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
        public FontType LoadFont(string path, string file)
        {
            string fontFile = string.Concat(path, file, ".fnt");
            string fontAtlas = string.Concat(path, file, ".png");

            if (!File.Exists(fontFile) || !File.Exists(fontAtlas))
            {
                Debug.LogError("Shader Files Can't be found!");
                throw new Exception("Shader Files Can't be found!");
            }

            return new FontType(fontFile, fontAtlas);
        }
        public static ImageFile LoadImage(string path)
        {
            using (var image = new Bitmap(path))
            {
                //image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                //image.MakeTransparent();
                var data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                return new ImageFile(data.Scan0, data.Width, data.Height);
            }
        }

        public static int GetSound(string soundName)
        {
            if (AssetsManager.instance._Sounds.TryGetValue(soundName, out AudioClip clip))
            {
                return clip.GetHandler();
            }
            else
            {
                return 0;
            }
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
        public static CubeMapTexture GetCubeMap(string TextureName)
        {
            if (AssetsManager.instance._CubeTextures.TryGetValue(TextureName, out CubeMapTexture texture))
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
        public static FontType GetFont(string FontName)
        {
            if (AssetsManager.instance._Fonts.TryGetValue(FontName, out FontType font))
            {
                return font;
            }
            else
            {
                throw new Exception("Dont Exist this Assets: " + FontName);
            }
        }

        #region UseFunctions
        public static void UseTexture(string TextureName)
        {
            if (AssetsManager.instance._Textures.TryGetValue(TextureName, out Texture texture))
            {
                texture.Use();
            }
        }

        public static void UseShader(string ShaderName)
        {
            if (AssetsManager.instance._Shaders.TryGetValue(ShaderName, out Shader shader))
            {
                shader.Use();
            }
        }

        public static void ShaderSet(string ShaderName, string name, Matrix4 value)
        {
            if (AssetsManager.instance._Shaders.TryGetValue(ShaderName, out Shader shader))
            {
                shader.SetMatrix4(name, value);
            }
        }

        public static void ShaderSet(string ShaderName, string name, Matrix4d value)
        {
            if (AssetsManager.instance._Shaders.TryGetValue(ShaderName, out Shader shader))
            {
                shader.SetMatrix4d(name, value);
            }
        }

        public static void ShaderSet(string ShaderName, string name, int value)
        {
            if (AssetsManager.instance._Shaders.TryGetValue(ShaderName, out Shader shader))
            {
                shader.SetInt(name, value);
            }
        }

        public static void ShaderSet(string ShaderName, string name, float value)
        {
            if (AssetsManager.instance._Shaders.TryGetValue(ShaderName, out Shader shader))
            {
                shader.SetFloat(name, value);
            }
        }

        public static void ShaderSet(string ShaderName, string name, bool value)
        {
            if (AssetsManager.instance._Shaders.TryGetValue(ShaderName, out Shader shader))
            {
                shader.Setbool(name, value);
            }
        }

        public static void ShaderSet(string ShaderName, string name, Vector3 value)
        {
            if (AssetsManager.instance._Shaders.TryGetValue(ShaderName, out Shader shader))
            {
                shader.SetVector3(name, value);
            }
        }

        public static void ShaderSet(string ShaderName, string name, Vector4 value)
        {
            if (AssetsManager.instance._Shaders.TryGetValue(ShaderName, out Shader shader))
            {
                shader.SetVector4(name, value);
            }
        }

        public static void ShaderSet(string ShaderName, string name, Color4 value)
        {
            if (AssetsManager.instance._Shaders.TryGetValue(ShaderName, out Shader shader))
            {
                shader.SetColor(name, value);
            }
        }
        #endregion

        public void AddTileUv(string tileName,Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4)
        {
            Vector2[] uvs = new Vector2[4];

            uvs[0] = point1;
            uvs[1] = point2;
            uvs[2] = point3;
            uvs[3] = point4;

            _Tiles.Add(tileName, uvs);
        }
        public static Vector2[] GetTileUV(string tileName)
        {
            if (AssetsManager.instance._Tiles.TryGetValue(tileName, out Vector2[] arraylist))
            {
                return arraylist;
            }
            else
            {
                throw new Exception("Dont Found this Tile UV: " + tileName);
            }
        }
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
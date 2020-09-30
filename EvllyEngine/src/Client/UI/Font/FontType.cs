using EvllyEngine;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.UI.Font
{
    public class FontType : IDisposable
    {
	    private double aspectRatio;

        private double verticalPerPixelSize;
        private double horizontalPerPixelSize;
        private double spaceWidth;
        private int[] padding;
        private int paddingWidth;
        private int paddingHeight;

        private Dictionary<string, string> values = new Dictionary<string, string>();
        private Dictionary<int, Character> metaData = new Dictionary<int, Character>();

        private string[] lines;
        private int ReadCountLine = 0;

        public Texture AtlasTexture;

        public FontType(string fontFile, string fontAtlas)
        {
            AtlasTexture = new Texture(AssetsManager.LoadImage(fontAtlas), TextureMinFilter.Linear, TextureMagFilter.Linear);

            aspectRatio = (double)Window.Instance.Width / (double)Window.Instance.Height;

            lines = File.ReadAllLines(fontFile);

            loadPaddingData();
            loadLineSizes();
            int imageWidth = getValueOfVariable("scaleW");
            loadCharacterData(imageWidth);
        }

        private bool NextLine()
        {
            values.Clear();

            if (ReadCountLine >= lines.Length - 1)
            {
                return false;
            }

            string[] lineSplit = lines[ReadCountLine].Split(" "[0]);
            ReadCountLine++;

            for (int i = 0; i < lineSplit.Length; i++)
            {
               string[] valuePairs = lineSplit[i].Split("="[0]);
                    
                    if (valuePairs.Length == 2)
                    {
                        values.Add(valuePairs[0], valuePairs[1]);
                    }
            }

            return true;
        }

        private void loadPaddingData()
        {
            NextLine();
            this.padding = getValuesOfVariable("padding");
            this.paddingWidth = padding[PAD_LEFT] + padding[PAD_RIGHT];
            this.paddingHeight = padding[PAD_TOP] + padding[PAD_BOTTOM];
        }

        private int[] getValuesOfVariable(string variable)
        {
            string[] numbers = values[variable].Split(","[0]);
            int[] actualValues = new int[numbers.Length];

            for (int i = 0; i < actualValues.Length; i++)
            {
                actualValues[i] = int.Parse(numbers[i]);
            }
            return actualValues;
        }

        private int getValueOfVariable(string variable)
        {
            return int.Parse(values[variable]);
        }

        private void loadLineSizes()
        {
            NextLine();
            int lineHeightPixels = getValueOfVariable("lineHeight") - paddingHeight;
            verticalPerPixelSize = LINE_HEIGHT / (double)lineHeightPixels;
            horizontalPerPixelSize = verticalPerPixelSize / aspectRatio;
        }

        private void loadCharacterData(int imageWidth)
        {
            NextLine();
            NextLine();
            while (NextLine())
            {
                Character c = loadCharacter(imageWidth);
                if (c != null)
                {
                    metaData.Add(c.getId(), c);
                }
            }
        }

        private Character loadCharacter(int imageSize)
        {
            int id = getValueOfVariable("id");
            if (id == SPACE_ASCII)
            {
                this.spaceWidth = (getValueOfVariable("xadvance") - paddingWidth) * horizontalPerPixelSize;
                return null;
            }
            double xTex = ((double)getValueOfVariable("x") + (padding[PAD_LEFT] - DESIRED_PADDING)) / imageSize;
            double yTex = ((double)getValueOfVariable("y") + (padding[PAD_TOP] - DESIRED_PADDING)) / imageSize;
            int width = getValueOfVariable("width") - (paddingWidth - (2 * DESIRED_PADDING));
            int height = getValueOfVariable("height") - ((paddingHeight) - (2 * DESIRED_PADDING));
            double quadWidth = width * horizontalPerPixelSize;
            double quadHeight = height * verticalPerPixelSize;
            double xTexSize = (double)width / imageSize;
            double yTexSize = (double)height / imageSize;
            double xOff = (getValueOfVariable("xoffset") + padding[PAD_LEFT] - DESIRED_PADDING) * horizontalPerPixelSize;
            double yOff = (getValueOfVariable("yoffset") + (padding[PAD_TOP] - DESIRED_PADDING)) * verticalPerPixelSize;
            double xAdvance = (getValueOfVariable("xadvance") - paddingWidth) * horizontalPerPixelSize;
            return new Character(id, xTex, yTex, xTexSize, yTexSize, xOff, yOff, quadWidth, quadHeight, xAdvance);
        }

        public double getSpaceWidth()
        {
            return spaceWidth;
        }

        public Character getCharacter(int ascii)
        {
            return metaData[ascii];
        }

        public void Dispose()
        {
            values.Clear();
            metaData.Clear();

            lines = null;
            values = null;
            metaData = null;
            padding = null;
        }

        private const int PAD_TOP = 0;
        private const int PAD_LEFT = 1;
        private const int PAD_BOTTOM = 2;
        private const int PAD_RIGHT = 3;

        private const int DESIRED_PADDING = 3;

        private const string Spliter = " ";
        private const string NumSpliter = ",";

        public const double LINE_HEIGHT = 0.03f;
        public const int SPACE_ASCII = 32;
    }

    public class Character
    {
        private int id;
        private double xTextureCoord;
        private double yTextureCoord;
        private double xMaxTextureCoord;
        private double yMaxTextureCoord;
        private double xOffset;
        private double yOffset;
        private double sizeX;
        private double sizeY;
        private double xAdvance;


        public Character(int id, double xTextureCoord, double yTextureCoord, double xTexSize, double yTexSize, double xOffset, double yOffset, double sizeX, double sizeY, double xAdvance)
        {
            this.id = id;
            this.xTextureCoord = xTextureCoord;
            this.yTextureCoord = yTextureCoord;
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.xMaxTextureCoord = xTexSize + xTextureCoord;
            this.yMaxTextureCoord = yTexSize + yTextureCoord;
            this.xAdvance = xAdvance;
        }

        public int getId()
        {
            return id;
        }

        public double getxTextureCoord()
        {
            return xTextureCoord;
        }

        public double getyTextureCoord()
        {
            return yTextureCoord;
        }

        public double getXMaxTextureCoord()
        {
            return xMaxTextureCoord;
        }

        public double getYMaxTextureCoord()
        {
            return yMaxTextureCoord;
        }

        public double getxOffset()
        {
            return xOffset;
        }

        public double getyOffset()
        {
            return yOffset;
        }

        public double getSizeX()
        {
            return sizeX;
        }

        public double getSizeY()
        {
            return sizeY;
        }

        public double getxAdvance()
        {
            return xAdvance;
        }

    }
}

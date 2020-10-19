using EvllyEngine;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using System.Drawing;

namespace ProjectEvlly.src.UI.Font
{
	public class FontRender : IDisposable
	{
		private string textString;
		private double fontSize;

		public TextAling textAling = TextAling.Center;
		public Rectangle _ParentRectangle;
		public Vector2d _FinalPos;

		private Color4 colour = Color4.White;

		private Vector2 position;
		private float lineMaxSize;
		private int numberOfLines;

		private string FontName;

		private int VAO, IBO, vbo, ubo;
		private TextMeshData textMeshData;
		FontType font;
		private bool Ready = false;

		public FontRender(string text, float fontsize, string fontName, Vector2 Position, float maxLineLength, Rectangle frec)
		{
			textString = text;
			fontSize = fontsize;
			position = Position;
			lineMaxSize = maxLineLength;
			FontName = fontName;
			_ParentRectangle = frec;

			font = AssetsManager.GetFont(FontName);

			GenRectangle();

			textMeshData = createTextMesh();
			SetUpBuffers();
		}

		public void TickRender()
		{
			if (Ready)
			{
				GL.Enable(EnableCap.Blend);
				GL.Disable(EnableCap.DepthTest);
				GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

				GL.BindVertexArray(VAO);
				GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

				GUI.GetFontShader.Use();

				font.AtlasTexture.Use();

				GUI.GetFontShader.Setbool("HaveTexture", true);
				GUI.GetFontShader.SetColor("MainColor", colour);

				GUI.GetFontShader.SetMatrix4("projection", Matrix4.CreateOrthographicOffCenter(Window.Instance.ClientRectangle.Left, Window.Instance.ClientRectangle.Right, Window.Instance.ClientRectangle.Bottom, Window.Instance.ClientRectangle.Top, 0f, 5.0f));

				GL.DrawArrays(PrimitiveType.Triangles, 0, textMeshData.getVertexLength());

				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				GL.BindVertexArray(0);

				GL.Enable(EnableCap.DepthTest);
				GL.Disable(EnableCap.Blend);
			}
		}

		public void Resize(Rectangle rec)
		{
			_ParentRectangle = rec;
			UpdateText(textString);
		}

		public void UpdateText(string text)
		{
			GenRectangle();

			textString = text;

			Ready = false;
			textMeshData = null;

			textMeshData = createTextMesh();

			GL.BindVertexArray(VAO);

			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.BufferData(BufferTarget.ArrayBuffer, textMeshData.getVertexLength() * Vector2.SizeInBytes, textMeshData.getVertexPositions(), BufferUsageHint.DynamicDraw);
			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);
			GL.EnableVertexAttribArray(0);

			GL.BindBuffer(BufferTarget.ArrayBuffer, ubo);
			GL.BufferData(BufferTarget.ArrayBuffer, textMeshData.getTextureCoordsLength() * Vector2.SizeInBytes, textMeshData.getTextureCoords(), BufferUsageHint.DynamicDraw);
			GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
			GL.EnableVertexAttribArray(1);

			Ready = true;
		}

		public void SetColor(Color4 color)
		{
			colour = color;
		}

		private void SetUpBuffers()
		{
			VAO = GL.GenVertexArray();
			//IBO = GL.GenBuffer();
			vbo = GL.GenBuffer();
			ubo = GL.GenBuffer();

			/*GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
			GL.BufferData(BufferTarget.ElementArrayBuffer, textMeshData.getVertexCount() * sizeof(int), _indices, BufferUsageHint.StaticDraw);*/

			GL.BindVertexArray(VAO);

			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.BufferData(BufferTarget.ArrayBuffer, textMeshData.getVertexLength() * Vector2.SizeInBytes, textMeshData.getVertexPositions(), BufferUsageHint.DynamicDraw);
			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);
			GL.EnableVertexAttribArray(0);

			GL.BindBuffer(BufferTarget.ArrayBuffer, ubo);
			GL.BufferData(BufferTarget.ArrayBuffer, textMeshData.getTextureCoordsLength() * Vector2.SizeInBytes, textMeshData.getTextureCoords(), BufferUsageHint.DynamicDraw);
			GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
			GL.EnableVertexAttribArray(1);

			Ready = true;
		}

		private TextMeshData createTextMesh()
		{
			List<Line> lines = createStructure();
			TextMeshData data = createQuadVertices(lines);
			return data;
		}

		private List<Line> createStructure()
		{
			char[] chars = textString.ToCharArray();
			List<Line> lines = new List<Line>();
			Line currentLine = new Line(font.getSpaceWidth(), fontSize, lineMaxSize);
			Word currentWord = new Word(fontSize);

			for (int i = 0; i < chars.Length; i++)
			{
				int ascii = (int)chars[i];
				if (ascii == FontType.SPACE_ASCII)
				{
					bool added = currentLine.attemptToAddWord(currentWord);
					if (!added)
					{
						lines.Add(currentLine);
						currentLine = new Line(font.getSpaceWidth(), fontSize, lineMaxSize);
						currentLine.attemptToAddWord(currentWord);
					}
					currentWord = new Word(fontSize);
					continue;
				}
				Character character = font.getCharacter(ascii);
				currentWord.addCharacter(character);
			}

			completeStructure(lines, currentLine, currentWord);
			return lines;
		}

		private void completeStructure(List<Line> lines, Line currentLine, Word currentWord)
		{
			bool added = currentLine.attemptToAddWord(currentWord);

			if (!added)
			{
				lines.Add(currentLine);
				currentLine = new Line(font.getSpaceWidth(), fontSize, lineMaxSize);
				currentLine.attemptToAddWord(currentWord);
			}
			lines.Add(currentLine);
		}

		private TextMeshData createQuadVertices(List<Line> lines)
		{
			numberOfLines = lines.Count;
			double curserX = 0f;
			double curserY = 0f;

			List<Vector2> vertices = new List<Vector2>();
			List<Vector2> textureCoords = new List<Vector2>();

			foreach (var line in lines)
			{
				switch (textAling)
				{
					case TextAling.Center:
						curserX = (font.getSpaceWidth() * fontSize) - (line.getLineLength() / 2f);
						curserY = (FontType.LINE_HEIGHT * fontSize) / 2f;
						break;
					case TextAling.Left:
						curserX = (font.getSpaceWidth() * fontSize) / 2f;
						curserY = (FontType.LINE_HEIGHT * fontSize) / 2f;
						break;
					case TextAling.Right:
						curserX = (font.getSpaceWidth() * fontSize) / 2f + (line.getLineLength() / line.getMaxLength());
						break;
					case TextAling.Top:
						break;
					case TextAling.Bottom:
						break;
				}

				foreach (var word in line.getWords())
				{
					foreach (var letter in word.getCharacters())
					{
						addVerticesForCharacter(curserX, curserY, letter, fontSize, vertices);
						addTexCoords(textureCoords, letter.getxTextureCoord(), letter.getyTextureCoord(), letter.getXMaxTextureCoord(), letter.getYMaxTextureCoord());
						curserX += letter.getxAdvance() * fontSize * 1.3f;
					}
					curserX += font.getSpaceWidth() * fontSize * 1.3f;
				}
				curserX = 0;
				curserY += FontType.LINE_HEIGHT * fontSize * 1.3f;
			}

			return new TextMeshData(vertices.ToArray(), textureCoords.ToArray());
		}

		private void addVerticesForCharacter(double curserX, double curserY, Character character, double fontSize, List<Vector2> vertices)
		{
			double x = curserX + (character.getxOffset() * fontSize);
			double y = curserY - (character.getyOffset() * fontSize);
			double maxX = x + (character.getSizeX() * fontSize);
			double maxY = y - (character.getSizeY() * fontSize);
			double properX = (2 * x);
			double properY = (-2 * y);
			double properMaxX = (2 * maxX);
			double properMaxY = (-2 * maxY);
			addVertices(vertices, properX * fontSize, properY * fontSize / 1.5f, properMaxX * fontSize, properMaxY * fontSize / 1.5f);
		}

		private void addVertices(List<Vector2> vertices, double x, double y, double maxX, double maxY)
		{
			/*vertices.Add(new Vector2((float)x, (float)y));
			vertices.Add(new Vector2((float)x, (float)maxY));
			vertices.Add(new Vector2((float)maxX, (float)maxY));
			vertices.Add(new Vector2((float)maxX, (float)maxY));
			vertices.Add(new Vector2((float)maxX, (float)y));
			vertices.Add(new Vector2((float)x, (float)y));*/


			vertices.Add(new Vector2((float)x + (float)_FinalPos.X, (float)y + (float)_FinalPos.Y));
			vertices.Add(new Vector2((float)x + (float)_FinalPos.X, (float)maxY + (float)_FinalPos.Y));
			vertices.Add(new Vector2((float)maxX + (float)_FinalPos.X, (float)maxY + (float)_FinalPos.Y));
			vertices.Add(new Vector2((float)maxX + (float)_FinalPos.X, (float)maxY + (float)_FinalPos.Y));
			vertices.Add(new Vector2((float)maxX + (float)_FinalPos.X, (float)y + (float)_FinalPos.Y));
			vertices.Add(new Vector2((float)x + (float)_FinalPos.X, (float)y + (float)_FinalPos.Y));


			/*vertices.Add(new Vector2((float)x + (_FinalRectangle.X + (_FinalRectangle.Width / 2f)) + (lineMaxSize / textString.Length / 2f) - fontSize, (float)y + (_FinalRectangle.Y + (_FinalRectangle.Height / 2f)) - fontSize));
			vertices.Add(new Vector2((float)x + (_FinalRectangle.X + (_FinalRectangle.Width / 2f)) + (lineMaxSize / textString.Length / 2f) - fontSize, (float)maxY + (_FinalRectangle.Y + (_FinalRectangle.Height / 2f)) - fontSize));
			vertices.Add(new Vector2((float)maxX + (_FinalRectangle.X + (_FinalRectangle.Width / 2f)) + (lineMaxSize / textString.Length / 2f) - fontSize, (float)maxY + (_FinalRectangle.Y + (_FinalRectangle.Height / 2f)) - fontSize));
			vertices.Add(new Vector2((float)maxX + (_FinalRectangle.X + (_FinalRectangle.Width / 2f)) + (lineMaxSize / textString.Length / 2f) - fontSize, (float)maxY + (_FinalRectangle.Y + (_FinalRectangle.Height / 2f)) - fontSize));
			vertices.Add(new Vector2((float)maxX + (_FinalRectangle.X + (_FinalRectangle.Width / 2f)) + (lineMaxSize / textString.Length / 2f) - fontSize, (float)y + (_FinalRectangle.Y + (_FinalRectangle.Height / 2f)) - fontSize));
			vertices.Add(new Vector2((float)x + (_FinalRectangle.X + (_FinalRectangle.Width / 2f)) + (lineMaxSize / textString.Length / 2f) - fontSize, (float)y + (_FinalRectangle.Y + (_FinalRectangle.Height / 2f)) - fontSize));
			*/

			/*
			vertices.Add(new Vector2((float)x + (_FinalRectangle.X	+	(_FinalRectangle.Width / 2) + (lineMaxSize / textString.Length / 1.5f)),		(float)y + (_FinalRectangle.Y + (_FinalRectangle.Height / 2) * 2 + 1)));
			vertices.Add(new Vector2((float)x + (_FinalRectangle.X	+	(_FinalRectangle.Width / 2) + (lineMaxSize / textString.Length / 1.5f)),		(float)maxY + (_FinalRectangle.Y + (_FinalRectangle.Height / 2) * 2 + 1)));
			vertices.Add(new Vector2((float)maxX + (_FinalRectangle.X + (_FinalRectangle.Width / 2) + (lineMaxSize / textString.Length / 1.5f)),	(float)maxY + (_FinalRectangle.Y + (_FinalRectangle.Height / 2) * 2 + 1)));
			vertices.Add(new Vector2((float)maxX + (_FinalRectangle.X + (_FinalRectangle.Width / 2) + (lineMaxSize / textString.Length / 1.5f)),	(float)maxY + (_FinalRectangle.Y + (_FinalRectangle.Height / 2) * 2 + 1)));
			vertices.Add(new Vector2((float)maxX + (_FinalRectangle.X + (_FinalRectangle.Width / 2) + (lineMaxSize / textString.Length / 1.5f)),	(float)y + (_FinalRectangle.Y + (_FinalRectangle.Height / 2) * 2 + 1)));
			vertices.Add(new Vector2((float)x + (_FinalRectangle.X +	(_FinalRectangle.Width / 2) + (lineMaxSize / textString.Length / 1.5f)),		(float)y + (_FinalRectangle.Y + (_FinalRectangle.Height / 2) * 2 + 1)));
			*/


			/*Debug.Log("Vert: " + vertices[(vertices.Count - 1) - 5]);
			Debug.Log("Vert: " + vertices[(vertices.Count - 1) - 4]);
			Debug.Log("Vert: " + vertices[(vertices.Count - 1) - 3]);
			Debug.Log("Vert: " + vertices[(vertices.Count - 1) - 2]);
			Debug.Log("Vert: " + vertices[(vertices.Count - 1) - 1]);
			Debug.Log("Vert: " + vertices[(vertices.Count - 1)]);*/
		}

		private void addTexCoords(List<Vector2> texCoords, double x, double y, double maxX, double maxY)
		{
			texCoords.Add(new Vector2((float)x, (float)y));
			texCoords.Add(new Vector2((float)x, (float)maxY));
			texCoords.Add(new Vector2((float)maxX, (float)maxY));
			texCoords.Add(new Vector2((float)maxX, (float)maxY));
			texCoords.Add(new Vector2((float)maxX, (float)y));
			texCoords.Add(new Vector2((float)x, (float)y));
		}

		private void GenRectangle()
		{
			_FinalPos.X = _ParentRectangle.X;
			_FinalPos.Y = _ParentRectangle.Y;

			switch (textAling)
			{
				case TextAling.Center:
					_FinalPos.X = _FinalPos.X + (_ParentRectangle.Width / 2.2f) - textString.Length;
					_FinalPos.Y = _ParentRectangle.Y + (_ParentRectangle.Height / 2f);
					break;
				case TextAling.Left:
					_FinalPos.Y = _ParentRectangle.Y + (_ParentRectangle.Height / 2f);
					break;
				case TextAling.Right:
					break;
				case TextAling.Top:
					break;
				case TextAling.Bottom:
					break;
			}
		}

		public void Dispose()
		{
			textMeshData = null;

			/*GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			GL.BindVertexArray(0);*/

			GL.BindVertexArray(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.DisableVertexAttribArray(0);

			GL.BindVertexArray(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.DisableVertexAttribArray(0);

			//GL.DeleteBuffer(IBO);
			GL.DeleteBuffer(vbo);
			GL.DeleteBuffer(ubo);
			GL.DeleteVertexArray(VAO);
		}
	}

	public class Line
	{
		private double maxLength;
		private double spaceSize;

		private List<Word> words = new List<Word>();
		private double currentLineLength = 0;

		public Line(double spaceWidth, double fontSize, double maxLength)
		{
			this.spaceSize = spaceWidth * fontSize;
			this.maxLength = maxLength;
		}

		public bool attemptToAddWord(Word word)
		{
			double additionalLength = word.getWordWidth();

			if (words.Count == 0)
			{
				additionalLength += 0;
			}
			else
			{
				additionalLength += spaceSize;
			}

			if (currentLineLength + additionalLength <= maxLength)
			{
				words.Add(word);
				currentLineLength += additionalLength;
				return true;
			}
			else
			{
				return false;
			}
		}

		public double getMaxLength()
		{
			return maxLength;
		}

		public double getLineLength()
		{
			return currentLineLength;
		}

		public List<Word> getWords()
		{
			return words;
		}

	}

	public class Word
	{
		private List<Character> characters = new List<Character>();
		private double width = 0;
		private double fontSize;

		public Word(double fontSize)
		{
			this.fontSize = fontSize;
		}

		public void addCharacter(Character character)
		{
			characters.Add(character);
			width += character.getxAdvance() * fontSize;
		}

		public List<Character> getCharacters()
		{
			return characters;
		}

		public double getWordWidth()
		{
			return width;
		}
	}

	public class TextMeshData
	{

		private Vector2[] vertexPositions;
		private Vector2[] textureCoords;

		public TextMeshData(Vector2[] vertexPositions, Vector2[] textureCoords)
		{
			this.vertexPositions = vertexPositions;
			this.textureCoords = textureCoords;
		}

		public Vector2[] getVertexPositions()
		{
			return vertexPositions;
		}

		public Vector2[] getTextureCoords()
		{
			return textureCoords;
		}

		public int getVertexCount()
		{
			return vertexPositions.Length / 2;
		}

		public int getVertexLength()
		{
			return vertexPositions.Length;
		}

		public int getTextureCoordsLength()
		{
			return textureCoords.Length;
		}

	}

	public enum TextAling : byte
	{
		Center, Left, Right, Top, Bottom
	}
}
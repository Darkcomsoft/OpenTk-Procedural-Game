using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using QuickFont;
using QuickFont.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public class UIManager
    {
        QFontDrawing _drawing;
        QFont _myFont;
        QFontRenderOptions RendeTextOption;

        public string Text;

        public UIManager()
        {
            _drawing = new QFontDrawing();
            _myFont = new QFont("OpenSans.ttf", 10 / (Game.Instance.Width / Game.Instance.Height), new QFontBuilderConfiguration(true));
            RendeTextOption = new QFontRenderOptions {
                WordWrap = true,
                Colour = Color.White, DropShadowActive = false,
                CharacterSpacing = 0.1f
            };

            /*
             * 
             * _drawing.DrawingPrimitives.Clear();
            _drawing.Print(_myFont, "text1", new OpenTK.Vector3(Engine.Instance.Width / 2, Engine.Instance.Height / 2, 0), QFontAlignment.Centre);
             * // draw with options
            var textOpts = new QFontRenderOptions()
            {
                Colour = Color.FromArgb(new Color4(0.8f, 0.1f, 0.1f, 1.0f).ToArgb()),
                DropShadowActive = true
            };
            SizeF size = _drawing.Print(_myFont, "text2", pos2, FontAlignment.Left, textOpts);

            var dp = new QFontDrawingPimitive(_myFont2);
            size = dp.Print(text, new Vector3(bounds.X, Height - yOffset, 0), new SizeF(maxWidth, float.MaxValue), alignment);
            drawing.DrawingPimitiveses.Add(dp);*/

            // after all changes do update buffer data and extend it's size if needed.
            //_drawing.RefreshBuffers();
        }

        public void DrawUI()
        {
            if (Game.Instance != null)
            {
                if (_drawing != null)
                {
                    if (_myFont != null)
                    {
                        if (RendeTextOption != null)
                        {
                            _drawing.DrawingPrimitives.Clear();
                            _drawing.ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(Game.Instance.ClientRectangle.X, Game.Instance.ClientRectangle.Width, Game.Instance.ClientRectangle.Y, Game.Instance.ClientRectangle.Height, 0f, 5.0f);
                            _drawing.Print(_myFont, Text, new Vector3(Game.Instance.Width / 2, Game.Instance.ClientRectangle.Bottom - 25, 0), new SizeF(Game.Instance.ClientRectangle.Width, Game.Instance.ClientRectangle.Height), QFontAlignment.Centre, RendeTextOption);
                            _drawing.RefreshBuffers();
                            _drawing.Draw();
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            _drawing.Dispose();
            _myFont.Dispose();
        }
    }
}

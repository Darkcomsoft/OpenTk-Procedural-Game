using ProjectEvlly.src.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickFont;
using QuickFont.Configuration;
using EvllyEngine;
using OpenTK;

namespace ProjectEvlly.src.UI
{
    public class GUITextInput : GUIBase
    {
        public string Value = "";
        public string PlacerHolder = "A Input";

        public string FontName = "OpenSans";

        private QFontDrawing _drawing;
        private QFontRenderOptions RendeTextOption;

        private Vector3 Pos;
        private SizeF MaxSize;

        public GUITextInput(Rectangle rec) : base(rec)
        {
            Start();
        }

        public GUITextInput(Rectangle rec, UIDock uIDock) : base(rec, uIDock)
        {
            Start();
        }

        private void Start()
        {
            _drawing = new QFontDrawing();
            RendeTextOption = new QFontRenderOptions
            {
                WordWrap = true,
                Colour = Color.Black,
                DropShadowActive = false,
                ClippingRectangle = GetRectangle
            };

            Pos = new Vector3(GetRectangle.X, GetRectangle.Y + (GetRectangle.Height / 2), 0);
            MaxSize = new SizeF(GetRectangle.Width, GetRectangle.Height);
        }

        public override void OnKeyPress(char KeyChar)
        {
            if (IsFocused)
            {
                if (Input.GetKeyDown(OpenTK.Input.Key.BackSpace))
                {
                    Value = "";
                }
                else
                {
                    Value += KeyChar;
                }
            }
            base.OnKeyPress(KeyChar);
        }

        public override void OnResize()
        {
            Pos = new Vector3(GetRectangle.X, GetRectangle.Y + (GetRectangle.Height / 2), 0);
            MaxSize = new SizeF(GetRectangle.Width, GetRectangle.Height);
            base.OnResize();
        }

        public override void RenderAfter()
        {
            _drawing.DrawingPrimitives.Clear();
            _drawing.ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(Window.Instance.ClientRectangle.X, Window.Instance.ClientRectangle.Width, Window.Instance.ClientRectangle.Y, Window.Instance.ClientRectangle.Height, 0f, 5.0f);

            RendeTextOption.ClippingRectangle = GetRectangle;
            

            if (Value.Equals(string.Empty))
            {
                if (Time._Time % 60 <= 20 && IsFocused)
                {
                    _drawing.Print(AssetsManager.GetFont(FontName), PlacerHolder + "|", Pos, MaxSize, QFontAlignment.Justify, RendeTextOption);

                }
                else
                {
                    _drawing.Print(AssetsManager.GetFont(FontName), PlacerHolder + " ", Pos, MaxSize, QFontAlignment.Justify, RendeTextOption);
                }
            }
            else
            {
                if (Time._Time % 60 <= 20 && IsFocused)
                {
                    _drawing.Print(AssetsManager.GetFont(FontName), Value + "|", Pos, MaxSize, QFontAlignment.Justify, RendeTextOption);

                }
                else
                {
                    _drawing.Print(AssetsManager.GetFont(FontName), Value + " ", Pos, MaxSize, QFontAlignment.Justify, RendeTextOption);
                }
            }

            _drawing.RefreshBuffers();
            _drawing.Draw();
            base.RenderAfter();
        }

        public override void Dispose()
        {
            _drawing.Dispose();
            RendeTextOption = null;
            base.Dispose();
        }
    }
}

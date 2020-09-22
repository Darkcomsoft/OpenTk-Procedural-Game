using EvllyEngine;
using ProjectEvlly.src.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickFont;
using OpenTK;

namespace ProjectEvlly.src.UI.GUIElements
{
    public class GUIButtom : GUIBase
    {
        public string TextureName;

        public string FontName = "OpenSans";
        public string Text;
        public Color TextColor = Color.Black;

        private QFontDrawing _drawing;
        private QFontRenderOptions RendeTextOption;

        private Vector3 Pos;
        private SizeF MaxSize;

        public GUIButtom(Rectangle rec) : base(rec)
        {
            Start();
        }

        public GUIButtom(Rectangle rec, UIDock uIDock) : base(rec, uIDock)
        {
            Start();
        }

        private void Start()
        {
            _drawing = new QFontDrawing();
            RendeTextOption = new QFontRenderOptions
            {
                WordWrap = true,
                Colour = TextColor,
                DropShadowActive = false,
                //ClippingRectangle = GetRectangle
            };
            
            _drawing.ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(Window.Instance.ClientRectangle.X, Window.Instance.ClientRectangle.Width, Window.Instance.ClientRectangle.Y, Window.Instance.ClientRectangle.Height, 0f, 5.0f);

            Pos = new Vector3(GetRectangle.X + (GetRectangle.Width / 2), GetRectangle.Y + (GetRectangle.Height / 2), 0);
            MaxSize = new SizeF(GetRectangle.Width, GetRectangle.Height);
        }

        public override void OnResize()
        {
            Pos = new Vector3(GetRectangle.X + (GetRectangle.Width / 2), GetRectangle.Y + (GetRectangle.Height / 2), 0);
            MaxSize = new SizeF(GetRectangle.Width, GetRectangle.Height);

            _drawing.ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(Window.Instance.ClientRectangle.X, Window.Instance.ClientRectangle.Width, Window.Instance.ClientRectangle.Y, Window.Instance.ClientRectangle.Height, 0f, 5.0f);

            //RendeTextOption.ClippingRectangle = GetRectangle;
            base.OnResize();
        }

        public override void RenderAfter()
        {
            _drawing.DrawingPrimitives.Clear();

            _drawing.Print(AssetsManager.GetFont(FontName), Text, Pos, MaxSize, QFontAlignment.Centre, RendeTextOption);

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

        public override void RenderCustomValues()
        {
            if (TextureName != null)
            {
                if (!TextureName.Equals(string.Empty))
                {
                    AssetsManager.UseTexture(TextureName);
                    GUIRender.GetShader.Setbool("HaveTexture", true);
                }
                else
                {
                    GUIRender.GetShader.Setbool("HaveTexture", false);
                }
            }
            base.RenderCustomValues();
        }

        public override void Click()
        {
            base.Click();
        }
    }
}

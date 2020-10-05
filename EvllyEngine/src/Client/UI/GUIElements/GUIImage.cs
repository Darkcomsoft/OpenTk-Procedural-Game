using EvllyEngine;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.UI
{
    public class GUIImage : GUIBase
    {
        public string TextureName = "";

        public GUIImage(Rectangle rec) : base(rec)
        {
            TextureName = "";
            NoInteractable();
            HideFocus();
        }

        public GUIImage(Rectangle rec, UIDock uIDock) : base(rec, uIDock)
        {
            TextureName = "";
            NoInteractable();
            HideFocus();
        }


        public GUIImage(Rectangle rec, string Image) : base(rec)
        {
            TextureName = Image;
            NoInteractable();
            HideFocus();
        }

        public GUIImage(Rectangle rec, UIDock uIDock, string Image) : base(rec, uIDock)
        {
            TextureName = Image;
            NoInteractable();
            HideFocus();
        }

        public GUIImage(Rectangle rec, UIDock uIDock, string Image, Color4 nColor, Color4 hColor, Color4 cColor, Color4 fColor) : base(rec, uIDock, nColor, hColor, cColor, fColor)
        {
            TextureName = Image;
            NoInteractable();
            HideFocus();
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
    }
}
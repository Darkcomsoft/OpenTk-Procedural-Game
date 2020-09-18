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
        public Texture ImageRaw;
        public Vector4 Color;

        public GUIImage(Rectangle rec) : base(rec)
        {
            ImageRaw = null;
            Color = new Vector4(1, 1, 1, 0.5f);
        }

        public GUIImage(Rectangle rec, UIDock uIDock) : base(rec, uIDock)
        {
            ImageRaw = null;
            Color = new Vector4(1, 1, 1, 0.5f);
        }


        public GUIImage(Rectangle rec, Texture Image) : base(rec)
        {
            ImageRaw = Image;
            Color = new Vector4(1, 1, 1, 0.5f);
        }

        public GUIImage(Rectangle rec, UIDock uIDock, Texture Image) : base(rec, uIDock)
        {
            ImageRaw = Image;
            Color = new Vector4(1, 1, 1, 0.5f);
        }


        public GUIImage(Rectangle rec, Texture Image, Color4 color) : base(rec)
        {
            ImageRaw = Image;
            Color = new Vector4(color.R, color.G, color.B, color.A);
        }

        public GUIImage(Rectangle rec, UIDock uIDock, Texture Image, Color4 color) : base(rec, uIDock)
        {
            ImageRaw = Image;
            Color = new Vector4(color.R, color.G, color.B, color.A);
        }
    }
}
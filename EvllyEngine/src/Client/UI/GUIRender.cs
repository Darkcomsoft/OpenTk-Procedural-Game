using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using EvllyEngine;
using System.Drawing;
using OpenTK.Input;
using ProjectEvlly.src.Utility;

namespace ProjectEvlly.src.UI
{
    public class GUIRender : IDisposable
    {
        public static GUIRender Instance; 

        private Shader _Shader;

        private List<GUIBase> GuiBaseList;

        public int GUISize = 1;

        private GUIBase UpUI;
        private GUIBase FocusedUI;
        private bool pressed;

        public GUIRender()
        {
            Instance = this;

            GuiBaseList = new List<GUIBase>();

            _Shader = AssetsManager.GetShader("GUI");

            /*GuiBaseList.Add(new GUIImage(new Rectangle(0, 0, 150, 50), UIDock.SizeBottom, null, Color4.Indigo, Color4.IndianRed, Color4.Wheat));
            GuiBaseList.Add(new GUIImage(new Rectangle(0, 0, 150, 50), UIDock.SizeTop, null, Color4.Green, Color4.IndianRed, Color4.Wheat));
            GuiBaseList.Add(new GUIImage(new Rectangle(0, 0, 50, 150), UIDock.SizeLeft, null, Color4.Blue, Color4.IndianRed, Color4.Wheat));
            GuiBaseList.Add(new GUIImage(new Rectangle(0, 0, 50, 150), UIDock.SizeRight, null, Color4.Gold, Color4.IndianRed, Color4.Wheat));
            

            //GuiBaseList.Add(new GUIImage(new Rectangle(0, 0, 50, 50), UIDock.Cennter));
            GuiBaseList.Add(new GUIImage(new Rectangle(0, 0, 200, 200), UIDock.CenterTop, "devTexture2"));
            GuiBaseList.Add(new GUIImage(new Rectangle(0, 0, 50, 50), UIDock.CenterBottom));
            GuiBaseList.Add(new GUIImage(new Rectangle(0, 0, 50, 50), UIDock.CenterLeft));
            GuiBaseList.Add(new GUIImage(new Rectangle(0, 0, 50, 50), UIDock.CenterRight));
            GuiBaseList.Add(new GUIImage(new Rectangle(0, 0, 50, 50), UIDock.TopLeft));
            GuiBaseList.Add(new GUIImage(new Rectangle(0, 0, 50, 50), UIDock.TopRight));
            GuiBaseList.Add(new GUIImage(new Rectangle(0, 0, 50, 50), UIDock.BottomLeft));
            GuiBaseList.Add(new GUIImage(new Rectangle(0, 0, 50, 50), UIDock.BottomRight));

            GuiBaseList.Add(new GUITextInput(new Rectangle(0, 0, 150, 50), UIDock.Cennter));*/
        }

        public void Tick()
        {
            if (pressed)
            {
                if (Mouse.GetCursorState().LeftButton == ButtonState.Released)
                {
                    if (UpUI != null)
                    {
                        UpUI.ClickReleased();
                    }
                    pressed = false;
                }
            }
            else
            {
                if (Mouse.GetCursorState().LeftButton == ButtonState.Pressed)
                {
                    if (UpUI != null)
                    {
                        if (UpUI.IsHover)
                        {
                            if (FocusedUI != null)
                            {
                                FocusedUI.UnFocus();
                            }

                            UpUI.Click();
                            FocusedUI = UpUI;

                            FocusedUI.Focus();
                        }
                        else
                        {
                            if (FocusedUI != null)
                            {
                                FocusedUI.UnFocus();
                            }
                        }
                    }
                    pressed = true;
                }
            }

            if (Input.GetKeyDown(Key.Escape) || Input.GetKeyDown(Key.Enter) || Input.GetKeyDown(Key.KeypadEnter))
            {
                if (FocusedUI != null)
                {
                    FocusedUI.UnFocus();
                }
            }

            for (int i = 0; i < GuiBaseList.Count; i++)
            {
                GuiBaseList[i].Tick();
            }
        }

        public void TickRender()
        {
            for (int i = 0; i < GuiBaseList.Count; i++)
            {
                GuiBaseList[i].TickRender();
            }
        }

        public void OnResize()
        {
            for (int i = 0; i < GuiBaseList.Count; i++)
            {
                for (int v = 0; v < 2; v++)//Check the size changes twice
                {
                    GuiBaseList[i].OnResize();
                }
            }
        }

        public void OnMouseMove(MouseMoveEventArgs e)
        {
            if (!EvllyEngine.MouseCursor.MouseLocked)
            {
                Point point = new Point(e.X, e.Y);

                foreach (var item in GuiBaseList)
                {
                    if (item.IsEnabled)
                    {
                        if (item.GetRectangle.Contains(point))
                        {
                            UpUI = item;

                            item.UnHover();
                            //item.UnFocus();
                        }
                        else
                        {
                            item.UnHover();
                            //item.UnFocus();
                        }
                    }
                }

                if (UpUI != null)
                {
                    if (UpUI.GetRectangle.Contains(point))
                    {
                        UpUI.Hover();
                    }
                    else
                    {
                        UpUI.UnHover();
                    }
                }
            }
        }

        public void OnKeyPress(KeyPressEventArgs e)
        {
            if (!EvllyEngine.MouseCursor.MouseLocked)
            {
                if (FocusedUI != null)
                {
                    FocusedUI.OnKeyPress(e.KeyChar);
                }
            }
        }

        public virtual void Dispose()
        {
            foreach (var item in GuiBaseList)
            {
                item.Dispose();
            }

            GuiBaseList.Clear();
        }

        public static void AddGuiElement(GUIBase baseGui)
        {
            GUIRender.Instance.GuiBaseList.Add(baseGui);
        }

        public static void RemoveGuiElement(GUIBase baseGui)
        {
            GUIRender.Instance.GuiBaseList.Remove(baseGui);
        }

        public static Shader GetShader { get { return GUIRender.Instance._Shader; } }
    }

    public enum UIDock : byte
    {
        Free,
        Cennter, CenterTop, CenterBottom, CenterLeft, CenterRight,
        TopLeft, TopRight,
        BottomLeft, BottomRight,

        SizeBottom, SizeTop, SizeLeft, SizeRight//this is for dock position, but resize the ui element
    }

    public enum TextureType
    {
        Empty, AssetTexture, BitMapTextures
    }

    public struct UIRectangle
    {
        public float x;
        public float y;
        public float w;
        public float h;

        public UIRectangle(float x, float y, float w, float h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        public bool In(Rectangle rec, Point Point)
        {
            return Point.X >= rec.Location.X && rec.Y >= rec.Location.Y &&
                Point.X < rec.Location.X + rec.Size.Width && Point.Y < rec.Location.Y + rec.Size.Height;
        }
    }
}

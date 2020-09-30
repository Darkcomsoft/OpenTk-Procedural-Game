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
        private Shader _FontShader;

        private List<GUIBase> GuiBaseList;

        public static float GUISize = 2;

        private GUIBase UpUI;
        private GUIBase FocusedUI;
        private bool pressed;

        public GUIRender()
        {
            Instance = this;

            GuiBaseList = new List<GUIBase>();

            _Shader = AssetsManager.GetShader("GUI");
            _FontShader = AssetsManager.GetShader("Font");
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

            /*for (int i = 0; i < GuiBaseList.Count; i++)
            {
                GuiBaseList[i].Tick();
            }*/
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
                    if (item.EnabledInput)
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
                    else
                    {
                        item.UnHover();
                        item.UnFocus();
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
        public static Shader GetFontShader { get { return GUIRender.Instance._FontShader; } }
    }

    public enum UIDock : byte
    {
        Free,
        Cennter, CenterTop, CenterBottom, CenterLeft, CenterRight,
        TopLeft, TopRight,
        BottomLeft, BottomRight,

        SizeBottom, SizeTop, SizeLeft, SizeRight,//this is for dock position, but resize the ui element
        ScreenSize, ScreenSizeRatio
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

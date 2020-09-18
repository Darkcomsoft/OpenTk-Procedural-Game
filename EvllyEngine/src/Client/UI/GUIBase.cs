using EvllyEngine;
using QuickFont;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using ProjectEvlly.src.Utility;
using System.Drawing;

namespace ProjectEvlly.src.UI
{
    public abstract class GUIBase : IDisposable
    {
        private bool Enabled;
        public Rectangle _Rectangle;
        private Rectangle _FinalRectangle;
        private UIDock _Dock;

        public GUIBase(Rectangle rec)
        {
            _Rectangle = rec;
            Enabled = true;
        }

        public GUIBase(Rectangle rec, UIDock uIDock)
        {
            _Rectangle = rec;
            _Dock = uIDock;
            Enabled = true;
        }

        public virtual void Tick()
        {
            _FinalRectangle = _Rectangle;

            switch (_Dock)
            {
                case UIDock.Cennter:
                    _FinalRectangle = _Rectangle;
                    break;
                case UIDock.CenterTop:
                    _FinalRectangle.X = _Rectangle.X;
                    _FinalRectangle.Y = Window.Instance.Height + _Rectangle.Y;
                    break;
                case UIDock.CenterBottom:
                    _FinalRectangle.X = _Rectangle.X;
                    _FinalRectangle.Y = -Window.Instance.Height - _Rectangle.Y;
                    break;
                case UIDock.CenterLeft:
                    _FinalRectangle.X = -Window.Instance.Width - _Rectangle.X;
                    _FinalRectangle.Y = _Rectangle.Y;
                    break;
                case UIDock.CenterRight:
                    _FinalRectangle.X = Window.Instance.Width + _Rectangle.X;
                    _FinalRectangle.Y = _Rectangle.Y;
                    break;
                case UIDock.TopLeft:
                    _FinalRectangle.X = -Window.Instance.Width - _Rectangle.X;
                    _FinalRectangle.Y = Window.Instance.Height + _Rectangle.Y;
                    break;
                case UIDock.TopRight:
                    _FinalRectangle.X = Window.Instance.Width + _Rectangle.X;
                    _FinalRectangle.Y = Window.Instance.Height + _Rectangle.Y;
                    break;
                case UIDock.BottomLeft:
                    _FinalRectangle.X = -Window.Instance.Width - _Rectangle.X;
                    _FinalRectangle.Y = -Window.Instance.Height - _Rectangle.Y;
                    break;
                case UIDock.BottomRight:
                    _FinalRectangle.X = Window.Instance.Width + _Rectangle.X;
                    _FinalRectangle.Y = -Window.Instance.Height - _Rectangle.Y;
                    break;



                case UIDock.SizeBottom:
                    _FinalRectangle.X = _Rectangle.X;
                    _FinalRectangle.Y = -Window.Instance.Height - _Rectangle.Y;

                    _FinalRectangle.Width = Window.Instance.Width - _Rectangle.Width;
                    break;
                case UIDock.SizeTop:
                    _FinalRectangle.X = _Rectangle.X;
                    _FinalRectangle.Y = Window.Instance.Height + _Rectangle.Y;

                    _FinalRectangle.Width = Window.Instance.Width + _Rectangle.Width;
                    break;
                case UIDock.SizeLeft:
                    _FinalRectangle.X = -Window.Instance.Width - _Rectangle.X;
                    _FinalRectangle.Y = _Rectangle.Y;

                    _FinalRectangle.Height = Window.Instance.Height - _Rectangle.Height;
                    break;
                case UIDock.SizeRight:
                    _FinalRectangle.X = Window.Instance.Width + _Rectangle.X;
                    _FinalRectangle.Y = _Rectangle.Y;

                    _FinalRectangle.Height = Window.Instance.Height - _Rectangle.Height;
                    break;
            }
        }

        public virtual void Enable()
        {
            Enabled = true;
        }

        public virtual void Disable()
        {
            Enabled = false;
        }

        public virtual void Dock(UIDock uIDock)
        {
            _Dock = uIDock;
        }

        public void Dispose()
        {

        }

        public Rectangle GetRectangle { get { return _FinalRectangle; } }
        public bool IsEnabled { get { return Enabled; } }
    }
}
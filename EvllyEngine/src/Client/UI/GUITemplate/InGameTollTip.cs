using EvllyEngine;
using OpenTK.Input;
using ProjectEvlly.src.UI.GUIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.UI.GUITemplate
{
    public class InGameTollTip : IDisposable
    {
        private GUILable texttest;

        public InGameTollTip()
        {
            texttest = new GUILable("TollTip: ", new System.Drawing.Rectangle(Window.Instance.Width / 2, Window.Instance.Height / 2, 100,50), UIDock.Free);
            texttest.Disable();
        }

        public void Enable()
        {
            MouseCursor.UnLockCursor();
            texttest.Enable();
        }

        public void Disable()
        {
            MouseCursor.LockCursor();
            texttest.Disable();
        }

        public void Dispose()
        {
            texttest.Dispose();
        }
    }
}

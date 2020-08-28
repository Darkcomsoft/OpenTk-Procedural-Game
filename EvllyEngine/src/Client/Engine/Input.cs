using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace EvllyEngine
{
    public class Input
    {
        private static List<Key> keyToggleList = new List<Key>();

        public static MouseMoveEventArgs GetMouse;
        public static int MouseWheelDelta;

        public static int GetMouseWheelDelta 
        { 
            get 
            {
                if (!Window.Instance.Focused) { return 0; }

                int f = MouseWheelDelta;
                MouseWheelDelta = 0;
                return f; 
            } 
        }

        public static bool GetKeyDown(Key key)
        {
            if (!Window.Instance.Focused) { return false; }

            if (!keyToggleList.Contains(key) && Keyboard.GetState().IsKeyDown(key))
            {
                keyToggleList.Add(key);
                return true;
            }
            else
            {
                if (Keyboard.GetState().IsKeyUp(key))
                {
                    keyToggleList.Remove(key);
                    return false;
                }
                return false;
            }
        }

        [Obsolete("Na verdade n esta implementado e n Obsoleto")]
        public static bool GetKeyUp(Key key)
        {
            if (!Window.Instance.Focused) { return false; }

            if (!keyToggleList.Contains(key) && Keyboard.GetState().IsKeyUp(key))
            {
                keyToggleList.Add(key);
                return true;
            }
            else
            {
                if (Keyboard.GetState().IsKeyUp(key))
                {
                    keyToggleList.Remove(key);
                    return false;
                }
                return false;
            }
        }

        public static bool GetKey(Key key)
        {
            if (!Window.Instance.Focused) { return false; }

            return Keyboard.GetState().IsKeyDown(key);
        }
    }
}
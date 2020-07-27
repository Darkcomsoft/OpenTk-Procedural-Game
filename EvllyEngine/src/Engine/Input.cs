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

        public static bool GetKeyDown(Key key)
        {
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

        [Obsolete("Na verdade n esta implementado e n Obisoleto")]
        public static bool GetKeyUp(Key key)
        {
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
            return Keyboard.GetState().IsKeyDown(key);
        }
    }
}
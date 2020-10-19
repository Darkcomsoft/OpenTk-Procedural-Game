﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace EvllyEngine
{
    /// <summary>
    /// All input calculation/system, mouse, keyboard, and more in near future...
    /// </summary>
    public static class Input
    {
        private static List<Key> keyToggleList = new List<Key>();
        private static List<MouseButton> mouseButtomToggleList = new List<MouseButton>();

        public static int ScrollWheelValue
        { 
            get 
            {
                if (!Window.Instance.Focused) { return 0; }
                return Mouse.GetState().ScrollWheelValue; 
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
        public static bool GetKeyDown(MouseButton buttom)
        {
            if (!Window.Instance.Focused) { return false; }

            if (!mouseButtomToggleList.Contains(buttom) && Mouse.GetState().IsButtonDown(buttom))
            {
                mouseButtomToggleList.Add(buttom);
                return true;
            }
            else
            {
                if (Mouse.GetState().IsButtonUp(buttom))
                {
                    mouseButtomToggleList.Remove(buttom);
                    return false;
                }
                return false;
            }
        }

        public static bool GetKeyUp(MouseButton buttom)
        {
            if (!Window.Instance.Focused) { return false; }

            if (!mouseButtomToggleList.Contains(buttom) && Mouse.GetState().IsButtonUp(buttom))
            {
                mouseButtomToggleList.Add(buttom);
                return true;
            }
            else
            {
                if (Mouse.GetState().IsButtonDown(buttom))
                {
                    mouseButtomToggleList.Remove(buttom);
                    return false;
                }
                return false;
            }
        }
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
                if (Keyboard.GetState().IsKeyDown(key))
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
        public static bool GetKey(MouseButton buttom)
        {
            if (!Window.Instance.Focused) { return false; }

            return Mouse.GetState().IsButtonDown(buttom);
        }
    }
}
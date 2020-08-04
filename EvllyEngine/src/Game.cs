using EvllyEngine;
using OpenTK;
using ProjectEvlly.src.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src
{
    /// <summary>
    /// This class is for the game logics, so every time you quit a server, or quit to mainmenu,
    /// game class is gone do erease all and start again, so this is not a static class or final class, is a dynamic class.
    /// </summary>
    public class Game
    {
        public static Game Instance;

        private Dictionary<string, Dimension> _Dimensions;
        private Queue<Dimension> _ToUnloadDimension;

        public Action<FrameEventArgs> DrawUpdate;
        public Action<FrameEventArgs> TransparentDrawUpdate;
        public Action<FrameEventArgs> UIDrawUpdate;

        public Game()
        {
            Instance = this;

            _Dimensions = new Dictionary<string, Dimension>();
            _ToUnloadDimension = new Queue<Dimension>();
        }

        public void Tick()
        {
            while (_ToUnloadDimension.Count > 0)
            {
                Dimension dime = _ToUnloadDimension.Dequeue();
                dime.OnUnloadDimension();
            }
        }

        public void Draw(FrameEventArgs e)
        {
            DrawUpdate.Invoke(e);//Draw all opaque objects
            Utilitys.CheckGLError("End Of DrawUpdate");
            TransparentDrawUpdate.Invoke(e);
            Utilitys.CheckGLError("End Of Transparent Draw");
        }

        public void Dispose()
        {
            if (_Dimensions.ContainsKey("VAK:MainGame"))
            {
                _ToUnloadDimension.Enqueue(_Dimensions["VAK:MainGame"]);
                _Dimensions.Remove("VAK:MainGame");
            }

            if (_Dimensions.ContainsKey("VAK:MidleWorld"))
            {
                _ToUnloadDimension.Enqueue(_Dimensions["VAK:MidleWorld"]);
                _Dimensions.Remove("VAK:MidleWorld");
            }

            while (_ToUnloadDimension.Count > 0)
            {
                Dimension dime = _ToUnloadDimension.Dequeue();
                dime.OnUnloadDimension();
            }

            _ToUnloadDimension.Clear();
            _Dimensions.Clear();
        }

        public void LoadMainMenu()
        {
            if (_Dimensions.ContainsKey("VAK:MidleWorld"))
            {
                _ToUnloadDimension.Enqueue(_Dimensions["VAK:MidleWorld"]);
                _Dimensions.Remove("VAK:MidleWorld");
            }

            _Dimensions.Add("VAK:MainGame", new MidleWorld("VAK:MainGame"));
        }

        public void LoadGameWorld()
        {
            if (_Dimensions.ContainsKey("VAK:MainGame"))
            {
                _ToUnloadDimension.Enqueue(_Dimensions["VAK:MainGame"]);
                _Dimensions.Remove("VAK:MainGame");
            }

            _Dimensions.Add("VAK:MidleWorld", new MidleWorld("VAK:MidleWorld"));
        }
    }
}

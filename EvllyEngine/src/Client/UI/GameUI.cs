using ProjectEvlly.src.UI.GUIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using EvllyEngine;
using ProjectEvlly.src.UI.GUITemplate;
using ProjectEvlly.src.save;
using OpenTK.Input;
using OpenTK;

namespace ProjectEvlly.src.UI
{
    public class GameUI : IDisposable
    {
        private GUILable VersionText;
        private GUILable debugText;

        private InGameTollTip inGameTollTip;

        private PrepareGUI _PrepareGUI;

        private Dictionary<string, InGuiGame> GuiList = new Dictionary<string, InGuiGame>();

        private bool Paused = false;
        private bool TollTip = false;

        public GameUI()
        {
            _PrepareGUI = new PrepareGUI(this);

            VersionText = new GUILable(GlobalData.AppName + " " + GlobalData.Version, new System.Drawing.Rectangle(5, 5, 200, 20), UIDock.TopLeft);
            VersionText.SetColor(Color4.White);
            VersionText.SetTextAling(Font.TextAling.Left);
            VersionText.ShowBackGround = false;

            debugText = new GUILable("Null", new System.Drawing.Rectangle(5, 30, 200, 20), UIDock.TopLeft);
            debugText.SetColor(Color4.White);
            debugText.SetTextAling(Font.TextAling.Left);
            debugText.ShowBackGround = false;

            inGameTollTip = new InGameTollTip();

            //Add Gui Instances here
            GuiList.Add("PauseMenu", new PauseMenu());
        }

        public void Tick()
        {
            debugText.SetText(string.Format("FPS:({0})  UPS:({1})  Tick%60:({2})  Time:({3})", Time.FPS, Time.UPS, Time._Tick, Time._Time));

            MenusInput();
        }

        public void Dispose()
        {
            VersionText.Dispose();
            debugText.Dispose();

            inGameTollTip.Dispose();

            _PrepareGUI.Dispose();

            foreach (var item in GuiList)
            {
                item.Value.Dispose();
            }

            GuiList.Clear();
        }

        public void SetCharData(CharSaveInfo[] charList)
        {
            _PrepareGUI.SetCharData(charList);
        }

        internal void PlayClick()
        {
            CloseAll();//Close all GuiMenu

            Game.GamePlay.PlayClick();
            _PrepareGUI.Dispose();
        }

        internal void DeleteClick()
        {
            Game.GamePlay.DeleteClick();
        }

        #region InGameGuiSystem

        private void MenusInput()
        {
            if (Input.GetKeyDown(MouseButton.Right))
            {
                if (TollTip)
                {
                    inGameTollTip.Disable(Input.GetMousePosition.X, Input.GetMousePosition.Y);
                    TollTip = false;
                }
                else
                {
                    inGameTollTip.Enable(Input.GetMousePosition.X, Input.GetMousePosition.Y);
                    TollTip = true;
                }
            }

            if (Input.GetKeyDown(OpenTK.Input.Key.Escape))
            {
                if (Paused)
                {
                    Paused = false;
                    CloseGui("PauseMenu");
                }
                else
                {
                    Paused = true;
                    OpenGuiCloseAll("PauseMenu");
                }
            }
        }

        public void OpenGui(string GuiName)
        {
            GuiList[GuiName].Open();
            EvllyEngine.MouseCursor.UnLockCursor();
        }

        public void CloseGui(string GuiName)
        {
            GuiList[GuiName].Close();
            EvllyEngine.MouseCursor.LockCursor();
        }

        public void OpenGuiCloseAll(string GuiName)
        {
            foreach (var item in GuiList)
            {
                item.Value.Close();
            }

            GuiList[GuiName].Open();
            EvllyEngine.MouseCursor.UnLockCursor();
        }

        public void CloseAll()
        {
            foreach (var item in GuiList)
            {
                item.Value.Close();
            }

            EvllyEngine.MouseCursor.LockCursor();
        }

        #endregion
    }

    public interface InGuiGame : IDisposable
    {
        /// <summary>
        /// Open the Gui
        /// </summary>
        void Open();

        /// <summary>
        /// Close the Gui
        /// </summary>
        void Close();
    }
}

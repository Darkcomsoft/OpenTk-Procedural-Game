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

namespace ProjectEvlly.src.UI
{
    public class GameUI : IDisposable
    {
        private GUILable VersionText;
        private GUILable debugText;

        private PrepareGUI _PrepareGUI;

        public GameUI()
        {
            _PrepareGUI = new PrepareGUI();

            VersionText = new GUILable(GlobalData.AppName + " " + GlobalData.Version, new System.Drawing.Rectangle(5, 5, 200, 20), UIDock.TopLeft);
            VersionText.SetColor(Color4.White);
            VersionText.SetTextAling(Font.TextAling.Left);
            VersionText.ShowBackGround = false;

            debugText = new GUILable("Null", new System.Drawing.Rectangle(5, 30, 200, 20), UIDock.TopLeft);
            debugText.SetColor(Color4.White);
            debugText.SetTextAling(Font.TextAling.Left);
            debugText.ShowBackGround = false;
        }

        public void Tick()
        {
            debugText.SetText("FPS:(" + Window.Instance.GetFPS + ") Tick:" + Time._Tick);
        }

        public void Dispose()
        {
            VersionText.Dispose();
            debugText.Dispose();

            _PrepareGUI.Dispose();
        }

        public void SetCharData(CharSaveInfo[] charList)
        {
            _PrepareGUI.SetCharData(charList);
        }
    }
}

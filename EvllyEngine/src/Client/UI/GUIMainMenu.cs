using EvllyEngine;
using Gwen.Control.Property;
using OpenTK.Graphics;
using ProjectEvlly.src.Net;
using ProjectEvlly.src.save;
using ProjectEvlly.src.UI.GUIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.UI
{
    public class GUIMainMenu : IDisposable
    {
        public Client _Client;
        private GUIButtom PlaySingleButtom;
        private GUIButtom PlayMultiButtom;
        private GUIButtom QuitButtom;

        private GUILable DarkcomsoftCopy;
        private GUILable VersionText;
        private GUILable debugText;

        private GUIImage Logo;
        private GUIImage imgteste;

        #region ConnectionScreen
        private GUIImage connectionBackgorund;
        private GUILable connectionTextInfo;
        #endregion

        public GUIMainMenu(Client cli)
        {
            _Client = cli;

            System.Random rand = new Random();

            imgteste = new GUIImage(new System.Drawing.Rectangle(0, 0, 0, 0), UIDock.ScreenSizeRatio);
            imgteste.TextureName = "BackGround";
            imgteste.EnabledInput = false;
            imgteste.SetBackColors(new Color4(102,70,36,255), Color4.White, Color4.White);

            Logo = new GUIImage(new System.Drawing.Rectangle(0, 135, 350, 50), UIDock.Cennter);
            Logo.TextureName = "VaKLogoYellow";
            Logo.EnabledInput = false;

            PlaySingleButtom = new GUIButtom("SinglePlayer", new System.Drawing.Rectangle(0, 55, 200, 50), UIDock.Cennter);
            PlaySingleButtom.SetColor(Color4.White);
            PlaySingleButtom.SetBackColors(Color4.White, Color4.Gray, Color4.Gray);
            PlaySingleButtom.TextureName = "Buttom";
            PlaySingleButtom.OnClick += PlayButtomClick;

            PlayMultiButtom = new GUIButtom("MultiPlayer", new System.Drawing.Rectangle(0, 0, 200, 50), UIDock.Cennter);
            PlayMultiButtom.SetColor(Color4.White);
            PlayMultiButtom.TextureName = "Buttom";
            PlayMultiButtom.OnClick += PlayMButtomClick;

            QuitButtom = new GUIButtom("Quit", new System.Drawing.Rectangle(0, -55, 200, 50), UIDock.Cennter);
            QuitButtom.SetColor(Color4.White);
            QuitButtom.TextureName = "Buttom";
            QuitButtom.OnClick += QuitButtomClick;

            VersionText = new GUILable(GlobalData.AppName + " " + GlobalData.Version, new System.Drawing.Rectangle(5, 5, 200, 20), UIDock.TopLeft);
            VersionText.SetColor(Color4.White);
            VersionText.SetTextAling(Font.TextAling.Left);
            VersionText.ShowBackGround = false;

            debugText = new GUILable("", new System.Drawing.Rectangle(5, 30, 200, 20), UIDock.TopLeft);
            debugText.SetColor(Color4.White);
            debugText.SetTextAling(Font.TextAling.Left);
            debugText.ShowBackGround = false;

            DarkcomsoftCopy = new GUILable("Copyright(c) 2020 Darkcomsoft - All rights reserved.", new System.Drawing.Rectangle(5, 5, 200, 20), UIDock.BottomLeft);
            DarkcomsoftCopy.SetColor(Color4.White);
            DarkcomsoftCopy.SetTextAling(Font.TextAling.Left);
            DarkcomsoftCopy.ShowBackGround = false;

            CreateLoadingScreen();
        }

        private void CreateLoadingScreen()
        {
            connectionBackgorund = new GUIImage(new System.Drawing.Rectangle(0, 0, 0, 0), UIDock.ScreenSizeRatio);
            connectionBackgorund.TextureName = "BackGround";
            connectionBackgorund.EnabledInput = false;
            connectionBackgorund.Disable();

            connectionTextInfo = new GUILable("Null Connection Screen Info", new System.Drawing.Rectangle(0, 0, 200, 20), UIDock.Cennter);
            connectionTextInfo.SetColor(Color4.White);
            connectionTextInfo.SetTextAling(Font.TextAling.Center);
            connectionTextInfo.ShowBackGround = false;
            connectionTextInfo.Disable();
        }

        public void Tick()
        {
            debugText.SetText("FPS:(" + EvllyEngine.Window.Instance.GetFPS + ") Tick:" + Time._Tick);
        }

        public void Dispose()
        {
            _Client = null;

            Logo.Dispose();
            imgteste.Dispose();

            debugText.Dispose();
            VersionText.Dispose();
            DarkcomsoftCopy.Dispose();

            PlaySingleButtom.OnClick -= PlayButtomClick;
            PlaySingleButtom.Dispose();

            PlayMultiButtom.OnClick -= PlayMButtomClick;
            PlayMultiButtom.Dispose();

            QuitButtom.OnClick -= QuitButtomClick;
            QuitButtom.Dispose();

            connectionBackgorund.Dispose();
            connectionTextInfo.Dispose();
        }

        private void PlayButtomClick()
        {
            _Client.StartSingleServer();
        }

        private void PlayMButtomClick()
        {
            
        }

        private void QuitButtomClick()
        {
            EvllyEngine.Window.Instance.Exit();
        }

        public void Open_ConnectingScreen(ClientType type)
        {
            Logo.Disable();
            imgteste.Disable();
            debugText.Disable();
            VersionText.Disable();
            DarkcomsoftCopy.Disable();
            PlaySingleButtom.Disable();
            PlayMultiButtom.Disable();
            QuitButtom.Disable();

            connectionBackgorund.Enable();
            connectionTextInfo.Enable();

            switch (type)
            {
                case ClientType.SinglePlayer:
                    connectionTextInfo.SetText("Starting SinglePlayer Server...");
                    break;
                case ClientType.Multiplayer:
                    connectionTextInfo.SetText("Connecting To Server...");
                    break;
            }
        }

        public void Close_ConnectingScreen(string error)
        {
            connectionTextInfo.SetText("ERROR " + error);

            connectionBackgorund.Disable();
            connectionTextInfo.Disable();

            Logo.Enable();
            imgteste.Enable();
            debugText.Enable();
            VersionText.Enable();
            DarkcomsoftCopy.Enable();
            PlaySingleButtom.Enable();
            PlayMultiButtom.Enable();
            QuitButtom.Enable();
        }
    }
}
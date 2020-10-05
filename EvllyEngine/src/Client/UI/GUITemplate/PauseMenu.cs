using ProjectEvlly.src.UI.GUIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ProjectEvlly.src.Net;

namespace ProjectEvlly.src.UI.GUITemplate
{
    public class PauseMenu : InGuiGame
    {
        private GUIButtom ResumeButtom;
        private GUIButtom OptionButtom;
        private GUIButtom DisconnectButtom;

        public PauseMenu()
        {
            ResumeButtom = new GUIButtom("Resume Game", new System.Drawing.Rectangle(0, 55, 200, 50), UIDock.Cennter);
            ResumeButtom.SetColor(Color4.White);
            ResumeButtom.SetBackColors(Color4.White, Color4.Gray, Color4.Gray, Color4.White);
            ResumeButtom.TextureName = "Buttom";

            OptionButtom = new GUIButtom("Options", new System.Drawing.Rectangle(0, 0, 200, 50), UIDock.Cennter);
            OptionButtom.SetColor(Color4.White);
            OptionButtom.SetBackColors(Color4.White, Color4.Gray, Color4.Gray, Color4.White);
            OptionButtom.TextureName = "Buttom";

            DisconnectButtom = new GUIButtom("Disconnect", new System.Drawing.Rectangle(0, -55, 200, 50), UIDock.Cennter);
            DisconnectButtom.SetColor(Color4.White);
            DisconnectButtom.SetBackColors(Color4.White, Color4.Gray, Color4.Gray, Color4.White);
            DisconnectButtom.TextureName = "Buttom";
            DisconnectButtom.OnClick += DisconnectOnClick;

            Close();
        }

        public void Open()
        {
            ResumeButtom.Enable();
            OptionButtom.Enable();
            DisconnectButtom.Enable();
        }

        public void Close()
        {
            ResumeButtom.Disable();
            OptionButtom.Disable();
            DisconnectButtom.Disable();
        }

        public void Dispose()
        {
            DisconnectButtom.OnClick -= DisconnectOnClick;

            ResumeButtom.Dispose();
            OptionButtom.Dispose();
            DisconnectButtom.Dispose();
        }

        private void DisconnectOnClick()
        {
            Network.Disconnect();
        }
    }
}

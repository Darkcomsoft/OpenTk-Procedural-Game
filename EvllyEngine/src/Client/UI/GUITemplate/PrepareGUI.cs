using ProjectEvlly.src.UI.GUIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using ProjectEvlly.src.Net;
using ProjectEvlly.src.save;

namespace ProjectEvlly.src.UI.GUITemplate
{
    public class PrepareGUI : IDisposable
    {
        private GUIImage Background;
        private GUIButtom DisconnectButtom;

        private GUIPanel CharListBack;

        private List<CharGuiItem> charGuiItems;

        public PrepareGUI()
        {
            charGuiItems = new List<CharGuiItem>();

            Background = new GUIImage(new System.Drawing.Rectangle(0, 0, 0, 0), UIDock.ScreenSizeRatio);
            Background.SetBackColors(Color4.YellowGreen, Color4.White, Color4.White);
            Background.TextureName = "BackGround";
            Background.EnabledInput = false;

            CharListBack = new GUIPanel(new System.Drawing.Rectangle(0, 55, 528, 528), UIDock.CenterBottom);
            CharListBack.TextureName = "Panel01";

            DisconnectButtom = new GUIButtom("Disconnect", new System.Drawing.Rectangle(0, 5, 200, 50), UIDock.CenterBottom);
            DisconnectButtom.SetTextAling(Font.TextAling.Center);
            DisconnectButtom.SetColor(Color4.White);
            DisconnectButtom.SetBackColors(Color4.White, Color4.Gray, Color4.Gray);
            DisconnectButtom.TextureName = "Buttom";
            DisconnectButtom.OnClick += DisconnectButtomClick;
        }

        public void Enable()
        {
            Background.Enable();
            DisconnectButtom.Enable();
        }

        public void Disable()
        {
            Background.Disable();
            DisconnectButtom.Disable();
        }

        public void Dispose()
        {
            Background.Dispose();
            DisconnectButtom.Dispose();

            CharListBack.Dispose();
        }

        public void SetCharData(CharSaveInfo[] charList)
        {
            for (int i = 0; i < charGuiItems.Count; i++)
            {
                charGuiItems[i].Dispose();
            }

            charGuiItems.Clear();

            for (int i = 0; i < 10; i++)
            {
                charGuiItems.Add(new CharGuiItem(charList[0], i));
            }
        }

        public void DisconnectButtomClick()
        {
            Network.Disconnect();
        }
    }

    public class CharGuiItem : IDisposable
    {
        private GUIPanel ItemBack;
        private GUILable CharName;

        public CharGuiItem(CharSaveInfo charInfo, int index)
        {
            ItemBack = new GUIPanel(new System.Drawing.Rectangle(0, 0 + -index * 50 + 520, 505, 50), UIDock.CenterBottom);
            ItemBack.SetBackColors(new Color4(0, 0, 0, 0.2f), Color4.White, Color4.White);

            CharName = new GUILable(charInfo.CharName + " I:" + index, new System.Drawing.Rectangle(0, 0 + -index * 50 + 520, 505, 50), UIDock.CenterBottom);
            CharName.SetTextAling(Font.TextAling.Left);
            CharName.SetColor(Color4.White);
            CharName.SetBackColors(Color4.White, Color4.Gray, Color4.Gray);
            CharName.ShowBackGround = false;
        }

        public void Dispose()
        {
            ItemBack.Dispose();
            CharName.Dispose();
        }
    }
}
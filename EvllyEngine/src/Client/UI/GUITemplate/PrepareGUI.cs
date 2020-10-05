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
        private GameUI _gameUI;
        private GUIImage Background;
        private GUIButtom DisconnectButtom;

        private GUIButtom PlayButtom;
        private GUIButtom DeleteButtom;

        private GUIPanel CharListBack;

        private List<CharGuiItem> charGuiItems;
        private CharGuiItem charGuiItemFocuse;
        private int indexFocused;

        public PrepareGUI(GameUI gameUI)
        {
            charGuiItems = new List<CharGuiItem>();

            _gameUI = gameUI;

            Background = new GUIImage(new System.Drawing.Rectangle(0, 0, 0, 0), UIDock.ScreenSizeRatio);
            Background.SetBackColors(Color4.YellowGreen, Color4.White, Color4.White, Color4.White);
            Background.TextureName = "BackGround";
            Background.SetInteractColor(Color4.White);
            Background.NoInteractable();

            CharListBack = new GUIPanel(new System.Drawing.Rectangle(0, 55, 528, 528), UIDock.CenterBottom);
            CharListBack.TextureName = "Panel01";

            DisconnectButtom = new GUIButtom("Disconnect", new System.Drawing.Rectangle(200, 5, 200, 50), UIDock.CenterBottom);
            DisconnectButtom.SetTextAling(Font.TextAling.Center);
            DisconnectButtom.SetColor(Color4.White);
            DisconnectButtom.SetBackColors(Color4.White, Color4.DarkGray, Color4.Gray, Color4.White);
            DisconnectButtom.TextureName = "Buttom";
            DisconnectButtom.OnClick += DisconnectButtomClick;


            PlayButtom = new GUIButtom("Play", new System.Drawing.Rectangle(-200, 5, 200, 50), UIDock.CenterBottom);
            PlayButtom.SetTextAling(Font.TextAling.Center);
            PlayButtom.SetColor(Color4.White);
            PlayButtom.SetBackColors(Color4.White, Color4.DarkGray, Color4.Gray, Color4.White);
            PlayButtom.TextureName = "Buttom";
            PlayButtom.NoInteractable();
            PlayButtom.OnClick += OnPlayClick;

            DeleteButtom = new GUIButtom("Delete", new System.Drawing.Rectangle(0, 5, 200, 50), UIDock.CenterBottom);
            DeleteButtom.SetTextAling(Font.TextAling.Center);
            DeleteButtom.SetColor(Color4.White);
            DeleteButtom.SetBackColors(Color4.White, Color4.DarkGray, Color4.Gray, Color4.White);
            DeleteButtom.TextureName = "Buttom";
            DeleteButtom.NoInteractable();
            DeleteButtom.OnClick += OnDeleteButtomClick;
        }

        public void Enable()
        {
            Background.Enable();
            DisconnectButtom.Enable();

            CharListBack.Enable();
            PlayButtom.Enable();
            DeleteButtom.Enable();
        }

        public void Disable()
        {
            Background.Disable();
            DisconnectButtom.Disable();

            CharListBack.Disable();
            PlayButtom.Disable();
            DeleteButtom.Disable();
        }

        public void Dispose()
        {
            PlayButtom.OnClick -= OnPlayClick;
            DeleteButtom.OnClick -= OnDeleteButtomClick;

            for (int i = 0; i < charGuiItems.Count; i++)
            {
                charGuiItems[i].OnClick -= OnCharFocus;
                charGuiItems[i].Dispose();
            }

            charGuiItems.Clear();

            Background.Dispose();
            DisconnectButtom.Dispose();

            CharListBack.Dispose();
            PlayButtom.Dispose();
            DeleteButtom.Dispose();
        }

        private void OnPlayClick()
        {
            _gameUI.PlayClick();
            Disable();
        }

        private void OnDeleteButtomClick()
        {
            _gameUI.DeleteClick();
        }

        public void SetCharData(CharSaveInfo[] charList)
        {
            for (int i = 0; i < charGuiItems.Count; i++)
            {
                charGuiItems[i].OnClick -= OnCharFocus;
                charGuiItems[i].Dispose();
            }

            charGuiItems.Clear();

            for (int i = 0; i < 1; i++)
            {
                CharGuiItem cahr = new CharGuiItem(charList[0], i);
                cahr.OnClick += OnCharFocus;
                charGuiItems.Add(cahr);
            }
        }

        private void OnCharFocus(int index)
        {
            if (charGuiItemFocuse != null)
            {
                if (charGuiItemFocuse != charGuiItems[index])
                {
                    charGuiItemFocuse.UnFocused();

                    charGuiItemFocuse = charGuiItems[index];
                    indexFocused = index;

                    charGuiItemFocuse.Focused();

                    PlayButtom.Interactable();
                    DeleteButtom.Interactable();
                }
            }
            else
            {
                charGuiItemFocuse = charGuiItems[index];
                indexFocused = index;

                charGuiItemFocuse.Focused();

                PlayButtom.Interactable();
                DeleteButtom.Interactable();
            }
        }

        public void DisconnectButtomClick()
        {
            Network.Disconnect();
        }
    }

    public class CharGuiItem : IDisposable
    {
        public string _CharName;

        private GUIPanel ItemBack;
        private GUIPanel ItemBackSelect;
        private GUILable CharName;

        private int CharIndex;

        public Action<int> OnClick;

        public CharGuiItem(CharSaveInfo charInfo, int index)
        {
            CharIndex = index;
            _CharName = charInfo.CharName;

            ItemBack = new GUIPanel(new System.Drawing.Rectangle(0, 0 + -index * 50 + 520, 505, 50), UIDock.CenterBottom);
            ItemBack.SetBackColors(new Color4(0, 0, 0, 0.2f), new Color4(0, 0, 0, 0.3f), new Color4(0, 0, 0, 0.5f), new Color4(1, 1, 1, 0.2f));
            ItemBack.Interactable();
            ItemBack.OnClick += onClcik;

            ItemBackSelect = new GUIPanel(new System.Drawing.Rectangle(0, 0 + -index * 50 + 520, 505, 50), UIDock.CenterBottom);
            ItemBackSelect.SetBackColors(new Color4(1, 1, 1, 0.2f), new Color4(1, 1, 1, 0.3f), new Color4(1, 1, 1, 0.5f), new Color4(1, 1, 1, 0.2f));
            ItemBackSelect.Interactable();
            ItemBackSelect.Disable();

            CharName = new GUILable(_CharName + " I:" + index, new System.Drawing.Rectangle(0, 0 + -index * 50 + 520, 505, 50), UIDock.CenterBottom);
            CharName.SetTextAling(Font.TextAling.Left);
            CharName.SetColor(Color4.White);
            CharName.SetBackColors(Color4.White, Color4.Gray, Color4.Gray, Color4.White);
            CharName.ShowBackGround = false;
            CharName.NoInteractable();
        }

        public void Focused()
        {
            ItemBackSelect.Enable();
        }

        public void UnFocused()
        {
            ItemBackSelect.Disable();
        }

        private void onClcik()
        {
            OnClick(CharIndex);
        }

        public void Dispose()
        {
            ItemBack.OnClick -= onClcik;

            ItemBackSelect.Dispose();
            ItemBack.Dispose();
            CharName.Dispose();
        }
    }
}
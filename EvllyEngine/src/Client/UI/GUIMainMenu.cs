using EvllyEngine;
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
        private GUIButtom PlaySingleButtom;
        private GUIButtom PlayMultiButtom;
        private GUIButtom QuitButtom;

        public GUIMainMenu()
        {
            PlaySingleButtom = new GUIButtom(new System.Drawing.Rectangle(0, 70, 200, 50), UIDock.Cennter);
            PlaySingleButtom.Text = "SinglePlayer";
            PlaySingleButtom.TextureName = "Buttom";
            PlaySingleButtom.OnClick += PlayButtomClick;

            PlayMultiButtom = new GUIButtom(new System.Drawing.Rectangle(0, 0, 200, 50), UIDock.Cennter);
            PlayMultiButtom.Text = "MultiPlayer";
            PlayMultiButtom.TextureName = "Buttom";
            PlayMultiButtom.OnClick += PlayMButtomClick;

            QuitButtom = new GUIButtom(new System.Drawing.Rectangle(0, -70, 200, 50), UIDock.Cennter);
            QuitButtom.Text = "Quit";
            QuitButtom.TextureName = "Buttom";
            QuitButtom.OnClick += QuitButtomClick;
        }

        public void Dispose()
        {
            PlaySingleButtom.OnClick -= PlayButtomClick;
            PlaySingleButtom.Dispose();

            PlayMultiButtom.OnClick -= PlayMButtomClick;
            PlayMultiButtom.Dispose();

            QuitButtom.OnClick -= QuitButtomClick;
            QuitButtom.Dispose();
        }

        private void PlayButtomClick()
        {
            Debug.Log("SinglePlayer Buttom Clicked!");
        }

        private void PlayMButtomClick()
        {
            Debug.Log("MultiPlayer Buttom Clicked!");
        }

        private void QuitButtomClick()
        {
            EvllyEngine.Window.Instance.Exit();
        }
    }
}
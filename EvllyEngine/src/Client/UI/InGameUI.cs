using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvllyEngine;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;
using Gwen.UnitTest;
using OpenTK;
using ProjectEvlly.src.Net;
using ProjectEvlly.src.save;
using ProjectEvlly.src.UI;

namespace ProjectEvlly.src.UI
{
    public class InGameUI : DockBase
    {
        private Gwen.Control.Label _Label;
        private Gwen.Control.Label _LabelDebug;

        #region PauseMenu
        private Gwen.Control.Button ResumeGame;
        private Gwen.Control.Button Options;
        private Gwen.Control.Button Quitbutton;

        private bool isPused = false;
        #endregion

        public InGameUI(Base parent) : base(parent)
        {
            Game._InGameUI = this;
            Dock = Pos.Fill;

            _Label = new Gwen.Control.Label(this);
            _Label.Text = MainApplication.AppName + " " + MainApplication.Version;
            _Label.MouseInputEnabled = true;
            _Label.SetToolTipText("Hi iam a tooltip!");
            _Label.Position(Pos.Top | Pos.Left, 10, 10);
            _Label.TextColor = Color.White;

            _LabelDebug = new Gwen.Control.Label(this);
            _LabelDebug.Text = "FPS:null";
            _LabelDebug.MouseInputEnabled = true;
            _LabelDebug.SetToolTipText("this is the graphical frame rate");
            _LabelDebug.Position(Pos.Top | Pos.Left, 10, 10);
            _LabelDebug.TextColor = Color.White;

            CreatePauseMenu();
        }

        public void OnResize()
        {
            #region PauseMenu
            if (Input.GetKeyDown(OpenTK.Input.Key.Escape))
            {
                if (isPused)
                {
                    isPused = false;

                    ResumeGame.Hide();
                    Quitbutton.Hide();
                    Options.Hide();

                    EvllyEngine.MouseCursor.LockCursor();
                }
                else
                {
                    isPused = true;

                    ResumeGame.Show();
                    Quitbutton.Show();
                    Options.Show();

                    EvllyEngine.MouseCursor.UnLockCursor();
                }
            }

            if (isPused)
            {
                ResumeGame.Position(Pos.Center, 0, -70);
                ResumeGame.SetSize(200, 50);
                Quitbutton.SetSize(200, 50);
                Align.PlaceDownLeft(Options, ResumeGame, 10);
                Align.PlaceDownLeft(Quitbutton, Options, 10);
            }
            #endregion
            _Label.Position(Pos.Top | Pos.Left, 10, 10);
            _LabelDebug.Position(Pos.Top | Pos.Left, 10, 30);

            _LabelDebug.Text = "FPS:(" + EvllyEngine.Window.Instance.GetFPS + ") PlayerPosition: " + PlayerEntity.Instance.transform.Position;
        }

        private void CreatePauseMenu()
        {
            ResumeGame = new Gwen.Control.Button(this);
            ResumeGame.Text = "Resume";
            ResumeGame.Clicked += ResumeGame_Clicked;
            ResumeGame.Position(Pos.Top, 0, 0);
            ResumeGame.SetSize(200, 50);
            ResumeGame.Hide();

            Options = new Gwen.Control.Button(this);
            Options.Text = "Option";
            Options.Clicked += Option_Clicked;
            Options.Position(Pos.Top, 0, 0);
            Options.SetSize(200, 50);
            Options.Hide();

            Align.PlaceDownLeft(Options, ResumeGame, 10);

            Quitbutton = new Gwen.Control.Button(this);
            Quitbutton.Text = "Quit To Menu";
            Quitbutton.Clicked += Quitbutton_Clicked;
            Quitbutton.SetSize(200, 50);
            Quitbutton.Hide();

            EvllyEngine.MouseCursor.LockCursor();
        }

        private void ResumeGame_Clicked(Base sender, ClickedEventArgs arguments)
        {
            isPused = false;

            ResumeGame.Hide();
            Quitbutton.Hide();
            Options.Hide();

            EvllyEngine.MouseCursor.LockCursor();
        }

        private void Option_Clicked(Base sender, ClickedEventArgs arguments)
        {

        }

        private void Quitbutton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Network.Disconnect();
        }

        public override void Dispose()
        {
            #region PauseMenu
            Quitbutton.Clicked -= Quitbutton_Clicked;
            Options.Clicked -= Option_Clicked;
            ResumeGame.Clicked -= ResumeGame_Clicked;

            ResumeGame.Dispose();
            Options.Dispose();
            Quitbutton.Dispose();
            #endregion

            _Label.Dispose();
            _LabelDebug.Dispose();
            base.Dispose();
        }
    }
}

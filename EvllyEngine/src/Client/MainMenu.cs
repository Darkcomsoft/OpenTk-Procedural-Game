using System;
using System.Collections.Generic;
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

namespace ProjectEvlly.src
{
    public class MainMenu : DockBase
    {
        private readonly Center _Center;

        private Gwen.Control.Label _Label;
        private readonly Gwen.Control.Label _copyText;

        private Gwen.Control.ImagePanel logoImage;

        private Gwen.Control.Button SinglePlayerbutton;
        private Gwen.Control.Button MultiPlayerbutton;
        private Gwen.Control.Button Quitbutton;

        private Gwen.Control.StatusBar CharacterScrollBack;
        private Gwen.Control.ScrollControl CharacterScroll;
        private Gwen.Control.Button CharacterBack;

        private Gwen.Control.Button CharCreate;

        ///////////////CharCreator/////////////////////
        private Gwen.Control.TextBox CharNameTextBox;
        private Gwen.Control.Button CharFinalCreate;
        private Gwen.Control.Button CharFinalCancel;

        //////////////////Direct Connect/////////////////
        private Gwen.Control.TextBox ipTextInput;
        private Gwen.Control.TextBoxNumeric portTextInput;
        private Gwen.Control.Button ipportConnect;
        private Gwen.Control.Button backDirectConnect;

        private List<Gwen.Control.Button> ButtomList;

        public MainMenu(Base parent) : base(parent)
        {
            ButtomList = new List<Gwen.Control.Button>();

            Dock = Pos.Fill;

            //DrawDebugOutlines = true;
            /*RightDock.TabControl.AddPage("Unit tests");
            RightDock.Width = 250;*/

            CreateDirectConnectContainer();

            CharacterBack = new Gwen.Control.Button(this);
            CharacterBack.Text = "Back";
            CharacterBack.Clicked += BackButtomClick;
            CharacterBack.Position(Pos.Left | Pos.Bottom, 0, 0);
            CharacterBack.SetSize(50, 40);
            CharacterBack.Margin = Margin.Zero;
            CharacterBack.Hide();

            CharacterScrollBack = new Gwen.Control.StatusBar(this);
            CharacterScrollBack.Dock = Pos.Right;
            CharacterScrollBack.SetBounds(0, 0, 250, 0);
            CharacterScrollBack.Hide();

            CharacterScroll = new Gwen.Control.ScrollControl(CharacterScrollBack);
            CharacterScroll.Dock = Pos.Right;
            CharacterScroll.EnableScroll(false, true);
            CharacterScroll.ShouldDrawBackground = true;
            CharacterScroll.SetBounds(0, 0, 250, 0);
            CharacterScroll.Hide();

            logoImage = new Gwen.Control.ImagePanel(this);
            logoImage.ImageName = "Assets/Images/VaKLogoYellow.png";
            logoImage.Position(Pos.Top, 0, 0);
            logoImage.SetSize(400, 100);

            SinglePlayerbutton = new Gwen.Control.Button(this);
            SinglePlayerbutton.Text = "SinglePlayer";
            SinglePlayerbutton.Clicked += PlayButtomClick;
            SinglePlayerbutton.Position(Pos.Top, 0,0);
            SinglePlayerbutton.SetSize(200, 50);

            MultiPlayerbutton = new Gwen.Control.Button(this);
            MultiPlayerbutton.Text = "MultiPlayer";
            MultiPlayerbutton.Clicked += PlayMultiPlayerClick;
            MultiPlayerbutton.Position(Pos.Top, 0, 0);
            MultiPlayerbutton.SetSize(200, 50);

            Align.PlaceDownLeft(MultiPlayerbutton, SinglePlayerbutton, 10);

            Quitbutton = new Gwen.Control.Button(this);
            Quitbutton.Text = "Quit";
            Quitbutton.Clicked += QuitButtomClick;
            Quitbutton.SetSize(200, 50);

            Align.PlaceDownLeft(Quitbutton, MultiPlayerbutton, 10);

            //CharCreator Window
            CharNameTextBox = new Gwen.Control.TextBox(this);
            CharNameTextBox.Text = "Character Name";
            CharNameTextBox.TextChanged += OnCharNameChange;
            CharNameTextBox.SetSize(200, 30);
            CharNameTextBox.TextColor = Color.White;
            CharNameTextBox.Hide();

            CharFinalCreate = new Gwen.Control.Button(this);
            CharFinalCreate.Text = "Create";
            CharFinalCreate.Clicked += OnButtomCreateClicked;
            CharFinalCreate.SetSize(50, 30);
            CharFinalCreate.Disable();
            CharFinalCreate.Hide();

            CharFinalCancel = new Gwen.Control.Button(this);
            CharFinalCancel.Text = "Cancel";
            CharFinalCancel.SetSize(50, 30);
            CharFinalCancel.Clicked += CharCreatorCancel;
            CharFinalCancel.Hide();

            _copyText = new Gwen.Control.Label(this);
            _copyText.Text = "Copyright (c) 2020 Darkcomsoft - All rights reserved.";
            _copyText.Position(Pos.Bottom, 10, 10);
            _copyText.TextColor = Color.White;

            _Label = new Gwen.Control.Label(this);
            _Label.Text = MainApplication.AppName + " " + MainApplication.Version;
            _Label.MouseInputEnabled = true;
            _Label.SetToolTipText("Hi iam a tooltip!");
            _Label.Position(Pos.Top | Pos.Left, 10, 10);
            _Label.TextColor = Color.White;

            Game.GUI.GetCanvas.ShouldDrawBackground = true;
            Game.GUI.GetCanvas.BackgroundColor = Color.Black;

            OnResize();
        }

        private void CreateDirectConnectContainer()
        {
            ipTextInput = new Gwen.Control.TextBox(this);
            ipTextInput.SetSize(150, 30);
            ipTextInput.Text = "127.0.0.1";
            ipTextInput.TextColor = Color.White;
            ipTextInput.Hide();

            portTextInput = new TextBoxNumeric(this);
            portTextInput.Value = 25000;
            portTextInput.SetSize(150, 30);
            portTextInput.TextColor = Color.White;
            portTextInput.Hide();

            ipportConnect = new Gwen.Control.Button(this);
            ipportConnect.Text = "Connect";
            ipportConnect.Clicked += IpportConnect_Clicked;
            ipportConnect.SetSize(150, 30);
            ipportConnect.Hide();

            backDirectConnect = new Gwen.Control.Button(this);
            backDirectConnect.Text = "Cancel";
            backDirectConnect.Clicked += BackDirectConnect_Clicked;
            backDirectConnect.SetSize(150, 30);
            backDirectConnect.Hide();
        }

        public void OnResize()
        {
            SinglePlayerbutton.Position(Pos.Center, 0, -40);
            logoImage.Position(Pos.Center, 0, -130);

            _copyText.Position(Pos.Left | Pos.Bottom, 10, 10);
            _Label.Position(Pos.Top | Pos.Left, 10, 10);

            CharacterBack.Position(Pos.Right | Pos.Bottom, 248, 0);

            CharNameTextBox.Position(Pos.CenterH | Pos.Top, 0, 15);
            CharFinalCreate.Position(Pos.CenterH | Pos.Top, 130, 15);
            CharFinalCancel.Position(Pos.CenterH | Pos.Top, -130, 15);

            ipTextInput.Position(Pos.Center, 0, -35);
            portTextInput.Position(Pos.Center, 0, 0);
            ipportConnect.Position(Pos.Center, 0, 35);
            backDirectConnect.Position(Pos.Center, 0, 70);

            SinglePlayerbutton.SetSize(200, 50);
            Quitbutton.SetSize(200, 50);

            Align.PlaceDownLeft(MultiPlayerbutton, SinglePlayerbutton, 10);
            Align.PlaceDownLeft(Quitbutton, MultiPlayerbutton, 10);
        }

        private void PlayButtomClick(Base control, EventArgs args)
        {
            if (CharCreate != null)
            {
                CharCreate.Clicked -= charCreateClick;
                CharacterScroll.RemoveChild(CharCreate, true);
            }

            foreach (var item in ButtomList)
            {
                item.DeleteAllChildren();

                if (item.Name == "containerRoll")
                {
                    CharacterScroll.RemoveChild(item, true);
                }

                item.Dispose();
            }

            ButtomList.Clear();

            CharSaveInfo[] listcahr = SaveManager.LoadChars();

            listcahr.Reverse();
            for (int i = 0; i < listcahr.Length; i++)
            {
                Gwen.Control.Button pTestButton = new Gwen.Control.Button(CharacterScroll);
                pTestButton.Name = "containerRoll";
                pTestButton.Text = listcahr[i].CharName;
                pTestButton.TextColor = Color.White;
                pTestButton.Alignment = Pos.Top | Pos.Left;
                pTestButton.SetBounds(0, i * 50, 249, 50);
                pTestButton.Disable();
                //pTestButton.DrawDebugOutlines = true;

                Gwen.Control.Button PlayButtom = new Gwen.Control.Button(pTestButton);
                PlayButtom.Position(Pos.Bottom | Pos.Left, 5,5);
                //lable.Alignment = Pos.Center;
                PlayButtom.Name = "PlayButtom";
                PlayButtom.Text = "Play";
                PlayButtom.UserData = (CharSaveInfo)listcahr[i];
                PlayButtom.Clicked += OnPlayItemClciked;

                Gwen.Control.Button DeleteButtom = new Gwen.Control.Button(pTestButton);
                DeleteButtom.Position(Pos.Bottom | Pos.Right, 5, 5);
                //lable.Alignment = Pos.Center;
                DeleteButtom.Name = "DeleteButtom";
                DeleteButtom.Text = "Delete";
                DeleteButtom.Clicked += OnDeleteItemClciked;
                DeleteButtom.UserData = (CharSaveInfo)listcahr[i];

                ButtomList.Add(pTestButton);
                ButtomList.Add(PlayButtom);
                ButtomList.Add(DeleteButtom);

                if (i == listcahr.Length -1)
                {
                    CharCreate = new Gwen.Control.Button(CharacterScroll);
                    CharCreate.SetText("Create Character");
                    CharCreate.Clicked += charCreateClick;
                    CharCreate.SetBounds(0, (i + 1) * 50, 250, 50);
                }
            }

            if (listcahr.Length == 0)
            {
                CharCreate = new Gwen.Control.Button(CharacterScroll);
                CharCreate.SetText("Create Character");
                CharCreate.Clicked += charCreateClick;
                CharCreate.SetBounds(0, 0, 249, 50);
            }

            CharacterScroll.Show();
            CharacterScrollBack.Show();
            CharacterBack.Show();

            //GameRef.GUI.GetCanvas.ShouldDrawBackground = false;
            //Game.Game.LoadGameWorld();
        }

        private void BackButtomClick(Base control, EventArgs args)
        {
            CharacterScroll.Hide();
            CharacterScrollBack.Hide();
            CharacterBack.Hide();
            //GameRef.GUI.GetCanvas.ShouldDrawBackground = false;
            //Game.Game.LoadGameWorld();
        }

        private void charCreateClick(Base control, EventArgs args)
        {
            CharacterScrollBack.Hide();
            CharacterScroll.Hide();
            CharacterBack.Hide();

            logoImage.Hide();
            SinglePlayerbutton.Hide();
            MultiPlayerbutton.Hide();
            Quitbutton.Hide();

            CharNameTextBox.Show();
            CharFinalCreate.Show();
            CharFinalCancel.Show();
        }

        private void CharCreatorCancel(Base control, EventArgs args)
        {
            CharacterScrollBack.Show();
            CharacterScroll.Show();
            CharacterBack.Show();

            logoImage.Show();
            SinglePlayerbutton.Show();
            MultiPlayerbutton.Show();
            Quitbutton.Show();

            CharNameTextBox.Hide();
            CharFinalCreate.Hide();
            CharFinalCancel.Hide();
        }

        private void OnButtomCreateClicked(Base control, EventArgs args)
        {
            Game.Client.CreateWorldAndPlay(new CharSaveInfo(CharNameTextBox.Text, "WorldName"));
        }

        private void OnCharNameChange(Base sender, EventArgs arguments)
        {
            if (CharNameTextBox.Text.Length > 0)
            {
                CharFinalCreate.Enable();
            }
            else
            {
                CharFinalCreate.Disable();
            }
        }

        private void QuitButtomClick(Base control, EventArgs args)
        {
            EvllyEngine.Window.Instance.Exit();
        }

        private void OnDeleteItemClciked(Base control, EventArgs args)
        {
            CharSaveInfo save = (CharSaveInfo)control.UserData;
            Game.Client.DeleteCharAndWorld(save);
            PlayButtomClick(control, args);
        }

        private void OnPlayItemClciked(Base control, EventArgs args)
        {
            CharSaveInfo save = (CharSaveInfo)control.UserData;
            Game.GUI.GetCanvas.ShouldDrawBackground = false;
            Game.Client.PlaySingleWorld(save);
        }

        private void PlayMultiPlayerClick(Base control, EventArgs args)
        {
            CharacterScrollBack.Hide();
            CharacterScroll.Hide();
            CharacterBack.Hide();

            logoImage.Show();
            SinglePlayerbutton.Hide();
            MultiPlayerbutton.Hide();
            Quitbutton.Hide();

            ipTextInput.Show();
            portTextInput.Show();
            ipportConnect.Show();
            backDirectConnect.Show();
        }

        private void BackDirectConnect_Clicked(Base sender, ClickedEventArgs arguments)
        {
            
            logoImage.Show();
            SinglePlayerbutton.Show();
            MultiPlayerbutton.Show();
            Quitbutton.Show();

            ipTextInput.Hide();
            portTextInput.Hide();
            ipportConnect.Hide();
            backDirectConnect.Hide();
        }

        private void IpportConnect_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Network.Connect(ipTextInput.Text, int.Parse(portTextInput.Text), "");
        }

        public override void Dispose()
        {
            SinglePlayerbutton.Clicked -= PlayButtomClick;
            //MultiPlayerbutton -= PlayButtomClick;
            CharacterBack.Clicked -= BackButtomClick;
            Quitbutton.Clicked -= QuitButtomClick;
            CharFinalCancel.Clicked -= CharCreatorCancel;
            CharNameTextBox.TextChanged -= OnCharNameChange;
            CharFinalCreate.Clicked -= OnButtomCreateClicked;
            backDirectConnect.Clicked -= BackDirectConnect_Clicked;
            ipportConnect.Clicked -= IpportConnect_Clicked;

            Game.GUI.GetCanvas.ShouldDrawBackground = false;

            if (CharCreate != null)
            {
                CharCreate.Clicked -= charCreateClick;
                CharCreate.Dispose();
            }

            foreach (var item in ButtomList)
            {
                CharacterScroll.RemoveChild(item, true);
                item.Clicked -= null;
                item.Dispose();
            }

            ButtomList.Clear();

            SinglePlayerbutton.Dispose();
            MultiPlayerbutton.Dispose();
            Quitbutton.Dispose();
            _Label.Dispose();
            logoImage.Dispose();
            CharacterScroll.Dispose();
            CharacterBack.Dispose();
            CharacterScrollBack.Dispose();

            CharNameTextBox.Dispose();
            CharFinalCreate.Dispose();
            CharFinalCancel.Dispose();

            base.Dispose();
        }
    }
}

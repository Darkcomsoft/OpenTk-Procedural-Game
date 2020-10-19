using System;
using System.Windows;
using System.IO;
using System.Collections;

namespace GameLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string LauncherVersion = "V:0.0.0.1";

        private readonly string RootPath = "";
        private readonly string AssetsPath = "";
        private readonly string BinPath = "";

        public MainWindow()
        {
            InitializeComponent();

            RootPath = Directory.GetCurrentDirectory();
            AssetsPath = Path.Combine(RootPath, "/Assets/");
            BinPath = Path.Combine(RootPath, "/Bin/");
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Version.Text = LauncherVersion;
        }

        private void Button_Click(object sender, RoutedEventArgs e)//Play Buttom
        {
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)//Config Buttom
        {

        }
    }
}

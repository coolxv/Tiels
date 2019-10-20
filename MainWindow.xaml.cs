﻿using DWinOverlay.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DWinOverlay
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+"\\Tiles";
        public List<TileWindow> tilesw = new List<TileWindow>();
        private System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();

        public MainWindow()
        {
            InitializeComponent();

            //Notify Icon
            ni.Icon = new System.Drawing.Icon(@"C:\Users\DcZipPL\Desktop\Tiles\appicon.ico");
            ni.Visible = false;
            ni.Click +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                    ni.Visible = false;
                };
        }

        private void TileLoaded(object sender, RoutedEventArgs e)
        {
            //If exists config and main app directory
            if (!Directory.Exists(path) || !File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Tiels\\config.json"))
            {
                ConfigureFirstRun();
            }
            else
            {
                Load();
            }
        }

        public async void Load()
        {
            main.Navigate(new Uri("pack://application:,,,/DWinOverlay;component/Pages/LoadingPage.xaml"));
            string[] tiles = Directory.EnumerateDirectories(path).ToArray();
            if (tiles.Length != 0)
            {
                ConfigClass config = Config.GetConfig();
                for (int i = 0; i <= tiles.Length - 1; i++)
                {
                    tiles[i] = tiles[i].Replace(path + "\\", "");

                    //Creating tile
                    TileWindow tile = new TileWindow();
                    tile.folderNameTB.Text = tiles[i];
                    tile.name = tiles[i];
                    tile.Show();
                    tilesw.Add(tile);
                    bool windowexist = false;

                    //Search for tile in config
                    foreach (var window in config.Windows)
                    {
                        if (window.Name == tiles[i])
                        {
                            //Tile exists?
                            windowexist = true;

                            //Setting tile rosition
                            tile.Left = window.Position.X;
                            tile.Top = window.Position.Y;

                            //Rotating tile
                            if (!window.EditBar)
                            {
                                tile.rd.Height = new GridLength(28);
                                tile.trd.Height = GridLength.Auto;
                                Grid.SetRow(tile.ActionGrid, 0);
                                Grid.SetRow(tile.FilesList, 1);
                            }
                            else
                            {
                                tile.rd.Height = GridLength.Auto;
                                tile.trd.Height = new GridLength(28);
                                Grid.SetRow(tile.ActionGrid, 1);
                                Grid.SetRow(tile.FilesList, 0);
                            }
                        }
                    }
                    //If tile not exists create default values
                    if (!windowexist)
                    {
                        JsonWindow jsonwindow = new JsonWindow();
                        jsonwindow.Name = tiles[i];
                        jsonwindow.Position = new WindowPosition { X = 0, Y = 0 };
                        jsonwindow.EditBar = false;
                        config.Windows.Add(jsonwindow);
                    }
                }
                //If Config File not Exists
                bool result = Config.SetConfig(config);
                if (result == false)
                {
                    Util.Reconfigurate();
                }
            }

            await Task.Delay(1300);
            //Load MainPage
            main.Navigate(new Uri("pack://application:,,,/DWinOverlay;component/Pages/MainPage.xaml"));
            loadingMessage.Text = "Tile Loaded Successfully!";
        }

        public async void ConfigureFirstRun()
        {
            main.Navigate(new Uri("pack://application:,,,/DWinOverlay;component/Pages/LoadingPage.xaml"));

            //Starting Tiels Console that generate Main App Directory and temp
            Process.Start("TielsConsole.exe","createlostandfound");
            await Task.Delay(2600);
            main.Navigate(new Uri("pack://application:,,,/DWinOverlay;component/Pages/ConfigurePage.xaml"));
            loadingMessage.Text = "Configuration.";

            //Creating config and creating example tile
            WindowPosition pos0 = new WindowPosition();
            WindowPosition pos1 = new WindowPosition();
            WindowPosition[] positions = new WindowPosition[] { pos0, pos1 };
            JsonWindow jwindow = new JsonWindow();
            List<JsonWindow> jwindows = new List<JsonWindow>();
            jwindows.Add(jwindow);
            ConfigClass config = new ConfigClass
            {
                FirstRun = true,
                Blur = true,
                Theme = 1,
                Color = "#19000000",
                Windows = jwindows
            };

            //Creating directory and config file
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Tiels"))
                File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Tiels\\config.json", json);
            Console.WriteLine(json);
            if (!Directory.Exists(path + "\\Example"))
                Directory.CreateDirectory(path + "\\Example");

            //Creating example text file in tile
            File.WriteAllText(path + "\\Example\\ExampleContent.txt","Example Text.");
        }

        private void CloseWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void HideWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void TaskbarWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            this.Hide();
            ni.Visible = true;
        }
    }
}
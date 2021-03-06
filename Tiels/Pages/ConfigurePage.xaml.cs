﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tiels.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy ConfigurePage.xaml
    /// </summary>
    public partial class ConfigurePage : Page
    {
        protected string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Tiles";
        public ConfigurePage()
        {
            InitializeComponent();
            mel.LoadedBehavior = MediaState.Manual;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            apath.Text = path;
        }

        private void Mel_Loaded(object sender, RoutedEventArgs e)
        {
            mel.Source = new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Assets\tutorial_fm.mp4");
            mel.Play();
        }

        private void TestPlay(object sender, RoutedEventArgs e)
        {
            mel.Source = new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Assets\tutorial_fm.mp4");
            mel.Play();
        }

        private void Mel_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ErrorWindow ew = new ErrorWindow();
            ew.ExceptionReason = e.ErrorException;
            ew.ShowDialog();
        }

        private void Mel_MediaEnded(object sender, RoutedEventArgs e)
        {
            mel.Play();
        }

        private void NextBtn(object sender, RoutedEventArgs e)
        {
            MainWindow mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mw.Load();
        }
    }
}

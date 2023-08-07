﻿using SSDEACT = Sucrose.Shared.Dependency.Enum.ArgumentCommandsType;
using Sucrose.Portal.Services.Contracts;
using Sucrose.Portal.ViewModels;
using System.IO;
using System.Windows;
using Wpf.Ui.Controls;
using SEWTT = Skylark.Enum.WindowsThemeType;
using SMC = Sucrose.Memory.Constant;
using SMMI = Sucrose.Manager.Manage.Internal;
using SMR = Sucrose.Memory.Readonly;
using SPVPLP = Sucrose.Portal.Views.Pages.LibraryPage;
using SPVPSGSP = Sucrose.Portal.Views.Pages.Setting.GeneralSettingPage;
using SWHWT = Skylark.Wing.Helper.WindowsTheme;
using WUAAT = Wpf.Ui.Appearance.ApplicationTheme;
using WUAT = Wpf.Ui.Appearance.ApplicationThemeManager;

namespace Sucrose.Portal.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IWindow
    {
        private static IList<char> Chars => Enumerable.Range('A', 'Z' - 'A' + 1).Concat(Enumerable.Range('a', 'z' - 'a' + 1)).Concat(Enumerable.Range('0', '9' - '0' + 1)).Select(C => (char)C).ToList();

        private static string Directory => SMMI.EngineSettingManager.GetSetting(SMC.Directory, Path.Combine(SMR.DocumentsPath, SMR.AppName));

        private static SEWTT Theme => SMMI.GeneralSettingManager.GetSetting(SMC.ThemeType, SWHWT.GetTheme());

        private static string Agent => SMMI.GeneralSettingManager.GetSetting(SMC.UserAgent, SMR.UserAgent);

        private static string Key => SMMI.PrivateSettingManager.GetSetting(SMC.Key, SMR.Key);

        private static bool Navigated { get; set; } = false;

        public MainWindowViewModel ViewModel { get; }

        public MainWindow(MainWindowViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            if (Theme == SEWTT.Dark)
            {
                WUAT.Apply(WUAAT.Dark);
            }
            else
            {
                WUAT.Apply(WUAAT.Light);
            }

            InitializeComponent();

            string[] Args = Environment.GetCommandLineArgs();

            if (Args.Count() > 1 && Args[1] == $"{SSDEACT.Setting}")
            {
                Navigated = true;
                UnrootView.Visibility = Visibility.Visible;
                UnrootView.Loaded += (_, _) => UnrootView.Navigate(typeof(SPVPSGSP));
            }
            else
            {
                RootView.Visibility = Visibility.Visible;
                RootView.Loaded += (_, _) => RootView.Navigate(typeof(SPVPLP));
            }

            //string StoreFile = Path.Combine(SMR.AppDataPath, SMR.AppName, SMR.CacheFolder, SMR.Store, SMR.StoreFile);

            //if (SSSHD.Store(StoreFile, Agent, Key))
            //{
            //    //MessageBox.Show(SSSHS.Json(StoreFile));

            //    SSSIR Root = SSSHS.DeserializeRoot(StoreFile);

            //    foreach (KeyValuePair<string, SSSIC> Category in Root.Categories)
            //    {
            //        //MessageBox.Show("Kategori: " + Category.Key);

            //        foreach (KeyValuePair<string, SSSIW> Wallpaper in Category.Value.Wallpapers)
            //        {
            //            //MessageBox.Show("Duvar Kağıdı: " + Wallpaper.Key);

            //            //MessageBox.Show("Kaynak: " + Wallpaper.Value.Source);
            //            //MessageBox.Show("Kapak: " + Wallpaper.Value.Cover);
            //            //MessageBox.Show("Canlı: " + Wallpaper.Value.Live);

            //            string Keys = SHG.GenerateString(Chars, 25, SMR.Randomise);
            //            bool Result = SSSHD.Theme(Path.Combine(Wallpaper.Value.Source, Wallpaper.Key), Path.Combine(Directory, Keys), Agent, Keys, Key).Result;
            //        }
            //    }
            //}
        }

        private void NavigationChange_Click(object sender, RoutedEventArgs e)
        {
            NavigationViewItem View = sender as NavigationViewItem;

            if (View.Name == "FromRoot")
            {
                RootView.Visibility = Visibility.Hidden;
                UnrootView.Visibility = Visibility.Visible;
                UnrootView.Navigate(typeof(SPVPSGSP));
            }
            else
            {
                UnrootView.Visibility = Visibility.Hidden;
                RootView.Visibility = Visibility.Visible;

                if (Navigated)
                {
                    Navigated = false;
                    RootView.Navigate(typeof(SPVPLP));
                }
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double WindowWidth = e.NewSize.Width;
            double SearchWidth = SearchBox.RenderSize.Width;

            SearchBox.Margin = new Thickness(0, 0, ((WindowWidth - SearchWidth) / 2) - 165, 0);
        }
    }
}
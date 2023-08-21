﻿using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using SMR = Sucrose.Memory.Readonly;
using SPMI = Sucrose.Portal.Manage.Internal;
using SPMM = Sucrose.Portal.Manage.Manager;
using SPVCSC = Sucrose.Portal.Views.Controls.StoreCard;
using SSRER = Sucrose.Shared.Resources.Extension.Resources;
using SSSHC = Sucrose.Shared.Space.Helper.Clean;
using SSSIC = Sucrose.Shared.Store.Interface.Category;
using SSSIR = Sucrose.Shared.Store.Interface.Root;
using SSSIW = Sucrose.Shared.Store.Interface.Wallpaper;
using SSTHI = Sucrose.Shared.Theme.Helper.Info;

namespace Sucrose.Portal.Views.Pages.Store
{
    /// <summary>
    /// FullStorePage.xaml etkileşim mantığı
    /// </summary>
    public partial class FullStorePage : Page, IDisposable
    {
        public static ICollection<NavigationViewItem> MenuItems { get; set; }

        private SSSIR Root = new();

        internal FullStorePage(SSSIR Root)
        {
            this.Root = Root;
            DataContext = this;

            ObservableCollection<NavigationViewItem> Categories = new();

            NavigationViewItem AllMenu = new(SSRER.GetValue("Portal", "Category", "All"), SPMI.AllIcon, null)
            {
                Tag = string.Empty,
                IsActive = SPMI.CategoryService.CategoryTag == string.Empty
            };

            AllMenu.Click += (s, e) => CategoryClick(s);

            Categories.Add(AllMenu);

            foreach (KeyValuePair<string, SSSIC> Category in Root.Categories)
            {
                if (Category.Value.Wallpapers.Any() && (SPMM.Adult || Category.Value.Wallpapers.Count(Wallpaper => Wallpaper.Value.Adult) != Category.Value.Wallpapers.Count()))
                {
                    SymbolRegular Symbol = SPMI.DefaultIcon;

                    if (SPMI.CategoryIcons.TryGetValue(Category.Key, out SymbolRegular Icon))
                    {
                        Symbol = Icon;
                    }

                    NavigationViewItem Menu = new(SSRER.GetValue("Portal", "Category", Category.Key.Replace(" ", "")), Symbol, null)
                    {
                        Tag = Category.Key,
                        IsActive = SPMI.CategoryService.CategoryTag == Category.Key
                    };

                    Menu.Click += (s, e) => CategoryClick(s);

                    Categories.Add(Menu);
                }
            }

            Categories = new(Categories.OrderBy(Menu => Menu.Content));

            Categories.Move(Categories.IndexOf(Categories.FirstOrDefault(Menu => Menu == AllMenu)), 0);

            MenuItems = Categories;

            InitializeComponent();

            Category();
            Search();
        }

        private void Search()
        {
            string Search = SPMI.SearchService.SearchText;

            SPMI.SearchService.Dispose();

            SPMI.SearchService = new()
            {
                SearchText = Search
            };

            SPMI.SearchService.SearchTextChanged += SearchService_SearchTextChanged;
        }

        private void Category()
        {
            string Tag = SPMI.CategoryService.CategoryTag;

            SPMI.CategoryService.Dispose();

            SPMI.CategoryService = new()
            {
                CategoryTag = Tag
            };

            SPMI.CategoryService.CategoryTagChanged += CategoryService_CategoryTagChanged;
        }

        private void CategoryClick(object s)
        {
            NavigationViewItem sender = s as NavigationViewItem;

            sender.IsActive = true;

            SPMI.CategoryService.CategoryTag = sender.Tag.ToString();

            CategoryView.MenuItems
                .OfType<NavigationViewItem>()
                .Where(Item => Item.IsActive)
                .ToList()
                .ForEach(Item =>
                {
                    if (Item != sender)
                    {
                        Item.IsActive = false;
                    }
                });
        }

        private async Task AddThemes(string Text, string Tag)
        {
            Dispose();

            foreach (KeyValuePair<string, SSSIC> Category in Root.Categories)
            {
                if (string.IsNullOrEmpty(SPMI.CategoryService.CategoryTag) || Category.Key == SPMI.CategoryService.CategoryTag)
                {
                    foreach (KeyValuePair<string, SSSIW> Wallpaper in Category.Value.Wallpapers)
                    {
                        if (!Wallpaper.Value.Adult || (Wallpaper.Value.Adult && SPMM.Adult))
                        {
                            string Title = Wallpaper.Key.ToLowerInvariant();
                            string Theme = Path.Combine(SMR.AppDataPath, SMR.AppName, SMR.CacheFolder, SMR.Store, SSSHC.FileName(Wallpaper.Key));

                            if (SearchControl(Text, Theme, Title))
                            {
                                if (SPMI.CategoryService.CategoryTag == Tag && SPMI.SearchService.SearchText == Text)
                                {
                                    SPVCSC StoreCard = new(Theme, Wallpaper, SPMM.Agent, SPMM.Key);

                                    ThemeStore.Children.Add(StoreCard);

                                    Empty.Visibility = Visibility.Collapsed;

                                    await Task.Delay(25);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (ThemeStore.Children.Count <= 0)
            {
                Empty.Visibility = Visibility.Visible;
            }
        }

        private bool SearchControl(string Search, string Theme, string Title)
        {
            if (string.IsNullOrEmpty(Search))
            {
                return true;
            }
            else
            {
                string InfoPath = Path.Combine(Theme, SMR.SucroseInfo);

                if (File.Exists(InfoPath))
                {
                    SSTHI Info = SSTHI.ReadJson(InfoPath);

                    Title = Info.Title.ToLowerInvariant();
                    string Description = Info.Description.ToLowerInvariant();

                    if (Title.Contains(Search) || Description.Contains(Search))
                    {
                        return true;
                    }
                }

                if (Title.Contains(Search))
                {
                    return true;
                }
            }

            return false;
        }

        private async void FullStorePage_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeStore.ItemMargin = new Thickness(SPMM.AdaptiveMargin);
            ThemeStore.MaxItemsPerRow = SPMM.AdaptiveLayout;

            await AddThemes(SPMI.SearchService.SearchText, SPMI.CategoryService.CategoryTag);
        }

        private async void SearchService_SearchTextChanged(object sender, EventArgs e)
        {
            Dispose();

            await AddThemes(SPMI.SearchService.SearchText, SPMI.CategoryService.CategoryTag);
        }

        private async void CategoryService_CategoryTagChanged(object sender, EventArgs e)
        {
            Dispose();

            await AddThemes(SPMI.SearchService.SearchText, SPMI.CategoryService.CategoryTag);
        }

        public void Dispose()
        {
            ThemeStore.Children.Clear();

            GC.Collect();
            GC.SuppressFinalize(this);
        }
    }
}
﻿using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Wpf.Ui.Controls;
using SEST = Skylark.Enum.StorageType;
using SMC = Sucrose.Memory.Constant;
using SMMI = Sucrose.Manager.Manage.Internal;
using SMMM = Sucrose.Manager.Manage.Manager;
using SMR = Sucrose.Memory.Readonly;
using SPVCEC = Sucrose.Portal.Views.Controls.ExpanderCard;
using SRER = Sucrose.Resources.Extension.Resources;
using SSCECT = Sucrose.Shared.Core.Enum.ChannelType;
using SSCEUT = Sucrose.Shared.Core.Enum.UpdateType;
using SSCMM = Sucrose.Shared.Core.Manage.Manager;
using SSSMI = Sucrose.Shared.Store.Manage.Internal;
using TextBlock = System.Windows.Controls.TextBlock;
using TextBox = Wpf.Ui.Controls.TextBox;

namespace Sucrose.Portal.ViewModels.Pages
{
    public partial class OtherSettingViewModel : ObservableObject, INavigationAware, IDisposable
    {
        [ObservableProperty]
        private List<UIElement> _Contents = new();

        private bool _isInitialized;

        public OtherSettingViewModel()
        {
            if (!_isInitialized)
            {
                InitializeViewModel();
            }
        }

        private void InitializeViewModel()
        {
            TextBlock HookArea = new()
            {
                Foreground = SRER.GetResource<Brush>("TextFillColorPrimaryBrush"),
                Text = SRER.GetValue("Portal", "Area", "Hook"),
                Margin = new Thickness(0, 0, 0, 0),
                FontWeight = FontWeights.Bold
            };

            Contents.Add(HookArea);

            SPVCEC DiscordHook = new()
            {
                Margin = new Thickness(0, 10, 0, 0),
                IsExpand = true
            };

            DiscordHook.LeftIcon.Symbol = SymbolRegular.SquareHintApps24;
            DiscordHook.Title.Text = SRER.GetValue("Portal", "OtherSettingPage", "DiscordHook");
            DiscordHook.Description.Text = SRER.GetValue("Portal", "OtherSettingPage", "DiscordHook", "Description");

            ToggleSwitch DiscordState = new()
            {
                IsChecked = SMMM.DiscordState
            };

            DiscordState.Checked += (s, e) => DiscordStateChecked(true);
            DiscordState.Unchecked += (s, e) => DiscordStateChecked(false);

            DiscordHook.HeaderFrame = DiscordState;

            StackPanel DiscordContent = new();

            StackPanel DiscordRefreshContent = new()
            {
                Orientation = Orientation.Horizontal
            };

            TextBlock DiscordRefreshText = new()
            {
                Text = SRER.GetValue("Portal", "OtherSettingPage", "DiscordHook", "DiscordRefresh"),
                Foreground = SRER.GetResource<Brush>("TextFillColorPrimaryBrush"),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0),
                FontWeight = FontWeights.SemiBold
            };

            ToggleSwitch DiscordRefresh = new()
            {
                IsChecked = SMMM.DiscordRefresh
            };

            DiscordRefresh.Checked += (s, e) => DiscordRefreshChecked(true);
            DiscordRefresh.Unchecked += (s, e) => DiscordRefreshChecked(false);

            StackPanel DiscordDelayContent = new()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 10, 0, 0)
            };

            TextBlock DiscordDelayText = new()
            {
                Text = SRER.GetValue("Portal", "OtherSettingPage", "DiscordHook", "DiscordDelay"),
                Foreground = SRER.GetResource<Brush>("TextFillColorPrimaryBrush"),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0),
                FontWeight = FontWeights.SemiBold
            };

            NumberBox DiscordDelay = new()
            {
                Icon = new SymbolIcon(SymbolRegular.TimePicker24),
                IconPlacement = ElementPlacement.Left,
                ClearButtonEnabled = false,
                Value = SMMM.DiscordDelay,
                MaxDecimalPlaces = 0,
                Maximum = 3600,
                MaxLength = 4,
                Minimum = 60
            };

            DiscordDelay.ValueChanged += (s, e) => DiscordDelayChanged(DiscordDelay.Value);

            DiscordRefreshContent.Children.Add(DiscordRefreshText);
            DiscordRefreshContent.Children.Add(DiscordRefresh);

            DiscordDelayContent.Children.Add(DiscordDelayText);
            DiscordDelayContent.Children.Add(DiscordDelay);

            DiscordContent.Children.Add(DiscordRefreshContent);
            DiscordContent.Children.Add(DiscordDelayContent);

            DiscordHook.FooterCard = DiscordContent;

            Contents.Add(DiscordHook);

            TextBlock PriorityArea = new()
            {
                Foreground = SRER.GetResource<Brush>("TextFillColorPrimaryBrush"),
                Text = SRER.GetValue("Portal", "Area", "Priority"),
                Margin = new Thickness(0, 10, 0, 0),
                FontWeight = FontWeights.Bold
            };

            Contents.Add(PriorityArea);

            SPVCEC Agent = new()
            {
                Margin = new Thickness(0, 10, 0, 0),
                Expandable = false
            };

            Agent.LeftIcon.Symbol = SymbolRegular.VideoPersonSparkle24;
            Agent.Title.Text = SRER.GetValue("Portal", "OtherSettingPage", "Agent");
            Agent.Description.Text = SRER.GetValue("Portal", "OtherSettingPage", "Agent", "Description");

            TextBox UserAgent = new()
            {
                Icon = new SymbolIcon(SymbolRegular.PersonHeart24),
                IconPlacement = ElementPlacement.Left,
                ClearButtonEnabled = false,
                Text = SMMM.UserAgent,
                IsReadOnly = true,
                MaxLength = 100,
                MinWidth = 125,
                MaxWidth = 250
            };

            UserAgent.TextChanged += (s, e) => UserAgentChanged(UserAgent);

            Agent.HeaderFrame = UserAgent;

            Contents.Add(Agent);

            SPVCEC Key = new()
            {
                Margin = new Thickness(0, 10, 0, 0)
            };

            Key.LeftIcon.Symbol = SymbolRegular.ShieldKeyhole24;
            Key.Title.Text = SRER.GetValue("Portal", "OtherSettingPage", "Key");
            Key.Description.Text = SRER.GetValue("Portal", "OtherSettingPage", "Key", "Description");

            StackPanel KeyContent = new();

            HyperlinkButton HintKey = new()
            {
                Content = SRER.GetValue("Portal", "OtherSettingPage", "Key", "HintKey"),
                Foreground = SRER.GetResource<Brush>("AccentTextFillColorPrimaryBrush"),
                Appearance = ControlAppearance.Transparent,
                BorderBrush = Brushes.Transparent,
                NavigateUri = SMR.KeyYouTube,
                Cursor = Cursors.Hand
            };

            TextBox PersonalKey = new()
            {
                PlaceholderText = SRER.GetValue("Portal", "OtherSettingPage", "Key", "PersonalKey"),
                Icon = new SymbolIcon(SymbolRegular.PersonKey20),
                HorizontalAlignment = HorizontalAlignment.Left,
                IconPlacement = ElementPlacement.Left,
                Margin = new Thickness(0, 10, 0, 0),
                Text = SMMM.Key,
                MaxLength = 93
            };

            PersonalKey.TextChanged += (s, e) => PersonalKeyChanged(PersonalKey);

            KeyContent.Children.Add(HintKey);
            KeyContent.Children.Add(PersonalKey);

            Key.FooterCard = KeyContent;

            Contents.Add(Key);

            TextBlock UpdateArea = new()
            {
                Foreground = SRER.GetResource<Brush>("TextFillColorPrimaryBrush"),
                Text = SRER.GetValue("Portal", "Area", "Update"),
                Margin = new Thickness(0, 10, 0, 0),
                FontWeight = FontWeights.Bold
            };

            Contents.Add(UpdateArea);

            SPVCEC Channel = new()
            {
                Margin = new Thickness(0, 10, 0, 0),
                Expandable = false
            };

            Channel.LeftIcon.Symbol = SymbolRegular.ChannelShare24;
            Channel.Title.Text = SRER.GetValue("Portal", "OtherSettingPage", "Channel");
            Channel.Description.Text = SRER.GetValue("Portal", "OtherSettingPage", "Channel", "Description");

            ComboBox ChannelType = new();

            ChannelType.SelectionChanged += (s, e) => ChannelTypeSelected(ChannelType.SelectedIndex);

            foreach (SSCECT Type in Enum.GetValues(typeof(SSCECT)))
            {
                ChannelType.Items.Add(SRER.GetValue("Portal", "Enum", "ChannelType", $"{Type}"));
            }

            ChannelType.SelectedIndex = (int)SSCMM.ChannelType;

            Channel.HeaderFrame = ChannelType;

            Contents.Add(Channel);

            SPVCEC Update = new()
            {
                Margin = new Thickness(0, 10, 0, 0),
                IsExpand = true
            };

            Update.LeftIcon.Symbol = SymbolRegular.ArrowSwap24;
            Update.Title.Text = SRER.GetValue("Portal", "OtherSettingPage", "Update");
            Update.Description.Text = SRER.GetValue("Portal", "OtherSettingPage", "Update", "Description");

            ComboBox UpdateType = new();

            UpdateType.SelectionChanged += (s, e) => UpdateTypeSelected(UpdateType.SelectedIndex);

            foreach (SSCEUT Type in Enum.GetValues(typeof(SSCEUT)))
            {
                UpdateType.Items.Add(SRER.GetValue("Portal", "Enum", "UpdateType", $"{Type}"));
            }

            UpdateType.SelectedIndex = (int)SSCMM.UpdateType;

            Update.HeaderFrame = UpdateType;

            StackPanel UpdateContent = new()
            {
                Orientation = Orientation.Horizontal
            };

            TextBlock UpdateLimitText = new()
            {
                Text = SRER.GetValue("Portal", "OtherSettingPage", "Update", "Limit"),
                Foreground = SRER.GetResource<Brush>("TextFillColorPrimaryBrush"),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0),
                FontWeight = FontWeights.SemiBold
            };

            NumberBox UpdateLimit = new()
            {
                Icon = new SymbolIcon(SymbolRegular.ArrowBetweenDown24),
                IconPlacement = ElementPlacement.Left,
                Margin = new Thickness(0, 0, 10, 0),
                Value = SMMM.UpdateLimitValue,
                ClearButtonEnabled = false,
                MaxDecimalPlaces = 0,
                Maximum = 99999999,
                MaxLength = 8,
                Minimum = 0
            };

            UpdateLimit.ValueChanged += (s, e) => UpdateLimitChanged(UpdateLimit.Value);

            ComboBox UpdateLimitType = new()
            {
                MaxDropDownHeight = 200
            };

            ScrollViewer.SetVerticalScrollBarVisibility(UpdateLimitType, ScrollBarVisibility.Auto);

            UpdateLimitType.SelectionChanged += (s, e) => UpdateLimitTypeSelected(UpdateLimitType.SelectedIndex);

            foreach (SEST Type in Enum.GetValues(typeof(SEST)))
            {
                if (Type >= SEST.Megabyte)
                {
                    UpdateLimitType.Items.Add(new ComboBoxItem()
                    {
                        IsEnabled = true,
                        Content = Type
                    });
                }
                else
                {
                    UpdateLimitType.Items.Add(new ComboBoxItem()
                    {
                        IsEnabled = false,
                        Content = Type
                    });
                }
            }

            UpdateLimitType.SelectedIndex = (int)SMMM.DownloadType;

            UpdateContent.Children.Add(UpdateLimitText);
            UpdateContent.Children.Add(UpdateLimit);
            UpdateContent.Children.Add(UpdateLimitType);

            Update.FooterCard = UpdateContent;

            Contents.Add(Update);

            TextBlock DeveloperArea = new()
            {
                Foreground = SRER.GetResource<Brush>("TextFillColorPrimaryBrush"),
                Text = SRER.GetValue("Portal", "Area", "Developer"),
                Margin = new Thickness(0, 10, 0, 0),
                FontWeight = FontWeights.Bold
            };

            Contents.Add(DeveloperArea);

            SPVCEC Developer = new()
            {
                Margin = new Thickness(0, 10, 0, 0),
                Expandable = false
            };

            Developer.LeftIcon.Symbol = SymbolRegular.WindowDevTools24;
            Developer.Title.Text = SRER.GetValue("Portal", "OtherSettingPage", "Developer");
            Developer.Description.Text = SRER.GetValue("Portal", "OtherSettingPage", "Developer", "Description");

            ToggleSwitch DeveloperState = new()
            {
                IsChecked = SMMM.DeveloperMode
            };

            DeveloperState.Checked += (s, e) => DeveloperStateChecked(true);
            DeveloperState.Unchecked += (s, e) => DeveloperStateChecked(false);

            Developer.HeaderFrame = DeveloperState;

            Contents.Add(Developer);

            _isInitialized = true;
        }

        public void OnNavigatedTo()
        {
            //
        }

        public void OnNavigatedFrom()
        {
            //Dispose();
        }

        private void UpdateTypeSelected(int Index)
        {
            if (Index != (int)SSCMM.UpdateType)
            {
                SSCEUT Type = (SSCEUT)Index;

                SMMI.UpdateSettingManager.SetSetting(SMC.UpdateType, Type);
            }
        }

        private void ChannelTypeSelected(int Index)
        {
            if (Index != (int)SSCMM.ChannelType)
            {
                SSCECT Type = (SSCECT)Index;

                SMMI.UpdateSettingManager.SetSetting(SMC.ChannelType, Type);
            }
        }

        private void DiscordStateChecked(bool State)
        {
            SMMI.HookSettingManager.SetSetting(SMC.DiscordState, State);
        }

        private void DiscordRefreshChecked(bool State)
        {
            SMMI.HookSettingManager.SetSetting(SMC.DiscordRefresh, State);
        }

        private void UserAgentChanged(TextBox TextBox)
        {
            if (string.IsNullOrEmpty(TextBox.Text))
            {
                TextBox.Text = SMR.UserAgent;
            }

            SMMI.GeneralSettingManager.SetSetting(SMC.UserAgent, TextBox.Text);
        }

        private void DeveloperStateChecked(bool State)
        {
            SMMI.EngineSettingManager.SetSetting(SMC.DeveloperMode, State);
        }

        private void UpdateLimitChanged(double? Value)
        {
            int NewValue = Convert.ToInt32(Value);

            if (NewValue != SMMM.UpdateLimitValue)
            {
                SMMI.UpdateSettingManager.SetSetting(SMC.UpdateLimitValue, NewValue);
            }
        }

        private void UpdateLimitTypeSelected(int Index)
        {
            if (Index != (int)SMMM.UpdateLimitType)
            {
                SEST Type = (SEST)Index;

                SMMI.UpdateSettingManager.SetSetting(SMC.UpdateLimitType, Type);
            }
        }

        private void DiscordDelayChanged(double? Value)
        {
            int NewValue = Convert.ToInt32(Value);

            if (NewValue != SMMM.DiscordDelay)
            {
                SMMI.HookSettingManager.SetSetting(SMC.DiscordDelay, NewValue);
            }
        }

        private void PersonalKeyChanged(TextBox TextBox)
        {
            if (TextBox.Text.Length == 93)
            {
                SMMI.PrivateSettingManager.SetSetting(SMC.Key, TextBox.Text);
                SSSMI.State = true;
            }
            else
            {
                SMMI.PrivateSettingManager.SetSetting(SMC.Key, SMR.Key);
                TextBox.PlaceholderText = SRER.GetValue("Portal", "OtherSettingPage", "Key", "PersonalKey", "Valid");
            }
        }

        public void Dispose()
        {
            Contents.Clear();

            GC.Collect();
            GC.SuppressFinalize(this);
        }
    }
}
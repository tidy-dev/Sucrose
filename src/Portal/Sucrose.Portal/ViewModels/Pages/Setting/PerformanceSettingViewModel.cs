﻿using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;
using SEST = Skylark.Enum.StorageType;
using SMC = Sucrose.Memory.Constant;
using SMMI = Sucrose.Manager.Manage.Internal;
using SMMM = Sucrose.Manager.Manage.Manager;
using SMR = Sucrose.Memory.Readonly;
using SPMM = Sucrose.Portal.Manage.Manager;
using SPVCEC = Sucrose.Portal.Views.Controls.ExpanderCard;
using SSDECT = Sucrose.Shared.Dependency.Enum.CommandsType;
using SSDEPT = Sucrose.Shared.Dependency.Enum.PerformanceType;
using SSDSHS = Sucrose.Shared.Dependency.Struct.HostStruct;
using SSRER = Sucrose.Shared.Resources.Extension.Resources;
using SSSHN = Sucrose.Shared.Space.Helper.Network;
using SSSHP = Sucrose.Shared.Space.Helper.Processor;
using SSSMI = Sucrose.Shared.Space.Manage.Internal;
using TextBlock = System.Windows.Controls.TextBlock;

namespace Sucrose.Portal.ViewModels.Pages
{
    public partial class PerformanceSettingViewModel : ObservableObject, INavigationAware, IDisposable
    {
        [ObservableProperty]
        private List<UIElement> _Contents = new();

        private bool _isInitialized;

        public PerformanceSettingViewModel()
        {
            if (!_isInitialized)
            {
                InitializeViewModel();
            }
        }

        private void InitializeViewModel()
        {
            TextBlock AppearanceBehaviorArea = new()
            {
                Foreground = SSRER.GetResource<Brush>("TextFillColorPrimaryBrush"),
                Text = SSRER.GetValue("Portal", "Area", "AppearanceBehavior"),
                Margin = new Thickness(0, 0, 0, 0),
                FontWeight = FontWeights.Bold
            };

            Contents.Add(AppearanceBehaviorArea);

            SPVCEC Counter = new()
            {
                Expandable = !SMMM.PerformanceCounter,
                IsExpand = !SMMM.PerformanceCounter,
                Margin = new Thickness(0, 10, 0, 0)
            };

            Counter.LeftIcon.Symbol = SymbolRegular.ShiftsActivity24;
            Counter.Title.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Counter");
            Counter.Description.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Counter", "Description");

            ToggleSwitch CounterState = new()
            {
                IsChecked = SMMM.PerformanceCounter
            };

            CounterState.Checked += (s, e) => CounterStateChecked(Counter, true);
            CounterState.Unchecked += (s, e) => CounterStateChecked(Counter, false);

            Counter.HeaderFrame = CounterState;

            TextBlock CounterHint = new()
            {
                Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Counter", "CounterHint"),
                Foreground = SSRER.GetResource<Brush>("TextFillColorSecondaryBrush"),
                HorizontalAlignment = HorizontalAlignment.Left,
                TextWrapping = TextWrapping.WrapWithOverflow,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(0, 0, 0, 0),
                FontWeight = FontWeights.SemiBold
            };

            Counter.FooterCard = CounterHint;

            Contents.Add(Counter);

            TextBlock SystemResourcesArea = new()
            {
                Foreground = SSRER.GetResource<Brush>("TextFillColorPrimaryBrush"),
                Text = SSRER.GetValue("Portal", "Area", "SystemResources"),
                Margin = new Thickness(0, 10, 0, 0),
                FontWeight = FontWeights.Bold
            };

            Contents.Add(SystemResourcesArea);

            SPVCEC Cpu = new()
            {
                Margin = new Thickness(0, 10, 0, 0)
            };

            Cpu.LeftIcon.Symbol = SymbolRegular.HeartPulse24;
            Cpu.Title.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Cpu");
            Cpu.Description.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Cpu", "Description");

            ComboBox CpuPerformance = new();

            CpuPerformance.SelectionChanged += (s, e) => CpuPerformanceSelected(CpuPerformance.SelectedIndex);

            foreach (SSDEPT Type in Enum.GetValues(typeof(SSDEPT)))
            {
                CpuPerformance.Items.Add(SSRER.GetValue("Portal", "Enum", "PerformanceType", $"{Type}"));
            }

            CpuPerformance.SelectedIndex = (int)SPMM.CpuPerformance;

            Cpu.HeaderFrame = CpuPerformance;

            StackPanel CpuContent = new()
            {
                Orientation = Orientation.Horizontal
            };

            TextBlock CpuUsageText = new()
            {
                Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Cpu", "CpuUsage"),
                Foreground = SSRER.GetResource<Brush>("TextFillColorPrimaryBrush"),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0),
                FontWeight = FontWeights.SemiBold
            };

            NumberBox CpuUsage = new()
            {
                Margin = new Thickness(0, 0, 10, 0),
                ClearButtonEnabled = false,
                Value = SMMM.CpuUsage,
                MaxDecimalPlaces = 0,
                Maximum = 100,
                MaxLength = 3,
                Minimum = 0
            };

            CpuUsage.ValueChanged += (s, e) => CpuUsageChanged(CpuUsage.Value);

            CpuContent.Children.Add(CpuUsageText);
            CpuContent.Children.Add(CpuUsage);

            Cpu.FooterCard = CpuContent;

            Contents.Add(Cpu);

            SPVCEC Memory = new()
            {
                Margin = new Thickness(0, 10, 0, 0)
            };

            Memory.LeftIcon.Symbol = SymbolRegular.Memory16;
            Memory.Title.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Memory");
            Memory.Description.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Memory", "Description");

            ComboBox MemoryPerformance = new();

            MemoryPerformance.SelectionChanged += (s, e) => MemoryPerformanceSelected(MemoryPerformance.SelectedIndex);

            foreach (SSDEPT Type in Enum.GetValues(typeof(SSDEPT)))
            {
                MemoryPerformance.Items.Add(SSRER.GetValue("Portal", "Enum", "PerformanceType", $"{Type}"));
            }

            MemoryPerformance.SelectedIndex = (int)SPMM.MemoryPerformance;

            Memory.HeaderFrame = MemoryPerformance;

            StackPanel MemoryContent = new()
            {
                Orientation = Orientation.Horizontal
            };

            TextBlock MemoryUsageText = new()
            {
                Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Memory", "MemoryUsage"),
                Foreground = SSRER.GetResource<Brush>("TextFillColorPrimaryBrush"),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0),
                FontWeight = FontWeights.SemiBold
            };

            NumberBox MemoryUsage = new()
            {
                Margin = new Thickness(0, 0, 10, 0),
                ClearButtonEnabled = false,
                Value = SMMM.MemoryUsage,
                MaxDecimalPlaces = 0,
                Maximum = 100,
                MaxLength = 3,
                Minimum = 0
            };

            MemoryUsage.ValueChanged += (s, e) => MemoryUsageChanged(MemoryUsage.Value);

            MemoryContent.Children.Add(MemoryUsageText);
            MemoryContent.Children.Add(MemoryUsage);

            Memory.FooterCard = MemoryContent;

            Contents.Add(Memory);

            SPVCEC Network = new()
            {
                Margin = new Thickness(0, 10, 0, 0)
            };

            Network.LeftIcon.Symbol = SymbolRegular.NetworkCheck24;
            Network.Title.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Network");
            Network.Description.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Network", "Description");

            ComboBox NetworkPerformance = new();

            NetworkPerformance.SelectionChanged += (s, e) => NetworkPerformanceSelected(NetworkPerformance.SelectedIndex);

            foreach (SSDEPT Type in Enum.GetValues(typeof(SSDEPT)))
            {
                NetworkPerformance.Items.Add(SSRER.GetValue("Portal", "Enum", "PerformanceType", $"{Type}"));
            }

            NetworkPerformance.SelectedIndex = (int)SPMM.NetworkPerformance;

            Network.HeaderFrame = NetworkPerformance;

            StackPanel NetworkContent = new();

            StackPanel NetworkAdapterContent = new()
            {
                Orientation = Orientation.Horizontal
            };

            TextBlock NetworkAdapterText = new()
            {
                Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Network", "NetworkAdapter"),
                Foreground = SSRER.GetResource<Brush>("TextFillColorPrimaryBrush"),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0),
                FontWeight = FontWeights.SemiBold
            };

            ComboBox NetworkAdapter = new()
            {
                MaxWidth = 700
            };

            if (SMMM.NetworkInterfaces.Any())
            {
                NetworkAdapter.SelectionChanged += (s, e) => NetworkAdapterSelected($"{NetworkAdapter.SelectedValue}");

                foreach (string Interface in SMMM.NetworkInterfaces)
                {
                    NetworkAdapter.Items.Add(Interface);
                }

                string SelectedAdapter = SMMM.NetworkAdapter;

                if (string.IsNullOrEmpty(SelectedAdapter))
                {
                    NetworkAdapter.SelectedIndex = 0;
                }
                else
                {
                    NetworkAdapter.SelectedValue = SelectedAdapter;
                }
            }
            else
            {
                NetworkAdapter.Items.Add(new ComboBoxItem()
                {
                    Content = SSRER.GetValue("Portal", "PerformanceSettingPage", "Network", "NetworkAdapter", "Empty"),
                    IsSelected = true
                });
            }

            StackPanel NetworkUploadContent = new()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 10, 0, 0)
            };

            TextBlock NetworkUploadText = new()
            {
                Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Network", "NetworkUpload"),
                Foreground = SSRER.GetResource<Brush>("TextFillColorPrimaryBrush"),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0),
                FontWeight = FontWeights.SemiBold
            };

            NumberBox NetworkUpload = new()
            {
                Margin = new Thickness(0, 0, 10, 0),
                ClearButtonEnabled = false,
                Value = SMMM.UploadValue,
                MaxDecimalPlaces = 0,
                Maximum = 99999999,
                MaxLength = 8,
                Minimum = 0
            };

            NetworkUpload.ValueChanged += (s, e) => NetworkUploadChanged(NetworkUpload.Value);

            ComboBox NetworkUploadType = new()
            {
                MaxDropDownHeight = 200
            };

            ScrollViewer.SetVerticalScrollBarVisibility(NetworkUploadType, ScrollBarVisibility.Auto);

            NetworkUploadType.SelectionChanged += (s, e) => NetworkUploadTypeSelected(NetworkUploadType.SelectedIndex);

            foreach (SEST Type in Enum.GetValues(typeof(SEST)))
            {
                NetworkUploadType.Items.Add(Type);
            }

            NetworkUploadType.SelectedIndex = (int)SMMM.UploadType;

            StackPanel NetworkDownloadContent = new()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 10, 0, 0)
            };

            TextBlock NetworkDownloadText = new()
            {
                Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Network", "NetworkDownload"),
                Foreground = SSRER.GetResource<Brush>("TextFillColorPrimaryBrush"),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0),
                FontWeight = FontWeights.SemiBold
            };

            NumberBox NetworkDownload = new()
            {
                Margin = new Thickness(0, 0, 10, 0),
                ClearButtonEnabled = false,
                Value = SMMM.DownloadValue,
                MaxDecimalPlaces = 0,
                Maximum = 99999999,
                MaxLength = 8,
                Minimum = 0
            };

            NetworkDownload.ValueChanged += (s, e) => NetworkDownloadChanged(NetworkDownload.Value);

            ComboBox NetworkDownloadType = new()
            {
                MaxDropDownHeight = 200
            };

            ScrollViewer.SetVerticalScrollBarVisibility(NetworkDownloadType, ScrollBarVisibility.Auto);

            NetworkDownloadType.SelectionChanged += (s, e) => NetworkDownloadTypeSelected(NetworkDownloadType.SelectedIndex);

            foreach (SEST Type in Enum.GetValues(typeof(SEST)))
            {
                NetworkDownloadType.Items.Add(Type);
            }

            NetworkDownloadType.SelectedIndex = (int)SMMM.DownloadType;

            StackPanel NetworkPingContent = new()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 10, 0, 0)
            };

            TextBlock NetworkPingText = new()
            {
                Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Network", "NetworkPing"),
                Foreground = SSRER.GetResource<Brush>("TextFillColorPrimaryBrush"),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0),
                FontWeight = FontWeights.SemiBold
            };

            NumberBox NetworkPing = new()
            {
                Margin = new Thickness(0, 0, 10, 0),
                ClearButtonEnabled = false,
                Value = SMMM.PingValue,
                MaxDecimalPlaces = 0,
                Maximum = 1000,
                MaxLength = 4,
                Minimum = 0
            };

            NetworkPing.ValueChanged += (s, e) => NetworkPingChanged(NetworkPing.Value);

            ComboBox NetworkPingType = new()
            {
                MaxDropDownHeight = 200
            };

            ScrollViewer.SetVerticalScrollBarVisibility(NetworkPingType, ScrollBarVisibility.Auto);

            NetworkPingType.SelectionChanged += (s, e) => NetworkPingTypeSelected($"{NetworkPingType.SelectedValue}");

            foreach (SSDSHS Host in SSSHN.GetHost())
            {
                NetworkPingType.Items.Add(Host.Name);
            }

            NetworkPingType.SelectedValue = SMMM.PingType;

            NetworkAdapterContent.Children.Add(NetworkAdapterText);
            NetworkAdapterContent.Children.Add(NetworkAdapter);

            NetworkUploadContent.Children.Add(NetworkUploadText);
            NetworkUploadContent.Children.Add(NetworkUpload);
            NetworkUploadContent.Children.Add(NetworkUploadType);

            NetworkDownloadContent.Children.Add(NetworkDownloadText);
            NetworkDownloadContent.Children.Add(NetworkDownload);
            NetworkDownloadContent.Children.Add(NetworkDownloadType);

            NetworkPingContent.Children.Add(NetworkPingText);
            NetworkPingContent.Children.Add(NetworkPing);
            NetworkPingContent.Children.Add(NetworkPingType);

            NetworkContent.Children.Add(NetworkAdapterContent);
            NetworkContent.Children.Add(NetworkUploadContent);
            NetworkContent.Children.Add(NetworkDownloadContent);
            NetworkContent.Children.Add(NetworkPingContent);

            Network.FooterCard = NetworkContent;

            Contents.Add(Network);

            TextBlock LaptopArea = new()
            {
                Foreground = SSRER.GetResource<Brush>("TextFillColorPrimaryBrush"),
                Text = SSRER.GetValue("Portal", "Area", "Laptop"),
                Margin = new Thickness(0, 10, 0, 0),
                FontWeight = FontWeights.Bold
            };

            Contents.Add(LaptopArea);

            SPVCEC Battery = new()
            {
                Margin = new Thickness(0, 10, 0, 0)
            };

            Battery.LeftIcon.Symbol = BatterySymbol(SMMM.BatteryUsage);
            Battery.Title.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Battery");
            Battery.Description.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Battery", "Description");

            ComboBox BatteryPerformance = new();

            BatteryPerformance.SelectionChanged += (s, e) => BatteryPerformanceSelected(BatteryPerformance.SelectedIndex);

            foreach (SSDEPT Type in Enum.GetValues(typeof(SSDEPT)))
            {
                BatteryPerformance.Items.Add(SSRER.GetValue("Portal", "Enum", "PerformanceType", $"{Type}"));
            }

            BatteryPerformance.SelectedIndex = (int)SPMM.BatteryPerformance;

            Battery.HeaderFrame = BatteryPerformance;

            StackPanel BatteryContent = new()
            {
                Orientation = Orientation.Horizontal
            };

            TextBlock BatteryUsageText = new()
            {
                Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Battery", "BatteryUsage"),
                Foreground = SSRER.GetResource<Brush>("TextFillColorPrimaryBrush"),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0),
                FontWeight = FontWeights.SemiBold
            };

            NumberBox BatteryUsage = new()
            {
                Margin = new Thickness(0, 0, 10, 0),
                ClearButtonEnabled = false,
                Value = SMMM.BatteryUsage,
                MaxDecimalPlaces = 0,
                Maximum = 100,
                MaxLength = 3,
                Minimum = 0
            };

            BatteryUsage.ValueChanged += (s, e) => BatteryUsageChanged(Battery, BatteryUsage.Value);

            BatteryContent.Children.Add(BatteryUsageText);
            BatteryContent.Children.Add(BatteryUsage);

            Battery.FooterCard = BatteryContent;

            Contents.Add(Battery);

            SPVCEC Saver = new()
            {
                Margin = new Thickness(0, 10, 0, 0),
                Expandable = false
            };

            Saver.LeftIcon.Symbol = SymbolRegular.BatterySaver24;
            Saver.Title.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Saver");
            Saver.Description.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Saver", "Description");

            ComboBox SaverPerformance = new();

            SaverPerformance.SelectionChanged += (s, e) => SaverPerformanceSelected(SaverPerformance.SelectedIndex);

            foreach (SSDEPT Type in Enum.GetValues(typeof(SSDEPT)))
            {
                SaverPerformance.Items.Add(SSRER.GetValue("Portal", "Enum", "PerformanceType", $"{Type}"));
            }

            SaverPerformance.SelectedIndex = (int)SPMM.SaverPerformance;

            Saver.HeaderFrame = SaverPerformance;

            Contents.Add(Saver);

            TextBlock SystemArea = new()
            {
                Foreground = SSRER.GetResource<Brush>("TextFillColorPrimaryBrush"),
                Text = SSRER.GetValue("Portal", "Area", "System"),
                Margin = new Thickness(0, 10, 0, 0),
                FontWeight = FontWeights.Bold
            };

            Contents.Add(SystemArea);

            SPVCEC Virtual = new()
            {
                Margin = new Thickness(0, 10, 0, 0),
                Expandable = false
            };

            Virtual.LeftIcon.Symbol = SymbolRegular.DesktopCheckmark24;
            Virtual.Title.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Virtual");
            Virtual.Description.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Virtual", "Description");

            ComboBox VirtualPerformance = new();

            VirtualPerformance.SelectionChanged += (s, e) => VirtualPerformanceSelected(VirtualPerformance.SelectedIndex);

            foreach (SSDEPT Type in Enum.GetValues(typeof(SSDEPT)))
            {
                VirtualPerformance.Items.Add(SSRER.GetValue("Portal", "Enum", "PerformanceType", $"{Type}"));
            }

            VirtualPerformance.SelectedIndex = (int)SPMM.VirtualPerformance;

            Virtual.HeaderFrame = VirtualPerformance;

            Contents.Add(Virtual);

            SPVCEC Remote = new()
            {
                Margin = new Thickness(0, 10, 0, 0),
                Expandable = false
            };

            Remote.LeftIcon.Symbol = SymbolRegular.Remote20;
            Remote.Title.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Remote");
            Remote.Description.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Remote", "Description");

            ComboBox RemotePerformance = new();

            RemotePerformance.SelectionChanged += (s, e) => RemotePerformanceSelected(RemotePerformance.SelectedIndex);

            foreach (SSDEPT Type in Enum.GetValues(typeof(SSDEPT)))
            {
                RemotePerformance.Items.Add(SSRER.GetValue("Portal", "Enum", "PerformanceType", $"{Type}"));
            }

            RemotePerformance.SelectedIndex = (int)SPMM.RemotePerformance;

            Remote.HeaderFrame = RemotePerformance;

            Contents.Add(Remote);

            SPVCEC Fullscreen = new()
            {
                Margin = new Thickness(0, 10, 0, 0),
                Expandable = false
            };

            Fullscreen.LeftIcon.Symbol = SymbolRegular.FullScreenMaximize24;
            Fullscreen.Title.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Fullscreen");
            Fullscreen.Description.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Fullscreen", "Description");

            ComboBox FullscreenPerformance = new();

            FullscreenPerformance.SelectionChanged += (s, e) => FullscreenPerformanceSelected(FullscreenPerformance.SelectedIndex);

            foreach (SSDEPT Type in Enum.GetValues(typeof(SSDEPT)))
            {
                FullscreenPerformance.Items.Add(SSRER.GetValue("Portal", "Enum", "PerformanceType", $"{Type}"));
            }

            FullscreenPerformance.SelectedIndex = (int)SPMM.FullscreenPerformance;

            Fullscreen.HeaderFrame = FullscreenPerformance;

            Contents.Add(Fullscreen);

            SPVCEC Focus = new()
            {
                Margin = new Thickness(0, 10, 0, 0),
                Expandable = false
            };

            Focus.LeftIcon.Symbol = SymbolRegular.DesktopCursor24;
            Focus.Title.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Focus");
            Focus.Description.Text = SSRER.GetValue("Portal", "PerformanceSettingPage", "Focus", "Description");

            ComboBox FocusPerformance = new();

            FocusPerformance.SelectionChanged += (s, e) => FocusPerformanceSelected(FocusPerformance.SelectedIndex);

            foreach (SSDEPT Type in Enum.GetValues(typeof(SSDEPT)))
            {
                FocusPerformance.Items.Add(SSRER.GetValue("Portal", "Enum", "PerformanceType", $"{Type}"));
            }

            FocusPerformance.SelectedIndex = (int)SPMM.FocusPerformance;

            Focus.HeaderFrame = FocusPerformance;

            Contents.Add(Focus);

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

        private SymbolRegular BatterySymbol(int Value)
        {
            if (Value <= 0)
            {
                return SymbolRegular.Battery024;
            }
            else if (Value <= 10)
            {
                return SymbolRegular.Battery124;
            }
            else if (Value <= 20)
            {
                return SymbolRegular.Battery224;
            }
            else if (Value <= 30)
            {
                return SymbolRegular.Battery324;
            }
            else if (Value <= 40)
            {
                return SymbolRegular.Battery424;
            }
            else if (Value <= 50)
            {
                return SymbolRegular.Battery524;
            }
            else if (Value <= 60)
            {
                return SymbolRegular.Battery624;
            }
            else if (Value <= 70)
            {
                return SymbolRegular.Battery724;
            }
            else if (Value <= 80)
            {
                return SymbolRegular.Battery824;
            }
            else if (Value <= 90)
            {
                return SymbolRegular.Battery924;
            }
            else
            {
                return SymbolRegular.Battery1024;
            }
        }

        private void CpuUsageChanged(double? Value)
        {
            int NewValue = Convert.ToInt32(Value);

            if (NewValue != SMMM.CpuUsage)
            {
                SMMI.BackgroundogSettingManager.SetSetting(SMC.CpuUsage, NewValue);
            }
        }

        private void CpuPerformanceSelected(int Index)
        {
            if (Index != (int)SPMM.CpuPerformance)
            {
                SSDEPT Type = (SSDEPT)Index;

                SMMI.BackgroundogSettingManager.SetSetting(SMC.CpuPerformance, Type);
            }
        }

        private void MemoryUsageChanged(double? Value)
        {
            int NewValue = Convert.ToInt32(Value);

            if (NewValue != SMMM.MemoryUsage)
            {
                SMMI.BackgroundogSettingManager.SetSetting(SMC.MemoryUsage, NewValue);
            }
        }

        private void NetworkPingChanged(double? Value)
        {
            int NewValue = Convert.ToInt32(Value);

            if (NewValue != SMMM.PingValue)
            {
                SMMI.BackgroundogSettingManager.SetSetting(SMC.PingValue, NewValue);
            }
        }

        private void NetworkUploadChanged(double? Value)
        {
            int NewValue = Convert.ToInt32(Value);

            if (NewValue != SMMM.UploadValue)
            {
                SMMI.BackgroundogSettingManager.SetSetting(SMC.UploadValue, NewValue);
            }
        }

        private void SaverPerformanceSelected(int Index)
        {
            if (Index != (int)SPMM.SaverPerformance)
            {
                SSDEPT Type = (SSDEPT)Index;

                SMMI.BackgroundogSettingManager.SetSetting(SMC.SaverPerformance, Type);
            }
        }

        private void FocusPerformanceSelected(int Index)
        {
            if (Index != (int)SPMM.FocusPerformance)
            {
                SSDEPT Type = (SSDEPT)Index;

                SMMI.BackgroundogSettingManager.SetSetting(SMC.FocusPerformance, Type);
            }
        }

        private void RemotePerformanceSelected(int Index)
        {
            if (Index != (int)SPMM.RemotePerformance)
            {
                SSDEPT Type = (SSDEPT)Index;

                SMMI.BackgroundogSettingManager.SetSetting(SMC.RemotePerformance, Type);
            }
        }

        private void NetworkUploadTypeSelected(int Index)
        {
            if (Index != (int)SMMM.UploadType)
            {
                SEST Type = (SEST)Index;

                SMMI.BackgroundogSettingManager.SetSetting(SMC.UploadType, Type);
            }
        }

        private void NetworkAdapterSelected(string Value)
        {
            if (Value != SMMM.NetworkAdapter)
            {
                SMMI.BackgroundogSettingManager.SetSetting(SMC.NetworkAdapter, Value);
            }
        }

        private void MemoryPerformanceSelected(int Index)
        {
            if (Index != (int)SPMM.MemoryPerformance)
            {
                SSDEPT Type = (SSDEPT)Index;

                SMMI.BackgroundogSettingManager.SetSetting(SMC.MemoryPerformance, Type);
            }
        }

        private void VirtualPerformanceSelected(int Index)
        {
            if (Index != (int)SPMM.VirtualPerformance)
            {
                SSDEPT Type = (SSDEPT)Index;

                SMMI.BackgroundogSettingManager.SetSetting(SMC.VirtualPerformance, Type);
            }
        }

        private void NetworkPingTypeSelected(string Value)
        {
            if (Value != SMMM.PingType)
            {
                SMMI.BackgroundogSettingManager.SetSetting(SMC.PingType, Value);
            }
        }

        private void NetworkPerformanceSelected(int Index)
        {
            if (Index != (int)SPMM.NetworkPerformance)
            {
                SSDEPT Type = (SSDEPT)Index;

                SMMI.BackgroundogSettingManager.SetSetting(SMC.NetworkPerformance, Type);
            }
        }

        private void BatteryPerformanceSelected(int Index)
        {
            if (Index != (int)SPMM.BatteryPerformance)
            {
                SSDEPT Type = (SSDEPT)Index;

                SMMI.BackgroundogSettingManager.SetSetting(SMC.BatteryPerformance, Type);
            }
        }

        private void NetworkDownloadChanged(double? Value)
        {
            int NewValue = Convert.ToInt32(Value);

            if (NewValue != SMMM.DownloadValue)
            {
                SMMI.BackgroundogSettingManager.SetSetting(SMC.DownloadValue, NewValue);
            }
        }

        private void NetworkDownloadTypeSelected(int Index)
        {
            if (Index != (int)SMMM.DownloadType)
            {
                SEST Type = (SEST)Index;

                SMMI.BackgroundogSettingManager.SetSetting(SMC.DownloadType, Type);
            }
        }

        private void FullscreenPerformanceSelected(int Index)
        {
            if (Index != (int)SPMM.FullscreenPerformance)
            {
                SSDEPT Type = (SSDEPT)Index;

                SMMI.BackgroundogSettingManager.SetSetting(SMC.FullscreenPerformance, Type);
            }
        }

        private void CounterStateChecked(SPVCEC Battery, bool State)
        {
            Battery.IsExpand = !State;
            Battery.Expandable = !State;
            SMMI.BackgroundogSettingManager.SetSetting(SMC.PerformanceCounter, State);

            if (State)
            {
                SSSHP.Run(SSSMI.Commandog, $"{SMR.StartCommand}{SSDECT.Backgroundog}{SMR.ValueSeparator}{SSSMI.Backgroundog}");
            }
            else
            {
                if (SSSHP.Work(SMR.Backgroundog))
                {
                    SSSHP.Kill(SMR.Backgroundog);
                }
            }
        }

        private void BatteryUsageChanged(SPVCEC Battery, double? Value)
        {
            int NewValue = Convert.ToInt32(Value);

            if (NewValue != SMMM.BatteryUsage)
            {
                Battery.LeftIcon.Symbol = BatterySymbol(NewValue);

                SMMI.BackgroundogSettingManager.SetSetting(SMC.BatteryUsage, NewValue);
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
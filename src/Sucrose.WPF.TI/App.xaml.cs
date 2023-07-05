﻿using System.Windows;
using Application = System.Windows.Application;
using SCSTISS = Sucrose.Common.Services.TrayIconServerService;
using SELLT = Skylark.Enum.LevelLogType;
using SEWTT = Skylark.Enum.WindowsThemeType;
using SGCTI = Sucrose.Grpc.Common.TrayIcon;
using SGMR = Sucrose.Globalization.Manage.Resources;
using SGSGSS = Sucrose.Grpc.Services.GeneralServerService;
using SMC = Sucrose.Memory.Constant;
using SMMI = Sucrose.Manager.Manage.Internal;
using SMR = Sucrose.Memory.Readonly;
using STCI = Sucrose.Tray.Command.Interface;
using STMI = Sucrose.Tray.Manage.Internal;
using SWDEMB = Sucrose.Watchdog.DarkErrorMessageBox;
using SWHWT = Skylark.Wing.Helper.WindowsTheme;
using SWLEMB = Sucrose.Watchdog.LightErrorMessageBox;
using SWW = Sucrose.Watchdog.Watch;

namespace Sucrose.WPF.TI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string Culture { get; set; } = SMMI.GeneralSettingManager.GetSetting(SMC.CultureName, SGMR.CultureInfo.Name);

        private static SEWTT Theme { get; set; } = SMMI.GeneralSettingManager.GetSetting(SMC.ThemeType, SWHWT.GetTheme());

        private static bool Visible { get; set; } = SMMI.TrayIconSettingManager.GetSetting(SMC.Visible, true);

        private static Mutex Mutex { get; } = new(true, SMR.TrayIconMutex);

        private static bool HasStart { get; set; } = false;

        public App()
        {
            System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);

            System.Windows.Forms.Application.ThreadException += (s, e) =>
            {
                Exception Exception = e.Exception;

                SWW.Watch_ThreadException(Exception);

                //Close();
                Message(Exception.Message);
            };

            AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
            {
                Exception Exception = e.Exception;

                SWW.Watch_FirstChanceException(Exception);

                //Close();
                //Message(Exception.Message);
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Exception Exception = (Exception)e.ExceptionObject;

                SWW.Watch_GlobalUnhandledExceptionHandler(Exception);

                //Close();
                Message(Exception.Message);
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                Exception Exception = e.Exception;

                SWW.Watch_UnobservedTaskException(Exception);

                e.SetObserved();

                //Close();
                Message(Exception.Message);
            };

            Current.DispatcherUnhandledException += (s, e) =>
            {
                Exception Exception = e.Exception;

                SWW.Watch_DispatcherUnhandledException(Exception);

                e.Handled = true;

                //Close();
                Message(Exception.Message);
            };
        }

        protected void Close()
        {
            SMMI.TrayIconLogManager.Log(SELLT.Error, $"Application has been closed.");

            Environment.Exit(0);
            Current.Shutdown();
            Shutdown();
        }

        protected void Message(string Message)
        {
            if (HasStart)
            {
                HasStart = false;

                string Path = SMMI.TrayIconLogManager.LogFile();

                switch (Theme)
                {
                    case SEWTT.Dark:
                        SWDEMB DarkMessageBox = new(Culture, Message, Path);
                        DarkMessageBox.ShowDialog();
                        break;
                    default:
                        SWLEMB LightMessageBox = new(Culture, Message, Path);
                        LightMessageBox.ShowDialog();
                        break;
                }

                Close();
            }
            else
            {
                Close();
            }
        }

        protected void Configure()
        {
            SMMI.TrayIconLogManager.Log(SELLT.Info, "Configuration initializing..");

            STMI.TrayIconManager.Start(Theme, Culture);

            SGSGSS.ServerCreate(SGCTI.BindService(new SCSTISS()));

            SMMI.TrayIconSettingManager.SetSetting(SMC.Host, SGSGSS.Host);
            SMMI.TrayIconSettingManager.SetSetting(SMC.Port, SGSGSS.Port);

            SMMI.TrayIconLogManager.Log(SELLT.Info, "Configuration initialized..");

            SMMI.TrayIconLogManager.Log(SELLT.Info, "Server initializing..");

            SGSGSS.ServerInstance.Start();

            SMMI.TrayIconLogManager.Log(SELLT.Info, "Server initialized..");

            HasStart = true;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            SGSGSS.ServerInstance.KillAsync().Wait();
            //SGSGSS.ServerInstance.ShutdownAsync().Wait();

            STMI.TrayIconManager.Dispose();

            Close();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SMMI.TrayIconLogManager.Log(SELLT.Info, "Application initializing..");

            if (Mutex.WaitOne(TimeSpan.Zero, true) && Visible)
            {
                SMMI.TrayIconLogManager.Log(SELLT.Info, "Application mutex is being releasing.");

                Mutex.ReleaseMutex();

                SMMI.TrayIconLogManager.Log(SELLT.Info, "Application mutex is being released.");

                Configure();

                SMMI.TrayIconLogManager.Log(SELLT.Info, "Application initialized..");
            }
            else
            {
                SMMI.TrayIconLogManager.Log(SELLT.Warning, "Application could not be initialized!");

                SMMI.TrayIconLogManager.Log(SELLT.Info, "Application Interface opening..");

                STCI.Command();

                SMMI.TrayIconLogManager.Log(SELLT.Info, "Application Interface opened..");

                Close();
            }
        }
    }
}
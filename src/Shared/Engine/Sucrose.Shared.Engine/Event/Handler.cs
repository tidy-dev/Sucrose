﻿using System.Windows;
using SEDST = Skylark.Enum.DisplayScreenType;
using SSDSHS = Sucrose.Shared.Dependency.Struct.HandleStruct;
using SSEHD = Sucrose.Shared.Engine.Helper.Data;
using SSEMI = Sucrose.Shared.Engine.Manage.Internal;
using SWE = Skylark.Wing.Engine;
using SWHWI = Skylark.Wing.Helper.WindowInterop;
using SWHWO = Skylark.Wing.Helper.WindowOperations;
using SWNM = Skylark.Wing.Native.Methods;
using SWUS = Skylark.Wing.Utility.Screene;

namespace Sucrose.Shared.Engine.Event
{
    internal static class Handler
    {
        public static void WindowLoaded(Window Window)
        {
            IntPtr Handle = SWHWI.Handle(Window);

            //ShowInTaskbar = false : causing issue with Windows10-Windows11 Taskview.
            SWHWO.RemoveWindowFromTaskbar(Handle);

            //this hides the window from taskbar and also fixes crash when Win10-Win11 taskview is launched. 
            Window.ShowInTaskbar = true;
            Window.ShowInTaskbar = false;
        }

        public static void ContentRendered(Window Window)
        {
            switch (SSEHD.GetDisplayScreenType())
            {
                case SEDST.SpanAcross:
                    SWE.WallpaperWindow(Window, SSEHD.GetExpandScreenType(), SSEHD.GetScreenType());
                    break;
                case SEDST.SameDuplicate:
                    SWE.WallpaperWindow(Window, SSEHD.GetDuplicateScreenType(), SSEHD.GetScreenType());
                    break;
                default:
                    SWE.WallpaperWindow(Window, SSEHD.GetScreenIndex(), SSEHD.GetScreenType());
                    break;
            }
        }

        public static void ApplicationLoaded(SSDSHS Application)
        {
            //ShowInTaskbar = false : causing issue with Windows10-Windows11 Taskview.
            SWHWO.RemoveWindowFromTaskbar(Application.Handle);
            SWHWO.RemoveWindowFromTaskbar(Application.MainWindowHandle);

            SWNM.ShowWindow(Application.Handle, (int)SWNM.SHOWWINDOW.SW_HIDE);
            SWNM.ShowWindow(Application.MainWindowHandle, (int)SWNM.SHOWWINDOW.SW_HIDE);

            int Style = SWNM.GetWindowLong(Application.MainWindowHandle, (int)SWNM.GWL.GWL_STYLE);
            SWNM.SetWindowLong(Application.MainWindowHandle, (int)SWNM.GWL.GWL_STYLE, Style & ~((int)SWNM.WindowStyles.WS_CAPTION | (int)SWNM.WindowStyles.WS_THICKFRAME | (int)SWNM.WindowStyles.WS_MINIMIZE | (int)SWNM.WindowStyles.WS_MAXIMIZE | (int)SWNM.WindowStyles.WS_SYSMENU | (int)SWNM.WindowStyles.WS_DLGFRAME | (int)SWNM.WindowStyles.WS_BORDER | (int)SWNM.WindowStyles.WS_EX_CLIENTEDGE));

            int MainWindowStyle = SWNM.GetWindowLong(Application.MainWindowHandle, (int)SWNM.GWL.GWL_STYLE);
            SWNM.SetWindowLong(Application.MainWindowHandle, (int)SWNM.GWL.GWL_STYLE, MainWindowStyle & ~((int)SWNM.WindowStyles.WS_CAPTION | (int)SWNM.WindowStyles.WS_THICKFRAME | (int)SWNM.WindowStyles.WS_MINIMIZE | (int)SWNM.WindowStyles.WS_MAXIMIZE | (int)SWNM.WindowStyles.WS_SYSMENU | (int)SWNM.WindowStyles.WS_DLGFRAME | (int)SWNM.WindowStyles.WS_BORDER | (int)SWNM.WindowStyles.WS_EX_CLIENTEDGE));

            SWHWO.BorderlessWinStyle(Application.Handle);
            SWHWO.BorderlessWinStyle(Application.MainWindowHandle);
        }

        public static void ApplicationRendered(SSDSHS Application)
        {
            switch (SSEHD.GetDisplayScreenType())
            {
                case SEDST.SpanAcross:
                    SWE.WallpaperProcess(Application.Process, SSEHD.GetExpandScreenType(), SSEHD.GetScreenType());
                    break;
                case SEDST.SameDuplicate:
                    SSEMI.Applications.ForEach(Application => SWE.WallpaperProcess(Application.Process, SSEMI.Applications.IndexOf(Application), SSEHD.GetScreenType()));
                    break;
                default:
                    SWE.WallpaperProcess(Application.Process, SSEHD.GetScreenIndex(), SSEHD.GetScreenType());
                    break;
            }

            SWNM.ShowWindow(Application.MainWindowHandle, (int)SWNM.SHOWWINDOW.SW_SHOW);
        }

        public static async void DisplaySettingsChanged(Window Window, DateTime DisplayChanged)
        {
            SSEMI.DisplayChanged = DisplayChanged;

            Window.Hide();

            while (DateTime.Now - SSEMI.DisplayChanged < TimeSpan.FromSeconds(2))
            {
                await Task.Delay(500);

                if (SSEMI.DisplayChanged != DisplayChanged)
                {
                    return;
                }
            }

            SWUS.Initialize();

            while (DateTime.Now - SSEMI.DisplayChanged < TimeSpan.FromSeconds(2.5))
            {
                await Task.Delay(500);

                if (SSEMI.DisplayChanged != DisplayChanged)
                {
                    return;
                }
            }

            ContentRendered(Window);

            Window.Show();
        }

        public static async void DisplaySettingsChanged(SSDSHS Application, DateTime DisplayChanged)
        {
            SSEMI.DisplayChanged = DisplayChanged;

            SWNM.ShowWindow(Application.Handle, (int)SWNM.SHOWWINDOW.SW_HIDE);
            SWNM.ShowWindow(Application.MainWindowHandle, (int)SWNM.SHOWWINDOW.SW_HIDE);

            while (DateTime.Now - SSEMI.DisplayChanged < TimeSpan.FromSeconds(2))
            {
                await Task.Delay(500);

                if (SSEMI.DisplayChanged != DisplayChanged)
                {
                    return;
                }
            }

            SWUS.Initialize();

            while (DateTime.Now - SSEMI.DisplayChanged < TimeSpan.FromSeconds(2.5))
            {
                await Task.Delay(500);

                if (SSEMI.DisplayChanged != DisplayChanged)
                {
                    return;
                }
            }

            ApplicationRendered(Application);

            SWNM.ShowWindow(Application.Handle, (int)SWNM.SHOWWINDOW.SW_SHOW);
            SWNM.ShowWindow(Application.MainWindowHandle, (int)SWNM.SHOWWINDOW.SW_SHOW);
        }
    }
}
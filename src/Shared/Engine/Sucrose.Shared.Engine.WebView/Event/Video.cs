﻿using Microsoft.Web.WebView2.Core;
using SSEHD = Sucrose.Shared.Engine.Helper.Data;
using SSEHS = Sucrose.Shared.Engine.Helper.Source;
using SSEMI = Sucrose.Shared.Engine.Manage.Internal;
using SSEWVHV = Sucrose.Shared.Engine.WebView.Helper.Video;
using SSEWVMI = Sucrose.Shared.Engine.WebView.Manage.Internal;

namespace Sucrose.Shared.Engine.WebView.Event
{
    internal static class Video
    {
        public static void WebEngineDOMContentLoaded(object sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
            SSEWVMI.WebEngine.CoreWebView2.ExecuteScriptAsync("document.getElementsByTagName('video')[0].requestFullscreen();");
            SSEWVMI.WebEngine.CoreWebView2.ExecuteScriptAsync("document.getElementsByTagName('video')[0].controls = false;");
            SSEWVMI.WebEngine.CoreWebView2.ExecuteScriptAsync("document.getElementsByTagName('video')[0].loop = true;");

            SSEWVHV.SetStretch(SSEHD.GetStretch());
            SSEWVHV.SetVolume(SSEHD.GetVolume());
        }

        public static void WebEngineInitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            SSEMI.Initialized = e.IsSuccess;

            Uri Video = SSEHS.GetSource(SSEWVMI.Video);

            if (SSEHS.GetExtension(Video))
            {
                SSEWVMI.WebEngine.Source = Video;
            }
            else
            {
                string Path = SSEHS.GetVideoContentPath();

                SSEHS.WriteVideoContent(Path, Video);

                SSEWVMI.WebEngine.Source = SSEHS.GetSource(Path);
            }

            SSEWVMI.WebEngine.CoreWebView2.DOMContentLoaded += WebEngineDOMContentLoaded;

            //SSEWVMI.WebEngine.CoreWebView2.OpenDevToolsWindow();
        }
    }
}
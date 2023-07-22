﻿using System.IO;
using SMR = Sucrose.Memory.Readonly;
using SSSHL = Sucrose.Shared.Space.Helper.Live;
using SSSHP = Sucrose.Shared.Space.Helper.Processor;

namespace Sucrose.Shared.Space.Helper
{
    internal static class Import
    {
        public static void Start(string Destination, string Application)
        {
            try
            {
                SSSHL.Kill();
                SSSHP.Kill(Application);

                foreach (string Setting in Settings(Destination))
                {
                    File.Copy(Setting, Path.Combine(SMR.AppDataPath, SMR.AppName, Path.GetFileName(Setting)), true);
                }

                SSSHP.Run(Application);
            }
            catch
            {
                //
            }
        }

        public static string[] Settings(string Path)
        {
            try
            {
                return Directory.GetFiles(Path, "*.json", SearchOption.TopDirectoryOnly);
            }
            catch
            {
                return Array.Empty<string>();
            }
        }
    }
}
﻿using SMHC = Sucrose.Manager.Helper.Cleaner;

namespace Sucrose.Manager.Helper
{
    internal static class Writer
    {
        public static void Write(string filePath, string fileContent)
        {
            try
            {
                using FileStream fileStream = new(filePath, FileMode.Truncate, FileAccess.Write, FileShare.None);
                using StreamWriter writer = new(fileStream);
                writer.Write(SMHC.Clean(fileContent));
            }
            catch
            {
                //
            }
        }

        public static void WriteBasic(string filePath, string fileContent)
        {
            try
            {
                using StreamWriter writer = File.AppendText(filePath);
                writer.WriteLine(SMHC.Clean(fileContent));
            }
            catch
            {
                //
            }
        }

        public static void WriteStable(string filePath, string fileContent)
        {
            try
            {
                File.WriteAllText(filePath, SMHC.Clean(fileContent));
            }
            catch
            {
                //
            }
        }
    }
}
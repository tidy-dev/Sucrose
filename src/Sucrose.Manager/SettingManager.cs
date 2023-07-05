﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using SMCIPAC = Sucrose.Manager.Converter.IPAddressConverter;
using SMR = Sucrose.Memory.Readonly;

namespace Sucrose.Manager
{
    public class SettingManager
    {
        private readonly string _settingsFilePath;
        private readonly ReaderWriterLockSlim _lock;
        private readonly JsonSerializerSettings _serializerSettings;

        public SettingManager(string settingsFileName, Formatting formatting = Formatting.Indented, TypeNameHandling typeNameHandling = TypeNameHandling.None)
        {
            _settingsFilePath = Path.Combine(SMR.AppDataPath, SMR.AppName, settingsFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(_settingsFilePath));

            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = typeNameHandling,
                Formatting = formatting,
                Converters =
                {
                    //new SMCEC(),
                    new SMCIPAC(),
                    //new StringEnumConverter(),
                }
            };

            _lock = new ReaderWriterLockSlim();
        }

        public T GetSetting<T>(string key, T back = default)
        {
            _lock.EnterReadLock();

            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    string json = ReadFile();

                    Settings settings = JsonConvert.DeserializeObject<Settings>(json, _serializerSettings);

                    if (settings.Properties.TryGetValue(key, out object value))
                    {
                        return ConvertToType<T>(value);
                    }
                }
            }
            finally
            {
                _lock.ExitReadLock();
            }

            return back;
        }

        public T GetSettingStable<T>(string key, T back = default)
        {
            _lock.EnterReadLock();

            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    string json = ReadFile();

                    Settings settings = JsonConvert.DeserializeObject<Settings>(json, _serializerSettings);

                    if (settings.Properties.TryGetValue(key, out object value))
                    {
                        return JsonConvert.DeserializeObject<T>(value.ToString());
                    }
                }
            }
            finally
            {
                _lock.ExitReadLock();
            }

            return back;
        }

        public T GetSettingAddress<T>(string key, T back = default)
        {
            _lock.EnterReadLock();

            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    string json = ReadFile();

                    Settings settings = JsonConvert.DeserializeObject<Settings>(json, _serializerSettings);

                    if (settings.Properties.TryGetValue(key, out object value))
                    {
                        return ConvertToType<T>(value);
                    }
                }
            }
            finally
            {
                _lock.ExitReadLock();
            }

            return back;
        }

        public void SetSetting<T>(string key, T value)
        {
            _lock.EnterWriteLock();

            try
            {
                Settings settings;

                if (File.Exists(_settingsFilePath))
                {
                    string json = ReadFile();
                    settings = JsonConvert.DeserializeObject<Settings>(json, _serializerSettings);
                }
                else
                {
                    settings = new Settings();
                }

                settings.Properties[key] = ConvertToType<T>(value);

                WriteFile(JsonConvert.SerializeObject(settings, _serializerSettings));
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        private string ReadFile()
        {
            try
            {
                //return File.ReadAllText(_settingsFilePath);

                using FileStream fileStream = new(_settingsFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using StreamReader reader = new(fileStream);
                return reader.ReadToEnd();
            }
            catch
            {
                return string.Empty;
            }
        }

        private void WriteFile(string serializedSettings)
        {
            try
            {
                //File.WriteAllText(_settingsFilePath, serializedSettings);

                using FileStream fileStream = new(_settingsFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                using StreamWriter writer = new(fileStream);
                writer.Write(serializedSettings);
            }
            catch
            {
                //
            }
        }

        private T ConvertToType<T>(object value)
        {
            if (typeof(T) == typeof(IPAddress))
            {
                return (T)(object)IPAddress.Parse(value.ToString());
            }
            else if (typeof(T) == typeof(Uri))
            {
                return (T)(object)new Uri(value.ToString());
            }
            else if (typeof(T).IsEnum)
            {
                return (T)Enum.Parse(typeof(T), value.ToString());
            }

            return (T)value;
        }

        private class Settings
        {
            public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
        }
    }
}
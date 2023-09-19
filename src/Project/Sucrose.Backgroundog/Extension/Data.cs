﻿using Newtonsoft.Json.Linq;
using SBMI = Sucrose.Backgroundog.Manage.Internal;

namespace Sucrose.Backgroundog.Extension
{
    internal static class Data
    {
        public static JObject GetCpuInfo()
        {
            return new JObject
            {
                { "Min", SBMI.CpuData.Min },
                { "Now", SBMI.CpuData.Now },
                { "Max", SBMI.CpuData.Max },
                { "Name", SBMI.CpuData.Name },
                { "Core", SBMI.CpuData.Core },
                { "Thread", SBMI.CpuData.Thread },
                { "Fullname", SBMI.CpuData.Fullname }
            };
        }
        public static JObject GetBiosInfo()
        {
            return new JObject
            {
                { "Name", SBMI.BiosData.Name },
                { "Caption", SBMI.BiosData.Caption },
                { "Version", SBMI.BiosData.Version },
                { "Description", SBMI.BiosData.Description },
                { "ReleaseDate", SBMI.BiosData.ReleaseDate },
                { "Manufacturer", SBMI.BiosData.Manufacturer },
                { "SerialNumber", SBMI.BiosData.SerialNumber },
                { "CurrentLanguage", SBMI.BiosData.CurrentLanguage }
            };
        }

        public static JObject GetDateInfo()
        {
            DateTime Date = DateTime.Now;

            return new JObject
            {
                { "Day", Date.Day },
                { "Hour", Date.Hour },
                { "Year", Date.Year },
                { "Month", Date.Month },
                { "Minute", Date.Minute },
                { "Second", Date.Second },
                { "Millisecond", Date.Millisecond }
            };
        }

        public static JObject GetMemoryInfo()
        {
            return new JObject
            {
                { "Name", SBMI.MemoryData.Name },
                { "MemoryUsed", SBMI.MemoryData.MemoryUsed },
                { "MemoryLoad", SBMI.MemoryData.MemoryLoad },
                { "MemoryAvailable", SBMI.MemoryData.MemoryAvailable },
                { "VirtualMemoryUsed", SBMI.MemoryData.VirtualMemoryUsed },
                { "VirtualMemoryLoad", SBMI.MemoryData.VirtualMemoryLoad },
                { "VirtualMemoryAvailable", SBMI.MemoryData.VirtualMemoryAvailable }
            };
        }

        public static JObject GetBatteryInfo()
        {
            return new JObject
            {
                { "Name", SBMI.BatteryData.Name },
                { "Voltage", SBMI.BatteryData.Voltage },
                { "ChargeRate", SBMI.BatteryData.ChargeRate },
                { "ChargeLevel", SBMI.BatteryData.ChargeLevel },
                { "ChargeCurrent", SBMI.BatteryData.ChargeCurrent },
                { "DischargeRate", SBMI.BatteryData.DischargeRate },
                { "DischargeLevel", SBMI.BatteryData.DischargeLevel },
                { "DischargeCurrent", SBMI.BatteryData.DischargeCurrent },
                { "DegradationLevel", SBMI.BatteryData.DegradationLevel },
                { "DesignedCapacity", SBMI.BatteryData.DesignedCapacity },
                { "RemainingCapacity", SBMI.BatteryData.RemainingCapacity },
                { "FullChargedCapacity", SBMI.BatteryData.FullChargedCapacity },
                { "ChargeDischargeRate", SBMI.BatteryData.ChargeDischargeRate },
                { "ChargeDischargeCurrent", SBMI.BatteryData.ChargeDischargeCurrent },
                { "RemainingTimeEstimated", SBMI.BatteryData.RemainingTimeEstimated }
            };
        }

        public static JObject GetNetworkInfo()
        {
            return new JObject
            {
                { "Name", SBMI.NetworkData.Name },
                { "Upload", SBMI.NetworkData.Upload },
                { "Download", SBMI.NetworkData.Download },
                { "FormatUploadData", SBMI.NetworkData.FormatUploadData },
                { "FormatDownloadData", SBMI.NetworkData.FormatDownloadData },
                {
                    "UploadData", new JObject
                    {
                        { "Text", SBMI.NetworkData.UploadData.Text },
                        { "Value", SBMI.NetworkData.UploadData.Value },
                        { "Type", $"{SBMI.NetworkData.UploadData.Type}" },
                        { "Short", $"{SBMI.NetworkData.UploadData.Short}" }
                    }
                },
                {
                    "DownloadData", new JObject
                    {
                        { "Text", SBMI.NetworkData.DownloadData.Text },
                        { "Value", SBMI.NetworkData.DownloadData.Value },
                        { "Type", $"{SBMI.NetworkData.DownloadData.Type}" },
                        { "Short", $"{SBMI.NetworkData.DownloadData.Short}" }
                    }
                },
            };
        }

        public static JObject GetMotherboardInfo()
        {
            return new JObject
            {
                { "Name", SBMI.MotherboardData.Name }
            };
        }
    }
}
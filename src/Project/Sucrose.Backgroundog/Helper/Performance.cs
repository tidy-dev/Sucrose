﻿using Skylark.Enum;
using Skylark.Standard.Extension.Storage;
using SBEL = Sucrose.Backgroundog.Extension.Lifecycle;
using SBMI = Sucrose.Backgroundog.Manage.Internal;
using SBMM = Sucrose.Backgroundog.Manage.Manager;
using SMMM = Sucrose.Manager.Manage.Manager;
using SSDECPT = Sucrose.Shared.Dependency.Enum.CategoryPerformanceType;
using SSDENPT = Sucrose.Shared.Dependency.Enum.NetworkPerformanceType;
using SSDEPT = Sucrose.Shared.Dependency.Enum.PerformanceType;
using SSLHK = Sucrose.Shared.Live.Helper.Kill;
using SSSHL = Sucrose.Shared.Space.Helper.Live;
using SSSHP = Sucrose.Shared.Space.Helper.Processor;

namespace Sucrose.Backgroundog.Helper
{
    internal static class Performance
    {
        public static async Task Start()
        {
            Console.WriteLine("Performance");

            if (await CpuPerformance())
            {
                return;
            }

            if (await SaverPerformance())
            {
                return;
            }

            if (await MemoryPerformance())
            {
                return;
            }

            if (await NetworkPerformance())
            {
                return;
            }

            if (await BatteryPerformance())
            {
                return;
            }

            await Task.CompletedTask;
        }

        private static void Lifecycle()
        {
            if (SBMI.Performance == SSDEPT.Close)
            {
                SSLHK.Stop();
            }
            else
            {
                SBMI.Live = SSSHL.Get();

                if (SBMI.Live != null && !SBMI.Live.HasExited)
                {
                    SBEL.Suspend(SBMI.Live);
                }

                if (!string.IsNullOrEmpty(SMMM.App))
                {
                    SBMI.App = SSSHP.Get(SMMM.App);

                    if (SBMI.App != null && !SBMI.App.HasExited)
                    {
                        SBEL.Suspend(SBMI.App);
                    }
                }
            }
        }

        private static async Task<bool> CpuPerformance()
        {
            if (SBMM.CpuPerformance != SSDEPT.Resume)
            {
                int Count = 0;
                int MaxCount = 5;

                while (SBMI.CpuData.State && SMMM.CpuUsage > 0 && SBMI.CpuData.Now >= SMMM.CpuUsage)
                {
                    if (Count >= MaxCount)
                    {
                        SBMI.Performance = SBMM.CpuPerformance;
                        SBMI.CategoryPerformance = SSDECPT.Cpu;
                        SBMI.Condition = true;
                        Lifecycle();

                        return true;
                    }
                    else
                    {
                        Count++;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }

            return false;
        }

        private static async Task<bool> SaverPerformance()
        {
            if (SBMM.SaverPerformance != SSDEPT.Resume)
            {
                int Count = 0;
                int MaxCount = 5;

                while (SBMI.BatteryData.State && (SBMI.BatteryData.SavingMode || SBMI.BatteryData.SaverStatus == "On"))
                {
                    if (Count >= MaxCount)
                    {
                        SBMI.Performance = SBMM.SaverPerformance;
                        SBMI.CategoryPerformance = SSDECPT.Saver;
                        SBMI.Condition = true;
                        Lifecycle();

                        return true;
                    }
                    else
                    {
                        Count++;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }

            return false;
        }

        private static async Task<bool> MemoryPerformance()
        {
            if (SBMM.MemoryPerformance != SSDEPT.Resume)
            {
                int Count = 0;
                int MaxCount = 5;

                while (SBMI.MemoryData.State && SMMM.MemoryUsage > 0 && SBMI.MemoryData.MemoryLoad >= SMMM.MemoryUsage)
                {
                    if (Count >= MaxCount)
                    {
                        SBMI.Performance = SBMM.MemoryPerformance;
                        SBMI.CategoryPerformance = SSDECPT.Memory;
                        SBMI.Condition = true;
                        Lifecycle();

                        return true;
                    }
                    else
                    {
                        Count++;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }

            return false;
        }

        private static async Task<bool> NetworkPerformance()
        {
            if (SBMM.NetworkPerformance != SSDEPT.Resume)
            {
                int Count = 0;
                int MaxCount = 5;

                while (SBMI.NetworkData.State && (SMMM.PingValue > 0 || SMMM.UploadValue > 0 || SMMM.DownloadValue > 0))
                {
                    if (SBMI.NetworkData.Ping >= SMMM.PingValue)
                    {
                        if (Count >= MaxCount)
                        {
                            SBMI.Performance = SBMM.NetworkPerformance;
                            SBMI.CategoryPerformance = SSDECPT.Network;
                            SBMI.NetworkPerformance = SSDENPT.Ping;
                            SBMI.Condition = true;
                            Lifecycle();

                            return true;
                        }
                        else
                        {
                            Count++;
                        }
                    }
                    else if (SBMI.NetworkData.Upload >= StorageExtension.Convert(SMMM.UploadValue, SMMM.UploadType, StorageType.Byte, ModeStorageType.Palila))
                    {
                        if (Count >= MaxCount)
                        {
                            SBMI.Performance = SBMM.NetworkPerformance;
                            SBMI.CategoryPerformance = SSDECPT.Network;
                            SBMI.NetworkPerformance = SSDENPT.Upload;
                            SBMI.Condition = true;
                            Lifecycle();

                            return true;
                        }
                        else
                        {
                            Count++;
                        }
                    }
                    else if (SBMI.NetworkData.Download >= StorageExtension.Convert(SMMM.DownloadValue, SMMM.DownloadType, StorageType.Byte, ModeStorageType.Palila))
                    {
                        if (Count >= MaxCount)
                        {
                            SBMI.Performance = SBMM.NetworkPerformance;
                            SBMI.CategoryPerformance = SSDECPT.Network;
                            SBMI.NetworkPerformance = SSDENPT.Download;
                            SBMI.Condition = true;
                            Lifecycle();

                            return true;
                        }
                        else
                        {
                            Count++;
                        }
                    }
                    else
                    {
                        break;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }

            return false;
        }

        private static async Task<bool> BatteryPerformance()
        {
            if (SBMM.BatteryPerformance != SSDEPT.Resume)
            {
                int Count = 0;
                int MaxCount = 5;

                while (SBMI.BatteryData.State && (SBMI.BatteryData.PowerLineStatus != PowerLineStatus.Online || SBMI.BatteryData.ACPowerStatus != "Online") && SMMM.BatteryUsage > 0 && SBMI.BatteryData.ChargeLevel <= SMMM.BatteryUsage)
                {
                    if (Count >= MaxCount)
                    {
                        SBMI.Performance = SBMM.BatteryPerformance;
                        SBMI.CategoryPerformance = SSDECPT.Battery;
                        SBMI.Condition = true;
                        Lifecycle();

                        return true;
                    }
                    else
                    {
                        Count++;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }

            return false;
        }
    }
}
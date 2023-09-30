﻿using SBHA = Sucrose.Backgroundog.Helper.Attempt;
using SBHC = Sucrose.Backgroundog.Helper.Condition;
using SBHP = Sucrose.Backgroundog.Helper.Performance;
using SBHS = Sucrose.Backgroundog.Helper.Specification;
using SBMI = Sucrose.Backgroundog.Manage.Internal;
using SSSHL = Sucrose.Shared.Space.Helper.Live;
using SMC = Sucrose.Memory.Constant;
using SMMI = Sucrose.Manager.Manage.Internal;

namespace Sucrose.Backgroundog.Helper
{
    internal class Initialize : IDisposable
    {
        public void Start()
        {
            SBMI.Computer.Open();
            TimerCallback Callback = InitializeTimer_Callback;
            SBMI.InitializeTimer = new(Callback, null, 0, SBMI.InitializeTime);

            SMMI.BackgroundogSettingManager.SetSetting(SMC.ClosePerformance, false);
            SMMI.BackgroundogSettingManager.SetSetting(SMC.PausePerformance, false);
        }

        public void Stop()
        {
            SBMI.Computer.Close();
            SBMI.InitializeTimer.Dispose();
        }

        private async void InitializeTimer_Callback(object State)
        {
            _ = SBHS.Start();

            if (SBMI.Processing)
            {
                SBMI.Processing = false;
                Console.WriteLine("Callback");

                if (SSSHL.Run() && !SBMI.Condition)
                {
                    await SBHP.Start();
                }
                else if (SBMI.Condition)
                {
                    await SBHC.Start();
                }
                else
                {
                    await SBHA.Start();
                }

                SBMI.Processing = true;
            }
        }

        public void Dispose()
        {
            GC.Collect();
            GC.SuppressFinalize(this);
        }
    }
}
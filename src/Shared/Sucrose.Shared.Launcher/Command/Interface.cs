﻿using SDECT = Sucrose.Dependency.Enum.CommandsType;
using SMR = Sucrose.Memory.Readonly;
using SSHP = Sucrose.Space.Helper.Processor;
using SSMI = Sucrose.Space.Manage.Internal;

namespace Sucrose.Shared.Launcher.Command
{
    internal static class Interface
    {
        public static void Command()
        {
            SSHP.Run(SSMI.Commandog, $"{SMR.StartCommand}{SDECT.Interface}{SMR.ValueSeparator}{SSMI.Portal}");
        }
    }
}
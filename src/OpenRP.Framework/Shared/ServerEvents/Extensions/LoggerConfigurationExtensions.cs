using OpenRP.Framework.Shared.ServerEvents.Services;
using Serilog;
using Serilog.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.ServerEvents.Extensions
{
    public static partial class LoggerConfigurationExtensions
    {
        public static LoggerConfiguration WriteToServerEventLog(
            this LoggerConfiguration loggerConfiguration)
        {
            return loggerConfiguration.WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(Matching.FromSource<ServerEventDispatcher>())
                .WriteTo.Async(a => a.File("Logs/ServerEvents/log-.txt", rollingInterval: RollingInterval.Day, retainedFileTimeLimit: TimeSpan.FromDays(7))));
        }
    }
}

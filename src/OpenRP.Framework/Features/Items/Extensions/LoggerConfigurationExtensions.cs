using OpenRP.Framework.Features.Items.Services;
using Serilog;
using Serilog.Configuration;
using Serilog.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Items.Extensions
{
    public static partial class LoggerConfigurationExtensions
    {
        public static LoggerConfiguration WriteToItemLog(
            this LoggerConfiguration loggerConfiguration)
        {
            return loggerConfiguration.WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(Matching.FromSource<ItemManager>())
                .WriteTo.Async(a => a.File("Logs/ItemManager/log-.txt", rollingInterval: RollingInterval.Day, retainedFileTimeLimit: TimeSpan.FromDays(7))));
        }
    }
}

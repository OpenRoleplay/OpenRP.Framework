using OpenRP.Framework.Features.CDN.Services;
using Serilog;
using Serilog.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.CDN.Extensions
{
    public static partial class LoggerConfigurationExtensions
    {
        public static LoggerConfiguration WriteToOpenCdnLog(
            this LoggerConfiguration loggerConfiguration)
        {
            return loggerConfiguration.WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(Matching.FromSource<OpenCdnService>())
                .WriteTo.File("Logs/OpenCDN/log-.txt", rollingInterval: RollingInterval.Day, retainedFileTimeLimit: TimeSpan.FromDays(7)));
        }
    }
}

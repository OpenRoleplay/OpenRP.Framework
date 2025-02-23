﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenRP.Framework.Features.Items.Extensions;
using OpenRP.Framework.Shared.Chat.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Shared.Logging.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServerLogging(this IServiceCollection self)
        {
            return self.AddLogging(logging =>
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.File("Logs/General/log-.txt", rollingInterval: RollingInterval.Day, retainedFileTimeLimit: TimeSpan.FromDays(7))
                    .WriteToItemLog() 
                    .CreateLogger();

                logging.ClearProviders();
                logging.AddSerilog(Log.Logger, dispose: true);
            });
        }
    }
}

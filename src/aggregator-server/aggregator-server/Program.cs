using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace aggregator_server
{
    public class Program
    {
        private static readonly ILog startupLog = LogManager.GetLogger($"Startup.{typeof(Program)}");

        private static RollingFileAppender CreateRollingFileAppender(string logFilePath, ILayout layout)
        {
            RollingFileAppender appender = new RollingFileAppender();
            appender.AppendToFile = true;
            appender.File = logFilePath;
            appender.Layout = layout;
            appender.MaxSizeRollBackups = 5;
            appender.MaximumFileSize = "10MB";
            appender.RollingStyle = RollingFileAppender.RollingMode.Size;
            appender.StaticLogFileName = true;
            appender.ActivateOptions();

            return appender;
        }

        public static void ConfigureLogging()
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
            patternLayout.ActivateOptions();

            RollingFileAppender startupAppender = CreateRollingFileAppender(@"Logs\Startup.log", patternLayout);

            var startupLogger = LogManager.GetLogger("Startup").Logger as Logger;
            startupLogger.AddAppender(startupAppender);

            RollingFileAppender allLog = CreateRollingFileAppender(@"Logs\All.log", patternLayout);
            hierarchy.Root.AddAppender(allLog);

            RollingFileAppender anomalousLog = CreateRollingFileAppender(@"Logs\Anomalous.log", patternLayout);

            ForwardingAppender allToAnomalousforwarder = new ForwardingAppender();
            allToAnomalousforwarder.AddAppender(anomalousLog);
            allToAnomalousforwarder.AddFilter(new log4net.Filter.LevelRangeFilter() { AcceptOnMatch = true, LevelMin=Level.Warn, LevelMax=Level.Fatal });
            hierarchy.Root.AddAppender(allToAnomalousforwarder);

            startupLogger.AddAppender(anomalousLog);

            hierarchy.Root.Level = Level.Info;
            hierarchy.Configured = true;
        }

        public static void Main(string[] args)
        {
            ConfigureLogging();

            startupLog.Info("***** Application Started *****");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

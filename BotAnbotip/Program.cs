using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BotAnbotip
{    public class Program
    {
        private static ILogger<Program> _logger;

        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();
        public static async Task MainAsync()
        {
            var servicesProvider = BuildDi();
            var loggerFactory = servicesProvider.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<Program>();
            PrepareAll(loggerFactory);

            try
            {
                PrivateData.Read();
                await DataManager.ReadAllDataAsync();
                foreach (var pair in DataManager.RatingChannels.Value)
                    pair.Value.ListOfObjects.Test();
                await DataManager.RatingChannels.SaveAsync();

                await BotClientManager.MainBot.PrepareAsync();

                bool mainLaunchResult = false;
                while (!mainLaunchResult) mainLaunchResult = await BotClientManager.MainBot.Launch();

                AppDomain.CurrentDomain.ProcessExit += (obj, args) => Console.WriteLine("Выход");
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                AppDomain.CurrentDomain.ProcessExit += (obj, args) => NLog.LogManager.Shutdown();


                //var runner = servicesProvider.GetRequiredService<Runner>();

                await Task.Delay(-1);
                NLog.LogManager.Shutdown();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Система не запущена");
            }
        }

        private static void PrepareAll(ILoggerFactory loggerFactory)
        {
            BotClientManager.Prepare(loggerFactory);
        }

        private static IServiceProvider BuildDi()
        {
            var services = new ServiceCollection();

            //Runner is the custom class
            //services.AddTransient<Runner>();

            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddLogging((builder) => builder.SetMinimumLevel(LogLevel.Trace));

            var serviceProvider = services.BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            //configure NLog
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            NLog.LogManager.Configuration = new NLog.Config.LoggingConfiguration();
            NLog.LogManager.Configuration.AddTarget(new NLog.Targets.ColoredConsoleTarget("HerokuConsole"));

            return serviceProvider;
        }
    }
}

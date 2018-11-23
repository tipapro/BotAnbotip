using BotAnbotip.Clients;
using BotAnbotip.Data;
using BotAnbotip.Services;
using BotAnbotip.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BotAnbotip
{    public class Program
    {
        private static ILogger<Program> _logger;

        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();
        public static async Task MainAsync()
        {
            PrivateData.Read();
            new ServiceControlManager(Data.CustomEnums.BotType.Auxiliary);
            var loggerFactory = ServiceControlManager.ServiceProvider.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<Program>();

            await PrepareAll(loggerFactory);

            try
            {
                bool mainLaunchResult = false;
                while (!mainLaunchResult) mainLaunchResult = await ClientControlManager.MainBot.Launch(); 
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Application crashed");
            }
            AppDomain.CurrentDomain.ProcessExit += (obj, args) =>
            {
                _logger.LogInformation("Application was powered off.", obj, args);
                NLog.LogManager.Shutdown();
            };
            await Task.Delay(-1);
        }

        private static async Task PrepareAll(ILoggerFactory loggerFactory)
        {
            ClientControlManager.Prepare(loggerFactory);
            await DataControlManager.ReadAllDataAsync();
            await ClientControlManager.MainBot.PrepareAsync();
        }
    }
}

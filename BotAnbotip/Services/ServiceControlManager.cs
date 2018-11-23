using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Clients;
using BotAnbotip.Data;
using BotAnbotip.Data.CustomEnums;
using BotAnbotip.Data.Group;
using BotAnbotip.Services.Interfaces;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace BotAnbotip.Services
{
    class ServiceControlManager
    {
        private BotType botType;

        public static VipRoleGiveawayService VipRoleGiveaway { get; private set; }
        public static InvitationRemover WantPlayAutoRemoving { get; private set; }
        public static LevelCounter LevelCounter { get; private set; }
        public static TopUpdater TopUpdating { get; private set; }
        public static ICloudStorage CloudStorage { get; private set; }
        public static IServiceProvider ServiceProvider { get; private set; }

        public ServiceControlManager(BotType type)
        {
            botType = type;
            ServiceProvider = BuildDi();
            CloudStorage = ServiceProvider.GetRequiredService<ICloudStorage>();
            var loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
            VipRoleGiveaway = new VipRoleGiveawayService(ClientControlManager.MainBot, "Розыгрыш VIP роли", loggerFactory);
            WantPlayAutoRemoving = new InvitationRemover(ClientControlManager.MainBot, "Автоудаление приглашений в игру", loggerFactory);
            LevelCounter = new LevelCounter(ClientControlManager.MainBot, "Счётчик уровня", loggerFactory);
            TopUpdating = new TopUpdater(ClientControlManager.MainBot, "Автообновление топа", loggerFactory);
        }

        public void RunAll()
        {
            if (botType == BotType.Main)
            {
                VipRoleGiveaway.Run();
                WantPlayAutoRemoving.Run();
                LevelCounter.Run();
                TopUpdating.Run();
            }
        }

        public void TurnOffAll()
        {
            if (botType == BotType.Main)
            {
                VipRoleGiveaway.Stop();
                WantPlayAutoRemoving.Stop();
                LevelCounter.Stop();
                TopUpdating.Stop();
            }
        }

        private IServiceProvider BuildDi()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddSingleton<ICloudStorage, DropboxStorage>();

            services.AddLogging((builder) => builder.SetMinimumLevel(LogLevel.Trace));

            var serviceProvider = services.BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            using (var fileStream = new FileStream("nlog.config", FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fileStream))
                writer.Write(Environment.GetEnvironmentVariable("nlog.config"));
            NLog.LogManager.LoadConfiguration("nlog.config");

            return serviceProvider;
        }
    }
}

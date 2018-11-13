using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.CustomEnums;
using BotAnbotip.Bot.Data.Group;
using Discord;

namespace BotAnbotip.Bot.Services
{
    class ServiceManager
    {
        private readonly BotType botType;

        public static DailyMessagesService DailyMessages { get; private set; }
        public static VipRoleGiveawayService VipRoleGiveaway { get; private set; }
        public static WantPlayAutoRemovingService WantPlayAutoRemoving { get; private set; }
        public static LevelCounterService LevelCounter { get; private set; }
        public static TopUpdatingService TopUpdating { get; private set; }

        public ServiceManager(BotType type)
        {
            botType = type;
            DailyMessages = new DailyMessagesService(BotClientManager.MainBot, "Ежедневные сообщения");
            VipRoleGiveaway = new VipRoleGiveawayService(BotClientManager.MainBot, "Розыгрыш VIP роли");
            WantPlayAutoRemoving = new WantPlayAutoRemovingService(BotClientManager.MainBot, "Автоудаление приглашений в игру");
            LevelCounter = new LevelCounterService(BotClientManager.MainBot, "Счётчик уровня");
            TopUpdating = new TopUpdatingService(BotClientManager.MainBot, "Автообновление топа");
        }

        public void RunAll()
        {
            if (botType == BotType.Main)
            {
                DailyMessages.Run();
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
                DailyMessages.Stop();
                VipRoleGiveaway.Stop();
                WantPlayAutoRemoving.Stop();
                LevelCounter.Stop();
                TopUpdating.Stop();
            }
        }
    }
}

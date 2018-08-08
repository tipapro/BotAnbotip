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

        public ServiceManager(BotType type)
        {
            botType = type;
            DailyMessages = new DailyMessagesService(BotClientManager.MainBot,
                "Ошибка ежедневных сообщений", "Ежедневные сообщения запущены", "Ежедневные сообщения остановлены");
            VipRoleGiveaway = new VipRoleGiveawayService(BotClientManager.MainBot,
                "Ошибка розыгрыша VIP роли", "Розыгрыш VIP роли запущен", "Розыгрыш VIP роли остановлен");
            WantPlayAutoRemoving = new WantPlayAutoRemovingService(BotClientManager.MainBot,
                "Ошибка автоудаления приглашений в игру", "Автоудаление приглашений в игру запущено", "Автоудаление приглашений в игру остановлены");
            LevelCounter = new LevelCounterService(BotClientManager.MainBot,
                "Ошибка счётчика уровня", "Счётчик уровня запущен", "Счётчик уровня остановлен");
        }

        public void RunAll()
        {
            if (botType == BotType.Main)
            {
                DailyMessages.Run();
                VipRoleGiveaway.Run();
                WantPlayAutoRemoving.Run();
                LevelCounter.Run();
            }
        }

        public void TurnOffAll()
        {
            if (botType == BotType.Main)
            {
                DailyMessages.Stop();
                VipRoleGiveaway.Stop();
                WantPlayAutoRemoving.Stop();
            }
        }
    }
}

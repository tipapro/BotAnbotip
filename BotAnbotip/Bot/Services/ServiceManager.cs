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
        private BotType botType;

        public static DailyMessagesService DailyMessages { get; private set; }
        public static HackerChannelAutoChangeService HackerChannelAutoChange { get; private set; }
        public static RainbowRoleAutoChangeService RainbowRoleAutoChange { get; private set; }
        public static VipRoleGiveawayService VipRoleGiveaway { get; private set; }
        public static WantPlayAutoRemovingService WantPlayAutoRemoving { get; private set; }
        public static LevelCounterService LevelCounter { get; private set; }

        public ServiceManager(BotType type)
        {
            botType = type;
            DailyMessages = new DailyMessagesService(BotClientManager.MainBot,
                "Ошибка ежедневных сообщений", "Ежедневные сообщения запущены", "Ежедневные сообщения остановлены");
            HackerChannelAutoChange = new HackerChannelAutoChangeService(BotClientManager.AuxiliaryBot,
                "Ошибка автосмены навзания канала", "Автосмена навзания канала запущена", "Автосмена навзания канала остановлена");
            RainbowRoleAutoChange = new RainbowRoleAutoChangeService(BotClientManager.AuxiliaryBot,
                "Ошибка автосмены цвета роли", "Автосмена цвета роли запущена", "Автосмена цвета роли остановлена");
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
            else if (botType == BotType.Auxiliary)
            {
                if (DataManager.HackerChannelIsRunning.Value) HackerChannelAutoChange.Run();
                if (DataManager.RainbowRoleIsRunning.Value) RainbowRoleAutoChange.Run();
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
            else if (botType == BotType.Auxiliary)
            {
                HackerChannelAutoChange.Stop();
                RainbowRoleAutoChange.Stop();
            }
        }
    }
}

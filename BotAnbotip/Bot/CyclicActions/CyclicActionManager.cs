using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.CustomEnums;
using BotAnbotip.Bot.Data.Group;
using Discord;

namespace BotAnbotip.Bot.CyclicActions
{
    class CyclicActionManager
    {
        private BotType botType;

        public static DailyMessagesCyclicAction DailyMessages { get; private set; }
        public static HackerChannelAutoChangeCyclicAction HackerChannelAutoChange { get; private set; }
        public static RainbowRoleAutoChangeCyclicAction RainbowRoleAutoChange { get; private set; }
        public static VipRoleGiveawayCyclicAction VipRoleGiveaway { get; private set; }
        public static WantPlayAutoRemovingCyclicAction WantPlayAutoRemoving { get; private set; }

        public CyclicActionManager(BotType type)
        {
            botType = type;
            DailyMessages = new DailyMessagesCyclicAction(BotClientManager.MainBot,
                "Ошибка ежедневных сообщений", "Ежедневные сообщения запущены", "Ежедневные сообщения остановлены");
            HackerChannelAutoChange = new HackerChannelAutoChangeCyclicAction(BotClientManager.AuxiliaryBot,
                "Ошибка автосмены навзания канала", "Автосмена навзания канала запущена", "Автосмена навзания канала остановлена");
            RainbowRoleAutoChange = new RainbowRoleAutoChangeCyclicAction(BotClientManager.AuxiliaryBot,
                "Ошибка автосмены цвета роли", "Автосмена цвета роли запущена", "Автосмена цвета роли остановлена");
            VipRoleGiveaway = new VipRoleGiveawayCyclicAction(BotClientManager.MainBot,
                "Ошибка розыгрыша VIP роли", "Розыгрыш VIP роли запущен", "Розыгрыш VIP роли остановлен");
            WantPlayAutoRemoving = new WantPlayAutoRemovingCyclicAction(BotClientManager.MainBot,
                "Ошибка автоудаления приглашений в игру", "Автоудаление приглашений в игру запущено", "Автоудаление приглашений в игру остановлены");
        }

        public void RunAll()
        {
            if (botType == BotType.Main)
            {
                DailyMessages.Run();
                VipRoleGiveaway.Run();
                WantPlayAutoRemoving.Run();
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.Group;
using Discord;

namespace BotAnbotip.Bot.CyclicActions
{
    class CyclicActionManager
    {
        private static DailyMessagesCyclicAction _dailyMessages = new DailyMessagesCyclicAction(BotClientManager.MainBot,
                "Ошибка ежедневных сообщений", "Ежедневные сообщения остановлены");
        private static HackerChannelAutoChangeCyclicAction _hackerChannelAutoChange = new HackerChannelAutoChangeCyclicAction(BotClientManager.AuxiliaryBot,
                "Ошибка автосмены навзания канала", "Автосмен навзания канала остановлена");
        private static RainbowRoleAutoChangeCyclicAction _rainbowRoleAutoChange = new RainbowRoleAutoChangeCyclicAction(BotClientManager.AuxiliaryBot,
                "Ошибка автосмены цвета роли", "Автосмен цвета роли остановлена");
        private static VipRoleGiveawayCyclicAction _vipRoleGiveaway = new VipRoleGiveawayCyclicAction(BotClientManager.MainBot,
                "Ошибка розыгрыша VIP роли", "Розыгрыш VIP роли остановлен");
        private static WantPlayAutoRemovingCyclicAction _wantPlayAutoRemoving = new WantPlayAutoRemovingCyclicAction(BotClientManager.MainBot,
                "Ошибка автоудаления приглашений в игру", "Автоудаление приглашений в игру остановлены");

        public static DailyMessagesCyclicAction DailyMessages => _dailyMessages;
        public static HackerChannelAutoChangeCyclicAction HackerChannelAutoChange => _hackerChannelAutoChange;
        public static RainbowRoleAutoChangeCyclicAction RainbowRoleAutoChange => _rainbowRoleAutoChange;
        public static VipRoleGiveawayCyclicAction VipRoleGiveaway => _vipRoleGiveaway;
        public static WantPlayAutoRemovingCyclicAction WantPlayAutoRemoving => _wantPlayAutoRemoving;

        public static void RunAllMain()
        {
            DailyMessages.Run();
            VipRoleGiveaway.Run();
            WantPlayAutoRemoving.Run();
        }

        public static void RunAllAuxiliary()
        {
            HackerChannelAutoChange.Run();
            RainbowRoleAutoChange.Run();
        }

        public static void TurnOffMain()
        {
            DailyMessages.Stop();
            VipRoleGiveaway.Stop();
            WantPlayAutoRemoving.Stop();
        }

        public static void TurnOffAuxiliary()
        {
            HackerChannelAutoChange.Stop();
            RainbowRoleAutoChange.Stop();
        }
    }
}

﻿using BotAnbotip.Bot.Data.CustomEnums;

namespace BotAnbotip.Bot.Clients
{
    class BotClientManager
    {
        public static MainBotClient MainBot { get; } = new MainBotClient(BotType.Main);
        public static AuxiliaryBotClient AuxiliaryBot { get; } = new AuxiliaryBotClient(BotType.Auxiliary);
    }
}

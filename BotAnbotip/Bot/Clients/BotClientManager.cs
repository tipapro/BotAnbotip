using BotAnbotip.Bot.Data.CustomEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotAnbotip.Bot.Clients
{
    class BotClientManager
    {
        private static MainBotClient _mainBot = new MainBotClient(BotType.Main);
        private static AuxiliaryBotClient _auxiliaryBot = new AuxiliaryBotClient(BotType.Auxiliary);       

        public static MainBotClient MainBot => _mainBot;
        public static AuxiliaryBotClient AuxiliaryBot => _auxiliaryBot;
    }
}

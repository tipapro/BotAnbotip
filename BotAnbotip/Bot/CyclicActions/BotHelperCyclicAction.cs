using System;
using System.Collections.Generic;
using System.Text;
using BotAnbotip.Bot.Clients;

namespace BotAnbotip.Bot.CyclicActions
{
    class BotHelperCyclicAction : CyclicActionBase
    {
        public BotHelperCyclicAction(BotClientBase botClient, string errorMessage, string startMessage, string stopMessage) : 
            base(botClient, errorMessage, startMessage, stopMessage)
        {
        }
    }
}

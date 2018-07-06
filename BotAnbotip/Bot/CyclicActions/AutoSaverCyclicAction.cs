using System;
using System.Collections.Generic;
using System.Text;
using BotAnbotip.Bot.Clients;

namespace BotAnbotip.Bot.CyclicActions
{
    class AutoSaverCyclicAction : CyclicActionBase
    {
        public AutoSaverCyclicAction(BotClientBase botClient, string errorMessage, string startMessage, string stopMessage) : 
            base(botClient, errorMessage, startMessage, stopMessage)
        {
        }
    }
}

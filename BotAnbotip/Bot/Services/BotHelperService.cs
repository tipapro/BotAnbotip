﻿using System;
using System.Collections.Generic;
using System.Text;
using BotAnbotip.Bot.Clients;

namespace BotAnbotip.Bot.Services
{
    class BotHelperService : ServiceBase
    {
        public BotHelperService(BotClientBase botClient, string errorMessage, string startMessage, string stopMessage) : 
            base(botClient, errorMessage, startMessage, stopMessage)
        {
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;
using BotAnbotip.Bot.Clients;

namespace BotAnbotip.Bot.Services
{
    class AutoSaverService : ServiceBase
    {
        public AutoSaverService(BotClientBase botClient, string errorMessage, string startMessage, string stopMessage) : 
            base(botClient, errorMessage, startMessage, stopMessage)
        {
        }
    }
}
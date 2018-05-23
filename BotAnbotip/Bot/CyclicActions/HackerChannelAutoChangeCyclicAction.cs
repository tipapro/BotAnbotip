﻿using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.Group;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.CyclicActions
{
    class HackerChannelAutoChangeCyclicAction : CyclicActionBase
    {        
        private const int Length = 10;
        private const int DelayTime = 25;

        private static Random random = new Random();

        public HackerChannelAutoChangeCyclicAction(BotClientBase botClient, string errorMessage, string stopMessage) :
            base(botClient, errorMessage, stopMessage)
        {
            _cycleMethod = Cycle;
        }

        private async Task Cycle(CancellationToken token)
        {
            while (IsStarted)
            {
                await Task.Delay(DelayTime, token);
                await BotClientManager.AuxiliaryBot.Guild.GetChannel(DataManager.HackerChannelId.Value).ModifyAsync((channelProperties) =>
                {
                    channelProperties.Name = GetRandomString(Length);
                });
            }
        }

        public static string GetRandomString(int lenght)
        {
            string resultString = "";
            for (int i = 0; i < lenght; i++)
            {
                resultString += GetRandomChar();
            }
            return resultString;
        }

        public static string GetRandomChar()
        {
            return char.ConvertFromUtf32(random.Next(400));
        }
    }
}

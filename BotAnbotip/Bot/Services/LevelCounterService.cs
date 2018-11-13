﻿using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.CustomClasses;
using BotAnbotip.Bot.Data.CustomEnums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.Services
{
    class LevelCounterService : ServiceBase
    {
        public LevelCounterService(BotClientBase botClient, string serviceName) : base(botClient, serviceName)
        {
        }

        protected override async Task Cycle(CancellationToken token)
        {
            List<ulong> lastUsers = new List<ulong>();
            while (IsStarted)
            {
                await Task.Delay(new TimeSpan(0, 5, 0), token);
                List<ulong> remainingUsers = new List<ulong>();
                var channels = BotClientManager.MainBot.Guild.VoiceChannels;
                foreach (var channel in channels)
                {
                    var channelUsers = channel.Users;
                    foreach(var user in channelUsers)
                    {
                        if (user.IsBot) continue;
                        remainingUsers.Add(user.Id);
                        if (lastUsers.Contains(user.Id))
                        {                          
                            if (!DataManager.UserProfiles.Value.ContainsKey(user.Id)) DataManager.UserProfiles.Value.Add(user.Id, new UserProfile(user.Id));
                            await DataManager.UserProfiles.Value[user.Id].AddPoints((long)ActionsCost.OneMinuteInVoiceChannel * 5);
                        }
                    }
                }                
                lastUsers = remainingUsers;
                await DataManager.UserProfiles.SaveAsync();
            }
        }

    }
}

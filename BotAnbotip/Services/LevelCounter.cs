using BotAnbotip.Clients;
using BotAnbotip.Data;
using BotAnbotip.Data.CustomClasses;
using BotAnbotip.Data.CustomEnums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BotAnbotip.Services
{
    class LevelCounter : ServiceBase
    {
        public LevelCounter(ClientBase botClient, string serviceName) : base(botClient, serviceName)
        {
        }

        protected override async Task Cycle(CancellationToken token)
        {
            List<ulong> lastUsers = new List<ulong>();
            while (IsStarted)
            {
                await Task.Delay(new TimeSpan(0, 5, 0), token);
                List<ulong> remainingUsers = new List<ulong>();
                var channels = ClientControlManager.MainBot.Guild.VoiceChannels;
                foreach (var channel in channels)
                {
                    var channelUsers = channel.Users;
                    foreach(var user in channelUsers)
                    {
                        if (user.IsBot) continue;
                        remainingUsers.Add(user.Id);
                        if (lastUsers.Contains(user.Id))
                        {                          
                            if (!DataControlManager.UserProfiles.Value.ContainsKey(user.Id)) DataControlManager.UserProfiles.Value.Add(user.Id, new UserProfile(user.Id));
                            await DataControlManager.UserProfiles.Value[user.Id].AddPoints((long)ActionsCost.OneMinuteInVoiceChannel * 5);
                        }
                    }
                }                
                lastUsers = remainingUsers;
                await DataControlManager.UserProfiles.SaveAsync();
            }
        }

    }
}

using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.Group;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.Services
{
    class WantPlayAutoRemovingService : ServiceBase
    {
        public WantPlayAutoRemovingService(BotClientBase botClient, string errorMessage, string startMessage, string stopMessage) :
            base(botClient, errorMessage, startMessage, stopMessage)
        {
        }

        protected override async Task Cycle(CancellationToken token)
        {
            while (IsStarted)
            {
                await Task.Delay(new TimeSpan(0, 5, 0), token);
                List<ulong> toDelete = new List<ulong>();
                foreach (var pair in DataManager.AgreeingToPlayUsers.Value)
                {
                    if ((DateTime.Now - pair.Value.Item1.DateTime).Duration() > new TimeSpan(1, 0, 0, 0))
                    {
                        var message = await ((IMessageChannel)BotClientManager.MainBot.Guild.GetChannel((ulong)ChannelIds.chat_gaming)).GetMessageAsync(pair.Key);
                        if (message != null) await message.DeleteAsync();
                        toDelete.Add(pair.Key);
                    }
                }
                foreach (var id in toDelete)
                {
                    if (DataManager.AgreeingToPlayUsers.Value.ContainsKey(id))
                    {
                        DataManager.AgreeingToPlayUsers.Value.Remove(id);
                        await DataManager.AgreeingToPlayUsers.SaveAsync();
                    }
                }
            }
        }
    }
}

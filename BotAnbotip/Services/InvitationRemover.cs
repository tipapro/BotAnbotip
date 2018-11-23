using BotAnbotip.Clients;
using BotAnbotip.Data;
using BotAnbotip.Data.Group;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotAnbotip.Services
{
    class InvitationRemover : ServiceBase
    {
        public InvitationRemover(ClientBase botClient, string serviceName, ILoggerFactory loggerFactory) : base(botClient, serviceName, loggerFactory)
        {
        }

        protected override async Task Cycle(CancellationToken token)
        {
            while (IsStarted)
            {
                await Task.Delay(new TimeSpan(0, 5, 0), token);
                List<ulong> toDelete = new List<ulong>();
                foreach (var pair in DataControlManager.AgreeingToPlayUsers.Value)
                {
                    if ((DateTime.Now - pair.Value.Item1.DateTime).Duration() > new TimeSpan(1, 0, 0, 0))
                    {
                        var message = await ((IMessageChannel)ClientControlManager.MainBot.Guild.GetChannel((ulong)ChannelIds.chat_gaming)).GetMessageAsync(pair.Key);
                        if (message != null) await message.DeleteAsync();
                        toDelete.Add(pair.Key);
                    }
                }
                foreach (var id in toDelete)
                {
                    if (DataControlManager.AgreeingToPlayUsers.Value.ContainsKey(id))
                    {
                        DataControlManager.AgreeingToPlayUsers.Value.Remove(id);
                        await DataControlManager.AgreeingToPlayUsers.SaveAsync();
                    }
                }
            }
        }
    }
}

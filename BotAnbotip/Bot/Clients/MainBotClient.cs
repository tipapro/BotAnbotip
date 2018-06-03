using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using BotAnbotip.Bot.Commands;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.Group;
using BotAnbotip.Bot.CyclicActions;
using BotAnbotip.Bot.Handlers;
using BotAnbotip.Bot.Data.CustomEnums;

namespace BotAnbotip.Bot.Clients
{
    public class MainBotClient:BotClientBase
    {
        private MessageHandler _msgHandler;
        private ReactionHandler _reactionHandler;
        
        public MainBotClient(BotType type) : base(type)
        {
        }

        public async Task PrepareAsync()
        {
            _reactionHandler = new ReactionHandler();
            
            _client.ReactionAdded += ReactionAdded;
            _client.ReactionRemoved += ReactionRemoved;
            _client.Disconnected += (ex) => { CyclicActionManager.TurnOffMain(); return Task.CompletedTask; };
            _client.GuildAvailable += Method;
            _client.GuildAvailable += RunCyclicalMethods;
            _client.UserJoined += UserJoinedTheGroup;

            await _client.SetGameAsync("ANBOTIP Group");          
        }

        private Task Method(SocketGuild arg)
        {
            _msgHandler = new MessageHandler(_client.CurrentUser.Id, PrivateData.MainPrefix);
            _client.MessageReceived += _msgHandler.MessageReceived;
            return Task.CompletedTask;
        }

        private async Task ReactionAdded(Cacheable<IUserMessage, ulong> messageWithReaction, ISocketMessageChannel channel, SocketReaction reaction)
        {
            await Task.Run(() =>_reactionHandler.ReactionAdded(messageWithReaction, channel, reaction));
        }

        private async Task ReactionRemoved(Cacheable<IUserMessage, ulong> messageWithReaction, ISocketMessageChannel channel, SocketReaction reaction)
        {
            await Task.Run(() => _reactionHandler.ReactionRemoved(messageWithReaction, channel, reaction));
        }

        private async Task UserJoinedTheGroup(SocketGuildUser user)
        {
            await user.AddRoleAsync(Guild.GetRole((ulong)RoleIds.Участник));
        }

        private Task RunCyclicalMethods(SocketGuild guild)
        {
            CyclicActionManager.RunAllMain();
            return Task.CompletedTask;
        }

        private Task UserActivity(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            throw new NotImplementedException();
        }
    }
}

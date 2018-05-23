using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.Group;
using BotAnbotip.Bot.Commands;
using BotAnbotip.Bot.Handlers;
using BotAnbotip.Bot.CyclicActions;
using System.Linq;
using BotAnbotip.Bot.Data.CustomEnums;

namespace BotAnbotip.Bot.Clients
{
    public class AuxiliaryBotClient: BotClientBase
    {
        private MessageHandler _msgHandler;

        public AuxiliaryBotClient(BotType type) : base(type)
        {
        }

        public async Task PrepareAsync()
        {            
            _client.GuildAvailable += RunCyclicalMethods;
            _client.Disconnected += (ex) => {CyclicActionManager.TurnOffAuxiliary(); return Task.CompletedTask;};
            _client.GuildAvailable += Method;

            await _client.SetGameAsync("ANBOTIP Group");
        }

        private Task Method(SocketGuild arg)
        {
            _msgHandler = new MessageHandler(_client.CurrentUser.Id, PrivateData.AuxiliaryPrefix);
            _client.MessageReceived += _msgHandler.MessageReceived;
            return Task.CompletedTask;
        }

        private Task MessageReceived(SocketMessage message)
        {
            _msgHandler.MessageReceived(message).GetAwaiter().GetResult();
            return Task.CompletedTask;
        }

        private static Task RunCyclicalMethods(SocketGuild guild)
        {
            if (DataManager.HackerChannelIsRunning.Value) CyclicActionManager.HackerChannelAutoChange.Run();
            if (DataManager.RainbowRoleIsRunning.Value) CyclicActionManager.RainbowRoleAutoChange.Run();
            return Task.CompletedTask;
        }
    }
}

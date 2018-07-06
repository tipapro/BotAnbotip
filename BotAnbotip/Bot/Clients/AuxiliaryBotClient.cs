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
        private CyclicActionManager _cyclicActionManager;

        public AuxiliaryBotClient(BotType type) : base(type)
        {
        }

        public async Task PrepareAsync()
        {
            _cyclicActionManager = new CyclicActionManager(_type);
            _client.Connected += OnConnection;            
            _client.Disconnected += OnDisconnection;
            _client.MessageReceived += OnMessageReceiving;

            await _client.SetGameAsync("ANBOTIP Group");
        }

        private Task OnConnection()
        {
            _cyclicActionManager.RunAll();
            _msgHandler = new MessageHandler(_client.CurrentUser.Id, PrivateData.AuxiliaryPrefix);
            return Task.CompletedTask;
        }

        private Task OnDisconnection(Exception ex)
        {
            _cyclicActionManager.TurnOffAll();
            return Task.CompletedTask;
        }

        private Task OnMessageReceiving(SocketMessage message)
        {
            _msgHandler.ProcessTheMessage(message);
            return Task.CompletedTask;
        }
    }
}

using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Handlers;
using BotAnbotip.Bot.Services;
using BotAnbotip.Bot.Data.CustomEnums;

namespace BotAnbotip.Bot.Clients
{
    public class AuxiliaryBotClient: BotClientBase
    {
        private MessageHandler _msgHandler;
        private ServiceManager _cyclicActionManager;

        public AuxiliaryBotClient(BotType type) : base(type)
        {
        }

        public async Task PrepareAsync()
        {
            _cyclicActionManager = new ServiceManager(_type);
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

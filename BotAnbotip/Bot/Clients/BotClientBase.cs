using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.CustomEnums;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.Clients
{
    public class BotClientBase
    {
        protected bool _isLoaded;
        protected ulong _id;
        protected DiscordSocketClient _client;
        protected SocketGuild _guild;
        protected BotType _type;

        public bool IsLoaded => _isLoaded;
        public ulong Id => _id;
        public DiscordSocketClient Client => _client;
        public SocketGuild Guild => _guild;
        public BotType Type => _type;

        public BotClientBase(BotType type)
        {
            _type = type;
            _client = new DiscordSocketClient();            
            _client.Log += Log;
            _client.GuildAvailable += SetGuild;
            _client.Ready += Loaded;
            _client.Disconnected += Disconnected;
        }

        private Task SetGuild(SocketGuild guild)
        {
            _guild = guild;
            return Task.CompletedTask;
        }

        private Task Loaded()
        {
            if (!IsLoaded)
            {
                _isLoaded = true;
                _id = _client.CurrentUser.Id;
                Log(new LogMessage(LogSeverity.Info, "", "Бот запущен"));
            }
            Log(new LogMessage(LogSeverity.Info, "", "Бот авторизован"));
            return Task.CompletedTask;
        }

        private Task Disconnected(Exception ex)
        {
            _isLoaded = false;
            new ExceptionLogger().Log(ex, Type + "Bot отключён");
            return Task.CompletedTask;
        }

        public async void Launch()
        {
            await _client.LoginAsync(TokenType.Bot, PrivateData.GetBotToken(Type));
            await _client.StartAsync();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(DateTime.Now + "  " + Type + "Bot: " + msg.Message);
            return Task.CompletedTask;
        }
    }
}

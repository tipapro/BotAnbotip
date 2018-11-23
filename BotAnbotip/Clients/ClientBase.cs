using BotAnbotip.Data;
using BotAnbotip.Data.CustomEnums;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BotAnbotip.Clients
{
    public class ClientBase
    {
        protected readonly ILogger<ClientBase> _logger;
        protected bool _isLoaded;
        protected DiscordSocketClient _client;
        protected SocketGuild _guild;
        protected BotType _type;

        public bool IsLoaded => _isLoaded;
        public DiscordSocketClient Client => _client;
        public SocketGuild Guild => _guild;
        public BotType Type => _type;

        public ClientBase(BotType type, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ClientBase>();
            _isLoaded = false;
            _type = type;
            _client = new DiscordSocketClient();            
            _client.Log += ConvertToNLog;
            _client.GuildAvailable += SetGuild;
            _client.Connected += Loaded;
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
                _logger.LogInformation("Бот {_type} запущен", _type);
            }
            _logger.LogInformation("Бот {_type} авторизован", _type);
            return Task.CompletedTask;
        }

        private Task Disconnected(Exception ex)
        {
            _isLoaded = false;
            _logger.LogWarning(ex, "Бот {_type} отключён", _type);
            return Task.CompletedTask;
        }

        public async Task<bool> Launch()
        {
            try
            {
                await _client.LoginAsync(TokenType.Bot, PrivateData.GetBotToken(Type));
                await _client.StartAsync();
            }
            catch (Discord.Net.HttpException ex)
            {
                _logger.LogCritical(ex, "Ошибка при запуске бота {_type}", _type);
                return false;
            }
            return true;
        }

        private Task ConvertToNLog(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Critical: _logger.LogCritical(msg.Exception, msg.Message); break;
                case LogSeverity.Error: _logger.LogError(msg.Exception, msg.Message); break;
                case LogSeverity.Warning: _logger.LogWarning(msg.Exception, msg.Message); break;
                case LogSeverity.Info: _logger.LogInformation(msg.Exception, msg.Message); break;
                case LogSeverity.Verbose: _logger.LogTrace(msg.Exception, msg.Message); break;
                case LogSeverity.Debug: _logger.LogDebug(msg.Exception, msg.Message); break;
            }
            return Task.CompletedTask;
        }
    }
}

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

namespace BotAnbotip.Bot.Clients
{
    public class MainBotClient
    {
        private static bool _botLoaded;
        private static ulong _id;
        private static DiscordSocketClient _client;
                
        public static bool BotLoaded => _botLoaded;
        public static ulong Id => _id;
        public static DiscordSocketClient Client => _client;

        private MessageHandler _msgHandler;
        private ReactionHandler _reactionHandler;
        
        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _reactionHandler = new ReactionHandler();

            PrivateData.Read();
            await DataManager.ReadAllDataAsync();

            _client.Log += Log;
            
            _client.ReactionAdded += _reactionHandler.ReactionAdded;
            _client.ReactionRemoved += _reactionHandler.ReactionRemoved;

            //_client.UserVoiceStateUpdated += UserActivity;
            _client.GuildAvailable += SetInfo;
            _client.GuildAvailable += RunCyclicalMethods;
            _client.UserJoined += UserJoinedTheGroup;

            await _client.SetGameAsync("ANBOTIP Group");
            await _client.SetStatusAsync(UserStatus.Online);

            await _client.LoginAsync(TokenType.Bot, PrivateData.MainBotToken);
            await _client.StartAsync();
            new AuxiliaryBotClient().MainAsync().GetAwaiter().GetResult();
            await Task.Delay(-1);
        }

        private async Task UserJoinedTheGroup(SocketGuildUser user)
        {
            await user.AddRoleAsync(ConstInfo.MainGroupGuild.GetRole((ulong)RoleIds.Участник));
        }

        private Task RunCyclicalMethods(SocketGuild guild)
        {
            CyclicalMethodsManager.RunAll();
            return Task.CompletedTask;
        }

        private async Task Disconnected(Exception ex)
        {
            await CyclicalMethodsManager.TurnOffMain();
            new ExceptionLogger().Log(ex, "Главный бот отключён");
        }

        private Task SetInfo(SocketGuild guild)
        {
            _msgHandler = new MessageHandler(_client.CurrentUser.Id, PrivateData.MainPrefix);
            _client.MessageReceived += _msgHandler.MessageReceived;
            var channel = ((ITextChannel)guild.GetChannel((ulong)ChannelIds.test));
            if (!BotLoaded)
            {
                _botLoaded = true;
                _id = _client.CurrentUser.Id;
                channel.SendMessageAsync("Бот запущен " + DateTime.Now);
            }
            ((ITextChannel)guild.GetChannel((ulong)ChannelIds.test)).SendMessageAsync("Бот авторизован " + DateTime.Now);
            ConstInfo.MainGroupGuild = guild;                       
            return Task.CompletedTask;
        }

        private Task UserActivity(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            throw new NotImplementedException();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine("MainBot: " + msg.ToString());
            return Task.CompletedTask;
        }

        public static async void NotifyTheUser(IUser user, string msg)
        {
            await user.SendMessageAsync(msg);
        }
    }
}

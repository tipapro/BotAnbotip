using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.Group;
using BotAnbotip.Bot.Commands;
using BotAnbotip.Bot.Handlers;
using BotAnbotip.Bot.CyclicActions;

namespace BotAnbotip.Bot.Clients
{
    public class AuxiliaryBotClient
    {
        private static bool _botLoaded;
        private static ulong _id;
        private static DiscordSocketClient _client;

        public static bool BotLoaded => _botLoaded;
        public static ulong Id => _id;
        public static DiscordSocketClient Client => _client;

        private MessageHandler _msgHandler;


        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
           
            _client.Log += Log;
            

            _client.GuildAvailable += SetInfo;
            _client.GuildAvailable += RunCyclicalMethods;
            _client.Disconnected += Disconnected;

            await _client.SetGameAsync("ANBOTIP Group");


            await _client.SetStatusAsync(UserStatus.Online);

            await _client.LoginAsync(TokenType.Bot, PrivateData.AuxiliaryBotToken);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private async Task Disconnected(Exception ex)
        {
            await CyclicalMethodsManager.TurnOffAuxiliary();
            new ExceptionLogger().Log(ex, "Вспомогательный бот отключён");
        }

        private Task RunCyclicalMethods(SocketGuild guild) => RunCyclicalMethods();

        private static async Task RunCyclicalMethods()
        {
            if (DataManager.HackerChannelIsRunning.Value)
            {
                DataManager.HackerChannelIsRunning.Value = false;
                await HackerChannelCommands.ChangeStateOfTheHackerChannelAsync("вкл");
            }
            if (DataManager.RainbowRoleIsRunning.Value)
            {
                DataManager.RainbowRoleIsRunning.Value = false;
                await RainbowRoleCommands.ChangeRainbowRoleState("вкл");
            }
        }

        private async Task SetInfo(SocketGuild guild)
        {
            _msgHandler = new MessageHandler(_client.CurrentUser.Id, '?');
            _client.MessageReceived += _msgHandler.MessageReceived;
            var channel = ((ITextChannel)guild.GetChannel((ulong)ChannelIds.test));
            if (!BotLoaded)
            {
                _botLoaded = true;
                _id = _client.CurrentUser.Id;
                await channel.SendMessageAsync("Бот запущен " + DateTime.Now);
            }
            ConstInfo.AuxiliaryGroupGuild = guild;
            await  ((ITextChannel)guild.GetChannel((ulong)ChannelIds.test)).SendMessageAsync("Бот авторизован " + DateTime.Now);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine("AuxBot: " + msg.ToString());
            return Task.CompletedTask;
        }
    }
}

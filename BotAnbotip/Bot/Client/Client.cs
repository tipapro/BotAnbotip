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

namespace BotAnbotip.Bot.Client
{
    public class Client
    {
        private static DiscordSocketClient _client;
        public static DiscordSocketClient BotClient => _client;  
        private MessageHandler _msgHandler;
        private ReactionHandler _reactionHandler;
        

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _msgHandler = new MessageHandler();
            _reactionHandler = new ReactionHandler();


            DataManager.ReadData();

            _client.Log += Log;
            _client.MessageReceived += _msgHandler.MessageReceived;
            _client.ReactionAdded += _reactionHandler.ReactionAdded;
            _client.ReactionRemoved += _reactionHandler.ReactionRemoved;

            //_client.UserVoiceStateUpdated += UserActivity;
            _client.GuildAvailable += SetInfo;
            _client.GuildAvailable += LaunchAutoChanging;
            _client.UserJoined += UserJoinedTheGroup;

            await _client.SetGameAsync("Pro Group");
            await _client.SetStatusAsync(UserStatus.Online);

            await _client.LoginAsync(TokenType.Bot, PrivateData.BotToken);
            await _client.StartAsync();
            await Task.Delay(-1);

        }

        private async Task UserJoinedTheGroup(SocketGuildUser user)
        {
            await user.AddRoleAsync(Info.GroupGuild.GetRole((ulong)RoleIds.Участник));
        }

        private async Task LaunchAutoChanging(SocketGuild guild)
        {
            if (DataManager.ChannelNameAutoChangingIsSwitchedOn)
            {
                DataManager.ChannelNameAutoChangingIsSwitchedOn = false;
                await ChangeTheChannelCommands.SetTheChannelNameAutoChangingAsync("вкл");
            }
            if (DataManager.RoleColorAutoChangingIsSwitchedOn)
            {
                DataManager.RoleColorAutoChangingIsSwitchedOn = false;
                await ChangeTheRoleCommands.SetTheRoleColorAutoChangingAsync("вкл");
            }
        }

        

        private Task SetInfo(SocketGuild guild)
        {
            Info.GroupGuild = guild;
            Info.BotLoaded = true;            
            return Task.CompletedTask;
        }


        private Task UserActivity(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {

            throw new NotImplementedException();
        }








        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}

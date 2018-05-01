﻿using System;
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

namespace BotAnbotip.Bot.Client
{
    public class BotClient
    {
        public static bool BotLoaded;
        private static DiscordSocketClient _client;
        public static DiscordSocketClient Client => _client;  
        private MessageHandler _msgHandler;
        private ReactionHandler _reactionHandler;
        private static bool IsLoaded;
        

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _msgHandler = new MessageHandler();
            _reactionHandler = new ReactionHandler();

            PrivateData.Read();
            await DataManager.ReadAllDataAsync();

            _client.Log += Log;
            _client.MessageReceived += _msgHandler.MessageReceived;
            _client.ReactionAdded += _reactionHandler.ReactionAdded;
            _client.ReactionRemoved += _reactionHandler.ReactionRemoved;

            //_client.UserVoiceStateUpdated += UserActivity;
            _client.GuildAvailable += SetInfo;
            _client.GuildAvailable += RunCyclicalMethods;
            _client.UserJoined += UserJoinedTheGroup;

            await _client.SetGameAsync("ANBOTIP Group");
            await _client.SetStatusAsync(UserStatus.Online);

            await _client.LoginAsync(TokenType.Bot, PrivateData.BotToken);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private async Task UserJoinedTheGroup(SocketGuildUser user)
        {
            await user.AddRoleAsync(ConstInfo.GroupGuild.GetRole((ulong)RoleIds.Участник));
        }

        private Task RunCyclicalMethods(SocketGuild guild)
        {
            CyclicalMethodsManager.RunAll();
            return Task.CompletedTask;
        }

        private Task SetInfo(SocketGuild guild)
        {
            var channel = ((ITextChannel)guild.GetChannel((ulong)ChannelIds.test));
            if (!IsLoaded)
            {
                IsLoaded = true;
                channel.SendMessageAsync("Бот запущен " + DateTime.Now);
            }
            ((ITextChannel)guild.GetChannel((ulong)ChannelIds.test)).SendMessageAsync("Бот авторизован " + DateTime.Now);
            ConstInfo.GroupGuild = guild;
            BotLoaded = true;            
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

        public static async void NotifyTheUser(IUser user, string msg)
        {
            await user.SendMessageAsync(msg);
        }
    }
}

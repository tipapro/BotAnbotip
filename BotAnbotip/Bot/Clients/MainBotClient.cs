﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.Group;
using BotAnbotip.Bot.Services;
using BotAnbotip.Bot.Handlers;
using BotAnbotip.Bot.Data.CustomEnums;
using BotAnbotip.Bot.Data.CustomClasses;

namespace BotAnbotip.Bot.Clients
{
    public class MainBotClient:BotClientBase
    {
        private MessageHandler _msgHandler;
        private ReactionHandler _reactionHandler;
        private AntiSpam _antiReactionSpam;
        private AntiSpam _antiMessageSpam;
        private ServiceManager _cyclicActionManager;

        public MainBotClient(BotType type) : base(type)
        {
        }

        public async Task PrepareAsync()
        {
            _cyclicActionManager = new ServiceManager(_type);
            _reactionHandler = new ReactionHandler();
            _antiMessageSpam = new AntiSpam(SpamType.Message);
            _antiReactionSpam = new AntiSpam(SpamType.Reaction);

            _client.ReactionAdded += OnReactionAddition;
            _client.ReactionRemoved += OnReactionRemoving;            
            _client.Connected += OnConnection;
            _client.Disconnected += OnDisconnection;
            _client.UserJoined += OnUserJoining;
            _client.MessageReceived += OnMessageReceiving;

            await _client.SetGameAsync("ANBOTIP Group");
        }

        private async Task OnUserJoining(SocketGuildUser user)
        {
            await user.AddRoleAsync(Guild.GetRole((ulong)RoleIds.Участник));
            if (DataManager.UserProfiles.Value.ContainsKey(user.Id))
                Guild.GetRole((ulong)LevelPoints.RolelList[DataManager.UserProfiles.Value[user.Id].Level]);
            else Guild.GetRole((ulong)LevelRoleIds.Медь1);
        }

        private Task OnConnection()
        {
            _cyclicActionManager.RunAll();
            _msgHandler = new MessageHandler(_client.CurrentUser.Id, PrivateData.MainPrefix);
            return Task.CompletedTask;
        }

        private Task OnDisconnection(Exception ex)
        {
            _cyclicActionManager.TurnOffAll();
            return Task.CompletedTask;
        }

        private Task OnMessageReceiving(SocketMessage message)
        {
            //_antiMessageSpam.Check(message.Author.Id, message.Content);
            _msgHandler.ProcessTheMessage(message);
            return Task.CompletedTask;
        }

        private Task OnReactionAddition(Cacheable<IUserMessage, ulong> messageWithReaction, ISocketMessageChannel channel, SocketReaction reaction)
        {
            //_antiMessageSpam.Check(reaction.User.Value.Id);
            _reactionHandler.ProcessTheAddedReaction(messageWithReaction, channel, reaction);
            return Task.CompletedTask;
        }

        private Task OnReactionRemoving(Cacheable<IUserMessage, ulong> messageWithReaction, ISocketMessageChannel channel, SocketReaction reaction)
        {
            //_antiMessageSpam.Check(reaction.User.Value.Id);
            _reactionHandler.ProcessTheRemovedReaction(messageWithReaction, channel, reaction);
            return Task.CompletedTask;
        }

        private Task UserActivity(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            throw new NotImplementedException();
        }
    }
}

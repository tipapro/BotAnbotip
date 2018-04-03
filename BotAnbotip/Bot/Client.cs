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

namespace BotAnbotip.Bot
{
    public class Client
    {
        private DiscordSocketClient _client;
        private const char Prefix = '=';
        

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            
            DataManager.ReadData();

            _client.Log += Log;
            _client.MessageReceived += MessageReceived;
            _client.ReactionAdded += ReactionAdded;
            _client.ReactionRemoved += ReactionRemoved;
            //_client.UserVoiceStateUpdated += UserActivity;
            _client.GuildAvailable += SetInfo;

            await _client.SetGameAsync("Pro Group");
            await _client.SetStatusAsync(UserStatus.Online);
            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("token"));
            await _client.StartAsync();
            await Task.Delay(-1);

        }

        private async Task ReactionRemoved(Cacheable<IUserMessage, ulong> messageWithReaction, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId != _client.CurrentUser.Id)
            {
                var message = await messageWithReaction.DownloadAsync();

                if (message.Embeds.Count != 0)
                {
                    if (message.Embeds.First().Title == "Приглашение в игру")
                    {
                        if ((message.Timestamp.DateTime - DateTime.Now) < new TimeSpan(1, 0, 0, 0))
                        {
                            Console.WriteLine("Реакция удалена");
                        }
                    }
                }
            }
        }

        private Task SetInfo(SocketGuild guild)
        {
            Info.GroupGuild = guild;
            Info.BotLoaded = true;
            Info.RoleColorAutoChangingIsSwitchedOn = false;
            Info.ChannelNameAutoChangingIsSwitchedOn = false;
            return Task.CompletedTask;
        }


        private Task UserActivity(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {

            throw new NotImplementedException();
        }

        private async Task ReactionAdded(Cacheable<IUserMessage, ulong> messageWithReaction, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId != _client.CurrentUser.Id)
            {
                var channelCategory = await ((IGuildChannel)channel).GetCategoryAsync();
                var message = await messageWithReaction.DownloadAsync();

                if (message.Author.Id == _client.CurrentUser.Id)
                {
                    //для рейтингового листа
                    if (channelCategory.Id == (ulong)CategoryIds.Рейтинговые_Листы)
                    {

                        await message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);

                        if ((reaction.Emote.Name == "💙") || (reaction.Emote.Name == "❌"))
                        {
                            await RatingListCommands.ChangeRatingAsync(message, channel, reaction);
                        }
                        else
                        {
                            if (reaction.Emote.Name == "🎮")
                            {
                                await WantPlayMessageCommands.SenAsync(RatingListCommands.ConvertMessageToRatingListObject(message),
                                    null, reaction.User.Value, message.Embeds.First().Thumbnail?.Url, message.Embeds.First().Url);
                            }
                        }
                    }

                    //для остального
                    if (message.Embeds.Count != 0)
                    {
                        if (message.Embeds.First().Title == "Приглашение в игру")
                        {
                            if ((message.Timestamp.DateTime - DateTime.Now) < new TimeSpan(1, 0, 0, 0))
                            {
                                Console.WriteLine("Реакция добавлена");
                            }
                        }
                    }
                }               
            }
        }


        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Content.ToCharArray()[0] == Prefix)
            {
                string command = message.Content.Substring(1).Split(' ')[0];
                string argument = "";
                if (message.Content.Length >= (Prefix + command + " ").ToCharArray().Length)
                {
                    argument = message.Content.Substring((Prefix + command + " ").ToCharArray().Length);
                }
                switch (command)
                {
                    case "анон":
                    case "анонимно": await Task.Run(() => AnonymousMessageCommands.SendAsync(message, argument)); break;
                    case "удалианон": await Task.Run(() => AnonymousMessageCommands.DeleteAsync(message, argument)); break;

                    case "хочуиграть": await Task.Run(() => WantPlayMessageCommands.SenAsync(argument, message)); break;

                    case "радуга": await Task.Run(() => ChangeTheRoleCommands.SetTheRoleColorAutoChangingAsync(message, argument)); break;
                    case "хакерканал": await Task.Run(() => ChangeTheChannelCommands.SetTheChannelNameAutoChangingAsync(message, argument)); break;
                    
                    case "добавьлист": await Task.Run(() => RatingListCommands.AddListAsync(message, argument)); break;
                    case "удалилист": await Task.Run(() => RatingListCommands.RemoveListAsync(message, argument)); break;

                    case "добавьоб": await Task.Run(() =>  RatingListCommands.AddValueAsync(message, argument)); break;
                    case "удалиоб": await Task.Run(() => RatingListCommands.RemoveValueAsync(message, argument)); break;

                    case "стоп": await Task.Run(() => BotControlCommands.Stop(message, _client)); break;

                    default:
                        {
                            await message.DeleteAsync();
                            await message.Author.SendMessageAsync($"Команда {command} не определена.\nВаше запрос: " + message.Content);
                            break;
                        }
                }
            }
        }



        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}

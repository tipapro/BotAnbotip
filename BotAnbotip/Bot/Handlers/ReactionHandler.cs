using BotAnbotip.Bot.Commands;
using BotAnbotip.Bot.Data;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data.Group;
using BotAnbotip.Bot.Data.CustomEnums;
using BotAnbotip.Bot.Clients;

namespace BotAnbotip.Bot.Handlers
{
    class ReactionHandler
    {
        private AntiSpam antiSpam;

        public ReactionHandler()
        {
            antiSpam = new AntiSpam(SpamType.Reaction);
        }

        public async Task ReactionAdded(Cacheable<IUserMessage, ulong> messageWithReaction, ISocketMessageChannel channel, SocketReaction reaction)
        {
            try
            {
                if (reaction.UserId != MainBotClient.Client.CurrentUser.Id)
                {
                    var user = reaction.User.Value;
                    if (antiSpam.Check(user.Id)) return;

                    var channelCategory = await ((IGuildChannel)channel).GetCategoryAsync();
                    var message = await messageWithReaction.DownloadAsync();

                    if (message.Author.Id == MainBotClient.Client.CurrentUser.Id)
                    {
                        //для рейтингового листа
                        if (channelCategory.Id == (ulong)CategoryIds.Рейтинговые_Листы)
                        {
                            await message.RemoveReactionAsync(reaction.Emote, user);

                            if ((reaction.Emote.Name == "💙") || (reaction.Emote.Name == "❌"))
                            {
                                await Task.Run(() => RatingListCommands.ChangeRatingAsync(message, channel, reaction));
                            }
                            else
                            {
                                if (reaction.Emote.Name == "🎮")
                                {
                                    await Task.Run(() => WantPlayMessageCommands.SendAsync(RatingListCommands.ConvertMessageToRatingListObject(message),
                                        null, user, message.Embeds.First().Thumbnail?.Url, message.Embeds.First().Url));
                                }
                            }
                        }

                        //для остального
                        if (message.Embeds.Count != 0)
                        {
                            if (message.Embeds.First().Title == ":video_game:Приглашение в игру:video_game:")
                            {
                                await Task.Run(() => WantPlayMessageCommands.AddUserAcceptedAsync(message, user));
                            }
                            if (message.Embeds.First().Title == ":gift:Еженедельный розыгрыш VIP роли:gift:"
                                && DataManager.ParticipantsOfTheGiveaway.Value.ContainsKey(GiveawayType.VIP))
                            {
                                if (!DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP].Contains(user.Id)) DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP].Add(user.Id);
                                await DataManager.ParticipantsOfTheGiveaway.SaveAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при обработке реакции: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Внутренняя ошибка 1: " + ex.InnerException.Message);
                    if (ex.InnerException.InnerException != null)
                    {
                        Console.WriteLine("Внутренняя ошибка 2: " + ex.InnerException.InnerException.Message);
                        if (ex.InnerException.InnerException.InnerException != null)
                            Console.WriteLine("Внутренняя ошибка 3: " + ex.InnerException.InnerException.InnerException.Message);
                    }
                }
            }
        }

        public async Task ReactionRemoved(Cacheable<IUserMessage, ulong> messageWithReaction, ISocketMessageChannel channel, SocketReaction reaction)
        {
            try
            {
                if (reaction.UserId != MainBotClient.Client.CurrentUser.Id)
                {

                    var user = reaction.User.Value;
                    if (antiSpam.Check(user.Id)) return;

                    var channelCategory = await ((IGuildChannel)channel).GetCategoryAsync();
                    var message = await messageWithReaction.DownloadAsync();

                    if (message.Author.Id == MainBotClient.Client.CurrentUser.Id)
                    {
                        //для рейтингового листа
                        if (channelCategory.Id == (ulong)CategoryIds.Рейтинговые_Листы)
                        {

                        }

                        //для остального
                        if (message.Embeds.Count != 0)
                        {
                            if (message.Embeds.First().Title == ":video_game:Приглашение в игру:video_game:")
                            {
                                await Task.Run(() => WantPlayMessageCommands.RemoveUserAcceptedAsync(message, user));
                            }
                            if (message.Embeds.First().Title == ":gift:Еженедельный розыгрыш VIP роли:gift:"
                                && DataManager.ParticipantsOfTheGiveaway.Value.ContainsKey(GiveawayType.VIP))
                            {
                                DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP].Remove(user.Id);
                                await DataManager.ParticipantsOfTheGiveaway.SaveAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при обработке реакции: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Внутренняя ошибка 1: " + ex.InnerException.Message);
                    if (ex.InnerException.InnerException != null)
                    {
                        Console.WriteLine("Внутренняя ошибка 2: " + ex.InnerException.InnerException.Message);
                        if (ex.InnerException.InnerException.InnerException != null)
                            Console.WriteLine("Внутренняя ошибка 3: " + ex.InnerException.InnerException.InnerException.Message);
                    }
                }
            }
        }
    }
}

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

namespace BotAnbotip.Bot.Client
{
    class ReactionHandler
    {
        public async Task ReactionAdded(Cacheable<IUserMessage, ulong> messageWithReaction, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId != BotClient.Client.CurrentUser.Id)
            {
                var channelCategory = await ((IGuildChannel)channel).GetCategoryAsync();
                var message = await messageWithReaction.DownloadAsync();
                var user = reaction.User.Value;

                if (message.Author.Id == BotClient.Client.CurrentUser.Id)
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

        public async Task ReactionRemoved(Cacheable<IUserMessage, ulong> messageWithReaction, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId != BotClient.Client.CurrentUser.Id)
            {
                var channelCategory = await ((IGuildChannel)channel).GetCategoryAsync();
                var message = await messageWithReaction.DownloadAsync();
                var user = reaction.User.Value;

                if (message.Author.Id == BotClient.Client.CurrentUser.Id)
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
    }
}

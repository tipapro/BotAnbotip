using BotAnbotip.Bot.Commands;
using BotAnbotip.Bot.Data;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.Client
{
    class ReactionHandler
    {
        public async Task ReactionAdded(Cacheable<IUserMessage, ulong> messageWithReaction, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId != Client.BotClient.CurrentUser.Id)
            {
                var channelCategory = await ((IGuildChannel)channel).GetCategoryAsync();
                var message = await messageWithReaction.DownloadAsync();

                if (message.Author.Id == Client.BotClient.CurrentUser.Id)
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
                                await WantPlayMessageCommands.SendAsync(RatingListCommands.ConvertMessageToRatingListObject(message),
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
                                await WantPlayMessageCommands.AddUserAcceptedAsync(message, reaction.User.Value);
                            }
                        }
                    }
                }
            }
        }

        public async Task ReactionRemoved(Cacheable<IUserMessage, ulong> messageWithReaction, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId != Client.BotClient.CurrentUser.Id)
            {
                var channelCategory = await ((IGuildChannel)channel).GetCategoryAsync();
                var message = await messageWithReaction.DownloadAsync();

                if (message.Author.Id == Client.BotClient.CurrentUser.Id)
                {
                    //для рейтингового листа
                    if (channelCategory.Id == (ulong)CategoryIds.Рейтинговые_Листы)
                    {

                    }

                    //для остального
                    if (message.Embeds.Count != 0)
                    {
                        if (message.Embeds.First().Title == "Приглашение в игру")
                        {
                            if ((message.Timestamp.DateTime - DateTime.Now) < new TimeSpan(1, 0, 0, 0))
                            {
                                await WantPlayMessageCommands.RemoveUserAcceptedAsync(message, reaction.User.Value);
                            }
                        }
                    }
                }
            }
        }
    }
}

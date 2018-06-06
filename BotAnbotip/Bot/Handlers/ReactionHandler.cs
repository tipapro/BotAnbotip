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
using BotAnbotip.Bot.Data.CustomClasses;

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
                await ProcessTheAddedReaction(messageWithReaction, channel, reaction);
            }
            catch (Exception ex)
            {
                new ExceptionLogger().Log(ex, "Ошибка при обработке добавленной реакции");
            }
            //return Task.CompletedTask;
        }

        private async Task ProcessTheAddedReaction(Cacheable<IUserMessage, ulong> messageWithReaction, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == BotClientManager.MainBot.Client.CurrentUser.Id) return;

            var user = reaction.User.Value;
            var message = await messageWithReaction.DownloadAsync();
            var messageTitle = message.Embeds.First().Title;

            if ((message.Author.Id != BotClientManager.MainBot.Client.CurrentUser.Id) || (message.Embeds.Count == 0)) return;
            if (!(channel is IDMChannel))
            {
                var channelCategory = await((IGuildChannel)channel).GetCategoryAsync();
                if (channelCategory == null) return;
                //для рейтингового листа
                if (channelCategory.Id == (ulong)CategoryIds.Рейтинговые_Листы)
                {
                    await message.RemoveReactionAsync(reaction.Emote, user);

                    switch (reaction.Emote.Name)
                    {
                        case "💙": await Task.Run(() => RatingListCommands.ChangeRatingAsync(message, user, Evaluation.Like)); break;
                        case "❌": await Task.Run(() => RatingListCommands.ChangeRatingAsync(message, user, Evaluation.Dislike)); break;
                        case "🎮":
                            await Task.Run(() => WantPlayMessageCommands.SendAsync(RatingListCommands.ConvertMessageToRatingListObject(message),
               message, message.Embeds.First().Thumbnail?.Url, message.Embeds.First().Url)); break;
                    }
                }
                else
                {
                    //для остального
                    
                    if ((messageTitle == null) || (messageTitle == "")) return;
                    switch (MessageTitles.GetType(messageTitle))
                    {
                        case TitleType.WantPlay:
                            switch (reaction.Emote.Name)
                            {
                                case "✅": await Task.Run(() => WantPlayMessageCommands.AddUserAcceptedAsync(message, user)); break;
                                    /*case "📩":
                                        await message.RemoveReactionAsync(reaction.Emote, user);
                                        await Task.Run(() => WantPlayMessageCommands.SendOptionsOfSubscriptionAsync(message, user)); break;*/
                            }
                            break;
                        case TitleType.VipGiveaway:
                            if (!DataManager.ParticipantsOfTheGiveaway.Value.ContainsKey(GiveawayType.VIP)
                                || (DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP].Contains(user.Id))) break;
                            DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP].Add(user.Id);
                            await DataManager.ParticipantsOfTheGiveaway.SaveAsync();
                            break;
                    }
                }
            }
            //Для лички
            else
            {
                switch (MessageTitles.GetType(messageTitle))
                {
                    case TitleType.SubscriptionManager:
                        switch (reaction.Emote.Name)
                        {
                            case "1⃣": await Task.Run(() => WantPlayMessageCommands.AddUserSubscriptionAsync(message, user, 1)); break;
                            case "2⃣": await Task.Run(() => WantPlayMessageCommands.AddUserSubscriptionAsync(message, user, 2)); break;
                            case "3⃣": await Task.Run(() => WantPlayMessageCommands.AddUserSubscriptionAsync(message, user, 3)); break;
                            case "4⃣": await Task.Run(() => WantPlayMessageCommands.AddUserSubscriptionAsync(message, user, 4)); break;

                            case "5⃣": await Task.Run(() => WantPlayMessageCommands.RemoveUserSubscriptionAsync(message, user, 5)); break;
                            case "6⃣": await Task.Run(() => WantPlayMessageCommands.RemoveUserSubscriptionAsync(message, user, 6)); break;
                            case "7⃣": await Task.Run(() => WantPlayMessageCommands.RemoveUserSubscriptionAsync(message, user, 7)); break;
                            case "8⃣": await Task.Run(() => WantPlayMessageCommands.RemoveUserSubscriptionAsync(message, user, 8)); break;
                        }
                        break;
                }
            }
        }

        public async Task ReactionRemoved(Cacheable<IUserMessage, ulong> messageWithReaction, ISocketMessageChannel channel, SocketReaction reaction)
        {
            try
            {
                await ProcessTheRemovedReaction(messageWithReaction, channel, reaction);
            }
            catch (Exception ex)
            {
                new ExceptionLogger().Log(ex, "Ошибка при обработке удалённой реакции");
            }
            //return Task.CompletedTask;
        }

        private async Task ProcessTheRemovedReaction(Cacheable<IUserMessage, ulong> messageWithReaction, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == BotClientManager.MainBot.Client.CurrentUser.Id) return;

            var user = reaction.User.Value;
            var message = await messageWithReaction.DownloadAsync();
            var messageTitle = message.Embeds.First().Title;

            if (!(message.Author.Id == BotClientManager.MainBot.Client.CurrentUser.Id) || (message.Embeds.Count == 0)) return;
            if (!(channel is IDMChannel))
            {
                var channelCategory = await((IGuildChannel)channel).GetCategoryAsync();
                if (channelCategory == null) return;
                //для рейтингового листа
                if (channelCategory.Id == (ulong)CategoryIds.Рейтинговые_Листы) return;
                //для остального
                switch (MessageTitles.GetType(messageTitle))
                {
                    case TitleType.WantPlay:
                        switch (reaction.Emote.Name)
                        {
                            case "✅": await Task.Run(() => WantPlayMessageCommands.RemoveUserAcceptedAsync(message, user)); break;
                        }
                        break;
                    case TitleType.VipGiveaway:
                        if (!DataManager.ParticipantsOfTheGiveaway.Value.ContainsKey(GiveawayType.VIP)
                            || (!DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP].Contains(user.Id))) break;
                        DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP].Remove(user.Id);
                        await DataManager.ParticipantsOfTheGiveaway.SaveAsync();
                        break;
                }
            }
            //Для лички
            else
            {

            }
        }
    }
}

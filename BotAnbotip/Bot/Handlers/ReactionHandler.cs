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
        private readonly AntiSpam antiSpam;

        public ReactionHandler()
        {
            antiSpam = new AntiSpam(SpamType.Reaction);
        }

        public async void ProcessTheAddedReaction(Cacheable<IUserMessage, ulong> messageWithReaction, ISocketMessageChannel channel, SocketReaction reaction)
        {
            try
            {
                await Task.Run(async () =>
                {
                    if (reaction.UserId == BotClientManager.MainBot.Client.CurrentUser.Id) return;

                    var user = reaction.User.Value;
                    var message = await messageWithReaction.DownloadAsync();
                    var messageTitle = message.Embeds.First().Title;

                    if ((message.Author.Id != BotClientManager.MainBot.Client.CurrentUser.Id) || (message.Embeds.Count == 0)) return;
                    if (!(channel is IDMChannel))
                    {
                        var channelCategory = await ((IGuildChannel)channel).GetCategoryAsync();
                        if (channelCategory == null) return;
                        //для рейтингового листа
                        if (channelCategory.Id == (ulong)CategoryIds.Рейтинговые_Листы)
                        {
                            await message.RemoveReactionAsync(reaction.Emote, user);
                            string objName = message.Embeds.First().Title;

                            switch (reaction.Emote.Name)
                            {
                                case "💙": await CommandManager.RatingList.ChangeRatingAsync(user.Id, channel, objName, Evaluation.Like); break;
                                case "❌": await CommandManager.RatingList.ChangeRatingAsync(user.Id, channel, objName, Evaluation.Dislike); break;
                                case "🎮":
                                    await Task.Run(() => CommandManager.WantPlayMessage.SendAsync(user, objName, message.Embeds.First().Thumbnail?.Url, message.Embeds.First().Url)); break;
                            }
                        }
                        else
                        {
                            //для остальных категорий

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
                                case TitleType.ManageRole:
                                    switch (reaction.Emote.Name)
                                    {
                                        case "🎵": if (DataManager.UserProfiles.Value[reaction.UserId].Level > 8) await Task.Run(() => CommandManager.RoleManagement.GetAsync(reaction.User.Value, (ulong)RoleIds.DJ)); break;
                                        case "🈹": if (DataManager.UserProfiles.Value[reaction.UserId].Level > 5) await Task.Run(() => CommandManager.RoleManagement.GetAsync(reaction.User.Value, (ulong)RoleIds.Любитель_Аниме)); break;
                                    }
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
                });
            }
            catch (Exception ex)
            {
                new ExceptionLogger().Log(ex, "Ошибка при обработке добавленной реакции");
            }
        }

        public async void AddReactionPoints(IUser reactedUser, Cacheable<IUserMessage, ulong> messageWithReaction)
        {
            await Task.Run(async () =>
            {
                if (reactedUser.IsBot) return;
                if (!DataManager.UserProfiles.Value.ContainsKey(reactedUser.Id))
                    DataManager.UserProfiles.Value.Add(reactedUser.Id, new UserProfile(reactedUser.Id));
                await DataManager.UserProfiles.Value[reactedUser.Id].AddPoints((long)ActionsCost.LeftReaction);

                var receivingReactionUser = (await messageWithReaction.DownloadAsync()).Author;
                if (receivingReactionUser.IsBot) return;
                if (receivingReactionUser.Id != reactedUser.Id)
                    if (!DataManager.UserProfiles.Value.ContainsKey(receivingReactionUser.Id))
                        DataManager.UserProfiles.Value.Add(receivingReactionUser.Id, new UserProfile(receivingReactionUser.Id));
                await DataManager.UserProfiles.Value[receivingReactionUser.Id].AddPoints((long)ActionsCost.ReceivedReaction);
                await DataManager.UserProfiles.SaveAsync();
            });
        }

        public async void ProcessTheRemovedReaction(Cacheable<IUserMessage, ulong> messageWithReaction, ISocketMessageChannel channel, SocketReaction reaction)
        {
            try
            {
                await Task.Run(async () =>
                {
                    if (reaction.UserId == BotClientManager.MainBot.Client.CurrentUser.Id) return;

                    var user = reaction.User.Value;
                    var message = await messageWithReaction.DownloadAsync();
                    var messageTitle = message.Embeds.First().Title;

                    if (!(message.Author.Id == BotClientManager.MainBot.Client.CurrentUser.Id) || (message.Embeds.Count == 0)) return;
                    if (!(channel is IDMChannel))
                    {
                        var channelCategory = await ((IGuildChannel)channel).GetCategoryAsync();
                        if (channelCategory == null) return;
                        // для рейтингового листа
                        if (channelCategory.Id == (ulong)CategoryIds.Рейтинговые_Листы) return;
                        // для остальных категорий
                        switch (MessageTitles.GetType(messageTitle))
                        {
                            case TitleType.WantPlay:
                                if (reaction.Emote.Name == "✅") await Task.Run(() => WantPlayMessageCommands.RemoveUserAcceptedAsync(message, user));
                                break;
                            case TitleType.VipGiveaway:
                                if (!DataManager.ParticipantsOfTheGiveaway.Value.ContainsKey(GiveawayType.VIP)) break;
                                DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP].Remove(user.Id);
                                await DataManager.ParticipantsOfTheGiveaway.SaveAsync();
                                break;
                            case TitleType.ManageRole:
                                switch (reaction.Emote.Name)
                                {
                                    case "🎵": await Task.Run(() => CommandManager.RoleManagement.RemoveAsync(reaction.User.Value, (ulong)RoleIds.DJ)); break;
                                    case "🈹": await Task.Run(() => CommandManager.RoleManagement.RemoveAsync(reaction.User.Value, (ulong)RoleIds.Любитель_Аниме)); break;
                                }
                                break;
                        }
                    }
                    //Для лички
                    else
                    {

                    }
                });
            }
            catch (Exception ex)
            {
                new ExceptionLogger().Log(ex, "Ошибка при обработке удалённой реакции");
            }
        }

        public async void RemoveReactionPoints(IUser reactedUser, Cacheable<IUserMessage, ulong> messageWithReaction)
        {
            await Task.Run(async () =>
            {
                if (reactedUser.IsBot) return;
                if (!DataManager.UserProfiles.Value.ContainsKey(reactedUser.Id))
                    DataManager.UserProfiles.Value.Add(reactedUser.Id, new UserProfile(reactedUser.Id));
                await DataManager.UserProfiles.Value[reactedUser.Id].RemovePoints((long)ActionsCost.LeftReaction);

                var receivingReactionUser = (await messageWithReaction.DownloadAsync()).Author;
                if (receivingReactionUser.IsBot) return;
                if (receivingReactionUser.Id != reactedUser.Id)
                    if (!DataManager.UserProfiles.Value.ContainsKey(receivingReactionUser.Id))
                        DataManager.UserProfiles.Value.Add(receivingReactionUser.Id, new UserProfile(receivingReactionUser.Id));
                await DataManager.UserProfiles.Value[receivingReactionUser.Id].RemovePoints((long)ActionsCost.ReceivedReaction);
                await DataManager.UserProfiles.SaveAsync();
            });
        }
    }
}

using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using System;
using System.Collections.Generic;
using Discord.Rest;
using BotAnbotip.Bot.Data.Group;
using BotAnbotip.Bot.Data.CustomClasses;
using BotAnbotip.Bot.Clients;

namespace BotAnbotip.Bot.Commands
{
    public class RatingListCommands : CommandsBase
    {
        public RatingListCommands() : base
            (
            (TransformMessageToAddListAsync,
            new string[] { "+лист", "добавьлист", "+list", "addlist" }),
            (TransformMessageToRemoveListAsync,
            new string[] { "-лист", "удалилист", "-list", "removelist" }),
            (TransformMessageToAddValueAsync,
            new string[] { "+объект", "добавьобъект", "добавьоб", "+object", "addobject", "addobj" }),
            (TransformMessageToRemoveValueAsync,
            new string[] { "-объект", "удалиобъект", "удалиоб", "-object", "removeobject", "removeobj" }),
            (TransformMessageToReverseAsync,
            new string[] { "переверни", "перевернилист", "reverse", "reverselist" }),
            (TransformMessageToUpdateList,
            new string[] { "обновилист", "updatelist" }),
            (TransformMessageToUpdateReactions,
            new string[] { "обновиреакции", "updatereactions" })
            )
        { }

        public static Dictionary<RatingListType, string> TypeEmodji = new Dictionary<RatingListType, string>()
        {
            {RatingListType.Gaming, "🎮"},
            {RatingListType.Musical, "🎵"}
        };

        private static async Task TransformMessageToAddListAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

            RatingListType listType = RatingListType.Other;
            var argumentList = CommandManager.ClearAndGetCommandArguments(ref argument);
            foreach (var (arg, str) in argumentList)
            {
                switch (arg)
                {
                    case 'т':
                    case 't':
                        switch (str)
                        {
                            case "игровой":
                            case "gaming": listType = RatingListType.Gaming; break;
                            case "музыкальный":
                            case "musical": listType = RatingListType.Musical; break;
                        }
                        break;
                    case 'и':
                    case 'g': listType = RatingListType.Gaming; break;
                    case 'м':
                    case 'm': listType = RatingListType.Musical; break;
                }
            }
            await CommandManager.RatingList.AddListAsync(argument, listType);
        }

        private static async Task TransformMessageToRemoveListAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;
            ulong ratingListId = ulong.Parse(argument);
            await CommandManager.RatingList.RemoveListAsync(ratingListId);
        }

        private static async Task TransformMessageToAddValueAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

            string thumbnailUrl = null, url = null;
            var argumentList = CommandManager.ClearAndGetCommandArguments(ref argument);
            foreach (var (arg, str) in argumentList)
            {
                switch (arg)
                {
                    case 'и':
                    case 'i': thumbnailUrl = str; break;
                    case 'с':
                    case 'l': url = str; break;
                }
            }
            await CommandManager.RatingList.AddValueAsync(message.Channel, argument, thumbnailUrl, url);
        }

        private static async Task TransformMessageToRemoveValueAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;
            await CommandManager.RatingList.RemoveValueAsync(message.Channel, argument);
        }

        private static async Task TransformMessageToReverseAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;
            await CommandManager.RatingList.ReverseAsync(message.Channel);
        }

        private static async Task TransformMessageToUpdateList(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;
            int fromPos = 0, toPos = 0;
            if (argument == "-1")
            {
                fromPos = 0;
                toPos = -1;
            }
            else
            {
                var strArray = argument.Split(' ');
                fromPos = int.Parse(strArray[0]);
                toPos = int.Parse(strArray[1]);
            }
            await CommandManager.RatingList.UpdateList(fromPos, toPos, message.Channel.Id);
        }

        private static async Task TransformMessageToUpdateReactions(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;
            int fromPos = 0, toPos = 0;
            if (argument == "-1")
            {
                fromPos = 0;
                toPos = -1;
            }
            else
            {
                var strArray = argument.Split(' ');
                fromPos = int.Parse(strArray[0]);
                toPos = int.Parse(strArray[1]);
            }
            await CommandManager.RatingList.UpdateReactions(fromPos, toPos, message.Channel.Id);
        }

        public async Task AddListAsync(string listName, RatingListType listType)
        {
            var newRatingChannel = await BotClientManager.MainBot.Guild.CreateTextChannelAsync(listName);
            await newRatingChannel.ModifyAsync((textChannelProperties) =>
            {
                textChannelProperties.CategoryId = (ulong)CategoryIds.Рейтинговые_Листы;
            });

            await newRatingChannel.AddPermissionOverwriteAsync(BotClientManager.MainBot.Guild.GetRole((ulong)RoleIds.Главный_Бот),
                OverwritePermissions.AllowAll(newRatingChannel));

            await newRatingChannel.AddPermissionOverwriteAsync(
                BotClientManager.MainBot.Guild.GetRole((ulong)RoleIds.Музыкальный_Бот),
                OverwritePermissions.DenyAll(newRatingChannel));
            await newRatingChannel.AddPermissionOverwriteAsync(
                BotClientManager.MainBot.Guild.GetRole((ulong)RoleIds.Чат_Бот),
                OverwritePermissions.DenyAll(newRatingChannel));
            await newRatingChannel.AddPermissionOverwriteAsync(
                BotClientManager.MainBot.Guild.GetRole((ulong)RoleIds._Бот),
                OverwritePermissions.DenyAll(newRatingChannel));
            await newRatingChannel.AddPermissionOverwriteAsync(
                BotClientManager.MainBot.Guild.GetRole((ulong)RoleIds.Backend_Bot),
                OverwritePermissions.DenyAll(newRatingChannel));

            await newRatingChannel.AddPermissionOverwriteAsync(
                BotClientManager.MainBot.Guild.EveryoneRole,
                OverwritePermissions.DenyAll(newRatingChannel));

            await newRatingChannel.AddPermissionOverwriteAsync(BotClientManager.MainBot.Guild.GetRole((ulong)RoleIds.Активный_Участник),
                OverwritePermissions.DenyAll(newRatingChannel).Modify(
                    createInstantInvite: PermValue.Allow, readMessages: PermValue.Allow, readMessageHistory: PermValue.Allow));
            await newRatingChannel.AddPermissionOverwriteAsync(BotClientManager.MainBot.Guild.GetRole((ulong)RoleIds.Модератор),
                OverwritePermissions.DenyAll(newRatingChannel).Modify(
                    createInstantInvite: PermValue.Allow, readMessages: PermValue.Allow, readMessageHistory: PermValue.Allow));
            await newRatingChannel.AddPermissionOverwriteAsync(BotClientManager.MainBot.Guild.GetRole((ulong)RoleIds.Администратор),
                OverwritePermissions.DenyAll(newRatingChannel).Modify(
                    createInstantInvite: PermValue.Allow, readMessages: PermValue.Allow, readMessageHistory: PermValue.Allow));
            await newRatingChannel.AddPermissionOverwriteAsync(BotClientManager.MainBot.Guild.GetRole((ulong)RoleIds.Заместитель),
                OverwritePermissions.DenyAll(newRatingChannel).Modify(
                    createInstantInvite: PermValue.Allow, readMessages: PermValue.Allow, readMessageHistory: PermValue.Allow));

            DataManager.RatingChannels.Value.Add(newRatingChannel.Id, new RatingList(newRatingChannel.Id, listType));
            await DataManager.RatingChannels.SaveAsync();
        }

        public async Task RemoveListAsync(ulong ratingListId)
        {
            var ratingChannel = BotClientManager.MainBot.Guild.GetChannel(ratingListId);
            if (ratingChannel != null) await ratingChannel.DeleteAsync();

            DataManager.RatingChannels.Value.Remove(ratingListId);
            await DataManager.RatingChannels.SaveAsync();
        }

        public async Task AddValueAsync(IMessageChannel channel, string objName, string thumbnailUrl = null, string url = null)//!!!!!
        {
            var queueIds = AddToQueue(nameof(DataManager.RatingChannels));
            try
            {
                var embedBuilder = new EmbedBuilder()
                .WithTitle(objName)
                .WithFooter("Количество лайков: 0 ❤️")
                .WithColor(Color.Green);
                if (thumbnailUrl != null) embedBuilder.WithThumbnailUrl(thumbnailUrl);
                if (url != null) embedBuilder.WithUrl(url);



                var sendedMessage = await channel.SendMessageAsync("", false, embedBuilder.Build());

                await sendedMessage.AddReactionAsync(new Emoji("💙"));
                await sendedMessage.AddReactionAsync(new Emoji("❌"));

                var ratingList = DataManager.RatingChannels.Value[channel.Id];
                var obj = new RLObject(objName, ratingList.ListOfObjects.Count, url, thumbnailUrl);
                ratingList.ListOfObjects.Add(obj);
                ratingList.ListOfMessageIds.Add(sendedMessage.Id);

                if (ratingList.Type != RatingListType.Other)
                    await sendedMessage.AddReactionAsync(new Emoji(TypeEmodji[ratingList.Type]));
                var newPosition = ratingList.ListOfObjects.Sort(obj, ratingList.ListOfObjects.Count - 1, Evaluation.None);

                var position = ratingList.ListOfMessageIds.IsReversed ? 0 : ratingList.ListOfMessageIds.Count - 1;
                await UpdateList(position, newPosition, 0, ratingList);

                await DataManager.RatingChannels.SaveAsync();
            }
            finally
            {
                RemoveFromQueue(queueIds);
            }
        }

        public async Task RemoveValueAsync(IMessageChannel channel, string objName)
        {
            var queueIds = AddToQueue(nameof(DataManager.RatingChannels));
            try
            {
                var list = DataManager.RatingChannels.Value[channel.Id];
                var (position, obj) = list.ListOfObjects.FindByName(objName);

                if (obj != null)
                {
                    var messageId = list.ListOfMessageIds[position];
                    var foundedMessage = await channel.GetMessageAsync(messageId);
                    await foundedMessage.DeleteAsync();

                    list.ListOfObjects.Remove(obj);
                    list.ListOfMessageIds.Remove(messageId);
                    await DataManager.RatingChannels.SaveAsync();
                }
            }
            finally
            {
                RemoveFromQueue(queueIds);
            }
        }

        public async Task ChangeRatingAsync(ulong userId, IMessageChannel channel, string objName, Evaluation eval)
        {
            var queueIds = AddToQueue(nameof(DataManager.RatingChannels));
            try
            {
                var ratingList = DataManager.RatingChannels.Value[channel.Id];
                var (position, obj) = ratingList.ListOfObjects.FindByName(objName);
                var likedUsers = obj?.LikedUsers;
                if (likedUsers == null) throw new NullReferenceException();

                if (eval == Evaluation.Like)
                {
                    if (!likedUsers.Contains(userId)) likedUsers.Add(userId);
                }
                else likedUsers.Remove(userId);

                var newPosition = ratingList.ListOfObjects.Sort(obj, position, eval);

                await UpdateList(position, newPosition, 0, ratingList);
                await DataManager.RatingChannels.SaveAsync();
            }
            finally
            {
                RemoveFromQueue(queueIds);
            }
        }

        public async Task ReverseAsync(IMessageChannel channel)
        {
            var queueIds = AddToQueue(nameof(DataManager.RatingChannels));
            try
            {
                var list = DataManager.RatingChannels.Value[channel.Id];

                list.ListOfMessageIds.IsReversed = !list.ListOfMessageIds.IsReversed;

                await UpdateList(0, -1, 0, list);
                await DataManager.RatingChannels.SaveAsync();
            }
            finally
            {
                RemoveFromQueue(queueIds);
            }
        }    

        public async Task UpdateList(int fromPos, int toPos, ulong channelId = 0, RatingList list = null)
        {
            if ((channelId != 0) && (list == null)) list = DataManager.RatingChannels.Value[channelId];
            if (list.ListOfObjects.Count == 0) return;
            if (toPos == -1) toPos = list.ListOfObjects.Count - 1;
            var channel = ((ITextChannel)BotClientManager.MainBot.Guild.GetChannel(list.Id));
            var objects = list.ListOfObjects;

            int sign = fromPos <= toPos ? 1 : -1;

            for (int i = fromPos; i != toPos + sign; i += sign)
            {
                await Task.Delay(200);
                var listObj = list.ListOfObjects[i];
                var messageObj = await channel.GetMessageAsync(list.ListOfMessageIds[i]);

                var embedBuilder = new EmbedBuilder()
                     .WithTitle(listObj.Name)
                     .WithFooter($"Количество лайков: {listObj.LikedUsers.Count} ❤️")
                     .WithColor(Color.Green);
                if (listObj.ThumbnailUrl != "") embedBuilder.WithThumbnailUrl(listObj.ThumbnailUrl);
                if (listObj.Url != "") embedBuilder.WithUrl(listObj.Url);

                await ((IUserMessage)messageObj).ModifyAsync((messageProperties) => messageProperties.Embed = embedBuilder.Build());
            }
        }

        public async Task UpdateReactions(int fromPos, int toPos, ulong channelId = 0, RatingList list = null)
        {
            if ((channelId != 0) && (list == null)) list = DataManager.RatingChannels.Value[channelId];
            if (list.ListOfObjects.Count == 0) return;
            if (toPos == -1) toPos = list.ListOfObjects.Count - 1;
            var channel = ((ITextChannel)BotClientManager.MainBot.Guild.GetChannel(list.Id));
            var objects = list.ListOfObjects;

            int sign = fromPos <= toPos ? 1 : -1;
            var variableCollection = new Dictionary<string, Dictionary<Evaluation, ulong>>();
            for (int i = fromPos; i != toPos + sign; i += sign)
            {                
                var listObj = list.ListOfObjects[i];
                var messageObj = await channel.GetMessageAsync(list.ListOfMessageIds[i]);
                var objName = messageObj.Embeds.First().Title;
                var reactions = ((IUserMessage)messageObj).Reactions;
                var reactionsAndUsers = new Dictionary<Evaluation, ulong>();
                foreach (var reaction in reactions)
                {
                    var users = await ((IUserMessage)messageObj).GetReactionUsersAsync(reaction.Key);
                    Evaluation eval;
                    switch (reaction.Key.Name)
                    {
                        case "💙": eval = Evaluation.Like; break;
                        case "❌": eval = Evaluation.Dislike; break;
                        default: continue;
                    }                   
                    foreach (var user in users)
                    {
                        if (user.Id == BotClientManager.MainBot.Id) continue;
                        reactionsAndUsers.Add(eval, user.Id);
                        await Task.Delay(200);
                        await ((IUserMessage)messageObj).RemoveReactionAsync(reaction.Key, user);
                    }
                }
                variableCollection[objName] = reactionsAndUsers;
            }
            foreach(var pair1 in variableCollection)
                foreach (var pair2 in pair1.Value)
                    await ChangeRatingAsync(pair2.Value, channel, pair1.Key, pair2.Key);
        }
    }
}

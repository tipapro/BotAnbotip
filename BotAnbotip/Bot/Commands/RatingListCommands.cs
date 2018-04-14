using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using System;
using System.Collections.Generic;
using Discord.Rest;

namespace BotAnbotip.Bot.Commands
{
    public class RatingListCommands
    {
        public static Dictionary<RatingListType, string> TypeEmodji = new Dictionary<RatingListType, string>()
        {
            {RatingListType.Game, "🎮"},
            {RatingListType.Music, "🎵"}
        };

        public static async Task AddListAsync(SocketMessage message, string argument)
        {
            await message.DeleteAsync();

            var userRoles = ((IGuildUser) message.Author).RoleIds;

            if (userRoles.Contains((ulong) RoleIds.Основатель) || userRoles.Contains((ulong) RoleIds.Администратор))
            {
                var strBuf = argument.Split(' ');
                var listName = argument.Substring((strBuf[0].Length));
                RatingListType listType = RatingListType.Other;
                if (strBuf.Length != 1)
                {
                    switch (strBuf[0])
                    {
                        case "игры":
                            listType = RatingListType.Game;
                            break;
                        case "музыка":
                            listType = RatingListType.Music;
                            break;
                    }
                }

                var newRatingChannel = await Info.GroupGuild.CreateTextChannelAsync(listName);
                await newRatingChannel.ModifyAsync((textChannelProperties) =>
                {
                    textChannelProperties.CategoryId = (ulong) CategoryIds.Рейтинговые_Листы;
                });

                await newRatingChannel.AddPermissionOverwriteAsync(Info.GroupGuild.GetRole((ulong) RoleIds.Главный_Бот),
                    OverwritePermissions.AllowAll(newRatingChannel));

                await newRatingChannel.AddPermissionOverwriteAsync(
                    Info.GroupGuild.GetRole((ulong) RoleIds.Музыкальный_Бот),
                    OverwritePermissions.DenyAll(newRatingChannel));
                await newRatingChannel.AddPermissionOverwriteAsync(
                    Info.GroupGuild.GetRole((ulong)RoleIds.Чат_Бот),
                    OverwritePermissions.DenyAll(newRatingChannel));
                await newRatingChannel.AddPermissionOverwriteAsync(
                    Info.GroupGuild.GetRole((ulong)RoleIds._Бот),
                    OverwritePermissions.DenyAll(newRatingChannel));

                await newRatingChannel.AddPermissionOverwriteAsync(Info.GroupGuild.EveryoneRole,
                    OverwritePermissions.DenyAll(newRatingChannel));
                await newRatingChannel.AddPermissionOverwriteAsync(Info.GroupGuild.EveryoneRole,
                    newRatingChannel.GetPermissionOverwrite(Info.GroupGuild.EveryoneRole).Value.Modify(PermValue.Allow,
                        null, PermValue.Allow, PermValue.Allow, null, null, null, null, null, PermValue.Allow));

                DataManager.ratingChannels.Add(newRatingChannel.Id,
                    new RatingList(newRatingChannel.Id, newRatingChannel.Name, listType));
            }

            await DataManager.SaveDataAsync();
        }

        public static async Task RemoveListAsync(SocketMessage message, string argument)
        {
            await message.DeleteAsync();

            var userRoles = ((IGuildUser) message.Author).RoleIds;

            if (userRoles.Contains((ulong) RoleIds.Основатель))
            {
                ulong id = ulong.Parse(argument);
                var ratingChannel = Info.GroupGuild.GetChannel(id);
                if (ratingChannel != null) await ratingChannel.DeleteAsync();

                DataManager.RemoveRatingList(id);
            }

            await DataManager.SaveDataAsync();
        }

        public static async Task AddValueAsync(SocketMessage message, string argument)
        {
            await ((IGuildChannel)message.Channel).AddPermissionOverwriteAsync(Info.GroupGuild.EveryoneRole,
                       ((IGuildChannel)message.Channel).GetPermissionOverwrite(Info.GroupGuild.EveryoneRole).Value.Modify(PermValue.Allow,
                           null, PermValue.Deny, PermValue.Allow, null, null, null, null, null, PermValue.Allow));

            await message.DeleteAsync();
            string[] bufStr = argument.Split(' ');
            const int numOfSpaces = 3;
            bool flag = bufStr[0] == "к+с";
            string thumbnailUrl = "", url = "";
            string objName = argument;

            if (flag)
            {
                thumbnailUrl = bufStr[1];
                url = bufStr[2];

                objName = argument.Substring(bufStr[0].Count() + bufStr[1].Count() + bufStr[2].Count() + numOfSpaces);
            }

            var embedBuilder = new EmbedBuilder()
                .WithDescription("**" + objName + "**")
                .WithFooter("Количество лайков: 0 ❤️")
                .WithColor(Color.Green);
            if (flag) embedBuilder = embedBuilder.WithThumbnailUrl(thumbnailUrl).WithUrl(url);

            var sendedMessage = await message.Channel.SendMessageAsync("", false, embedBuilder.Build());

            await sendedMessage.AddReactionAsync(new Emoji("💙"));
            await sendedMessage.AddReactionAsync(new Emoji("❌"));

            var ratingList = DataManager.ratingChannels[message.Channel.Id];
            if (flag) ratingList.ListObjects.Add(objName, sendedMessage.Id, url, thumbnailUrl);
            else ratingList.ListObjects.Add(objName, sendedMessage.Id);

            if (ratingList.Type != RatingListType.Other)
            {
                await sendedMessage.AddReactionAsync(new Emoji(TypeEmodji[ratingList.Type]));
            }

            await SortAsync(message.Channel, DataManager.ratingChannels[message.Channel.Id].ListObjects[objName]); //не эффективно по аргументу
            await DataManager.SaveDataAsync();

            await ((IGuildChannel)message.Channel).AddPermissionOverwriteAsync(Info.GroupGuild.EveryoneRole,
                       ((IGuildChannel)message.Channel).GetPermissionOverwrite(Info.GroupGuild.EveryoneRole).Value.Modify(PermValue.Allow,
                           null, PermValue.Allow, PermValue.Allow, null, null, null, null, null, PermValue.Allow));
        }

        public static async Task RemoveValueAsync(SocketMessage message, string argument)
        {
            await message.DeleteAsync();
            if (DataManager.ratingChannels[message.Channel.Id].ListObjects[argument] != null)
            {
                var foundedMessage =
                    await message.Channel.GetMessageAsync(DataManager.ratingChannels[message.Channel.Id]
                        .ListObjects[argument].MessageId);
                await foundedMessage.DeleteAsync();

                DataManager.ratingChannels[message.Channel.Id].ListObjects.Remove(argument);
                await DataManager.SaveDataAsync();
            }
        }

        public static async Task ChangeRatingAsync(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            await ((IGuildChannel)channel).AddPermissionOverwriteAsync(Info.GroupGuild.EveryoneRole,
                    ((IGuildChannel)channel).GetPermissionOverwrite(Info.GroupGuild.EveryoneRole).Value.Modify(PermValue.Allow,
                        null, PermValue.Deny, PermValue.Allow, null, null, null, null, null, PermValue.Allow));

            var objName = ConvertMessageToRatingListObject(message);

            var likedObject = DataManager.ratingChannels[channel.Id].ListObjects[objName];
            Evaluation eval = reaction.Emote.Name == "💙" ? Evaluation.Like : Evaluation.Dislike;

            //Проверка на наличие пользователя в массиве и сравнение его оценки
            if (!(likedObject.LikedUsers.Contains(reaction.User.Value.Id) &&
                  likedObject.UserEvaluation[reaction.User.Value.Id] == eval))
            {
                likedObject.ChangeEvaluation(reaction.User.Value.Id, eval);
                likedObject.LastEvaluation = eval;

                await message.ModifyAsync((messageProperties) =>
                {
                    var messageEmbed = message.Embeds.First();
                    var embedBuilder = messageEmbed.ToEmbedBuilder()
                        .WithFooter("Количество лайков: " + likedObject.NumberOfLikes + " ❤️");
                    messageProperties.Embed = embedBuilder.Build();
                });
                await SortAsync(channel, likedObject);
                await DataManager.SaveDataAsync();

                await ((IGuildChannel)channel).AddPermissionOverwriteAsync(Info.GroupGuild.EveryoneRole,
                    ((IGuildChannel)channel).GetPermissionOverwrite(Info.GroupGuild.EveryoneRole).Value.Modify(PermValue.Allow,
                        null, PermValue.Allow, PermValue.Allow, null, null, null, null, null, PermValue.Allow));
            }
        }

        private static async Task SortAsync(ISocketMessageChannel channel, ObjectOfRatingList obj)
        {
            await ((IGuildChannel)channel).AddPermissionOverwriteAsync(Info.GroupGuild.EveryoneRole,
                    ((IGuildChannel)channel).GetPermissionOverwrite(Info.GroupGuild.EveryoneRole).Value.Modify(PermValue.Allow,
                        null, PermValue.Deny, PermValue.Allow, null, null, null, null, null, PermValue.Allow));

            var bufMessage1 = await channel.GetMessageAsync(obj.MessageId);

            DataManager.ratingChannels[channel.Id].ListObjects.Sort(obj);


            if (obj.CurrentPosition != obj.PreviousPosition)
            {
                int eval = (int) obj.LastEvaluation;

                for (int i = obj.PreviousPosition; i != obj.CurrentPosition; i -= eval)
                {
                    var firstObject = DataManager.ratingChannels[channel.Id].ListObjects[i];
                    var firstMessage =
                        await channel.GetMessageAsync(firstObject.MessageId);
                    await Task.Delay(300);

                    var secondObject = DataManager.ratingChannels[channel.Id].ListObjects[i - eval];
                    var secondMessage =
                        await channel.GetMessageAsync(secondObject.MessageId);
                    await Task.Delay(300);

                    await SwapTwoMessage(firstMessage, firstObject, secondMessage, secondObject);

                }

                var bufMessage2 = await channel.GetMessageAsync(DataManager.ratingChannels[channel.Id]
                    .ListObjects[obj.CurrentPosition].MessageId);
                await ((IUserMessage) bufMessage2).ModifyAsync((messageProperties) =>
                {
                    var embedBuilder = new EmbedBuilder()
                .WithDescription("**" + obj.Name + "**")
                .WithFooter($"Количество лайков: {obj.NumberOfLikes} ❤️")
                .WithColor(Color.Green);
                    if (obj.ThumbnailUrl != "") embedBuilder.WithThumbnailUrl(obj.ThumbnailUrl);
                    if (obj.Url != "") embedBuilder.WithUrl(obj.Url);

                    messageProperties.Embed = embedBuilder.Build();
                });

            }

            await ((IGuildChannel)channel).AddPermissionOverwriteAsync(Info.GroupGuild.EveryoneRole,
                    ((IGuildChannel)channel).GetPermissionOverwrite(Info.GroupGuild.EveryoneRole).Value.Modify(PermValue.Allow,
                        null, PermValue.Allow, PermValue.Allow, null, null, null, null, null, PermValue.Allow));
        }

        private static async Task SwapTwoMessage(IMessage firstMessage, ObjectOfRatingList firstObject, IMessage secondMessage, ObjectOfRatingList secondObject)
        {
            await ((IUserMessage)firstMessage).ModifyAsync((messageProperties) =>
            {
                var embedBuilder = new EmbedBuilder()
                .WithDescription("**" + firstObject.Name + "**")
                .WithFooter($"Количество лайков: {firstObject.NumberOfLikes} ❤️")
                .WithColor(Color.Green);
                if (firstObject.ThumbnailUrl != "") embedBuilder.WithThumbnailUrl(firstObject.ThumbnailUrl);
                if (firstObject.Url != "") embedBuilder.WithUrl(firstObject.Url);

                messageProperties.Embed = embedBuilder.Build();
            });

            await Task.Delay(300);

            await ((IUserMessage)secondMessage).ModifyAsync((messageProperties) =>
            {
                var embedBuilder = new EmbedBuilder()
                .WithDescription("**" + secondObject.Name + "**")
                .WithFooter($"Количество лайков: {secondObject.NumberOfLikes} ❤️")
                .WithColor(Color.Green);
                if (secondObject.ThumbnailUrl != "") embedBuilder.WithThumbnailUrl(secondObject.ThumbnailUrl);
                if (secondObject.Url != "") embedBuilder.WithUrl(secondObject.Url);

                messageProperties.Embed = embedBuilder.Build();
            });

            await Task.Delay(300);
        }

        public static string ConvertMessageToRatingListObject(IUserMessage message)
        {
            return message.Embeds.First().Description
                .Substring(2, message.Embeds.First().Description.Length - 4);   // "**...**" **** = 4
        }
    }
}

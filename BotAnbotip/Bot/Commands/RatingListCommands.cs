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
    public class RatingListCommands
    {
        public static Task AwaitedTask;
        public static Dictionary<RatingListType, string> TypeEmodji = new Dictionary<RatingListType, string>()
        {
            {RatingListType.Game, "🎮"},
            {RatingListType.Music, "🎵"}
        };

        public static async Task AddListAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

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

        public static async Task RemoveListAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

            ulong ratingListId = ulong.Parse(argument);
            var ratingChannel = BotClientManager.MainBot.Guild.GetChannel(ratingListId);
            if (ratingChannel != null) await ratingChannel.DeleteAsync();

            DataManager.RemoveRatingList(ratingListId);
            await DataManager.RatingChannels.SaveAsync();
        }

        public static async Task AddValueAsync(IMessage message, string argument, bool hasLink, bool hasImage)//!!!!!
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

            string[] bufStr = argument.Split(' ');
            string thumbnailUrl = "", url = "";
            string objName = argument;

            if ((hasLink) && (hasImage))
            {
                url = bufStr[1];
                thumbnailUrl = bufStr[0];
                objName = argument.Substring(url.Count() + thumbnailUrl.Count() + 2);
            }
            else if (hasLink)
            {
                url = bufStr[0];
                objName = argument.Substring(url.Count() + 1);
            }
            else if (hasImage)
            {
                thumbnailUrl = bufStr[0];
                objName = argument.Substring(thumbnailUrl.Count() + 1);
            }

            var embedBuilder = new EmbedBuilder()
            .WithDescription("**" + objName + "**")
            .WithFooter("Количество лайков: 0 ❤️")
            .WithColor(Color.Green);
            embedBuilder = embedBuilder.WithThumbnailUrl(thumbnailUrl).WithUrl(url);

            var sendedMessage = await message.Channel.SendMessageAsync("", false, embedBuilder.Build());

            await sendedMessage.AddReactionAsync(new Emoji("💙"));
            await sendedMessage.AddReactionAsync(new Emoji("❌"));

            var ratingList = DataManager.RatingChannels.Value[message.Channel.Id];
            var obj = new RLObject(objName, ratingList.ListOfObjects.Count, url, thumbnailUrl);
            ratingList.ListOfObjects.Add(obj);
            ratingList.ListOfMessageIds.Add(sendedMessage.Id);

            if (ratingList.Type != RatingListType.Other)
            {
                await sendedMessage.AddReactionAsync(new Emoji(TypeEmodji[ratingList.Type]));
            }

            var position = ratingList.ListOfMessageIds.IsReversed ? 0 : ratingList.ListOfObjects.Count - 1;
            var newPosition = ratingList.ListOfObjects.Sort(obj, position, Evaluation.None);

            await UpdateList(ratingList, position, newPosition);

            await DataManager.RatingChannels.SaveAsync();
        }

        public static async Task RemoveValueAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

            var list = DataManager.RatingChannels.Value[message.Channel.Id];
            var buf = list.ListOfObjects.FindByName(argument);
            var position = buf.Item1;
            var obj = buf.Item2;

            if (obj != null)
            {
                var messageId = list.ListOfMessageIds[buf.Item1];
                var foundedMessage = await message.Channel.GetMessageAsync(messageId);
                await foundedMessage.DeleteAsync();

                list.ListOfObjects.Remove(list.ListOfObjects.FindByName(argument).Item2);
                list.ListOfMessageIds.Remove(messageId);
                await DataManager.RatingChannels.SaveAsync();
            }
        }

        public static async Task ChangeRatingAsync(IUserMessage message, IUser user, Evaluation eval)
        {
            if (!CommandManager.CheckPermission((IGuildUser)user, RoleIds.Активный_Участник)) return;

            var objName = ConvertMessageToRatingListObject(message);

            var ratingList = DataManager.RatingChannels.Value[message.Channel.Id];
            var buf = ratingList.ListOfObjects.FindByName(objName);
            var currentPosition = buf.Item1;
            var obj = buf.Item2;
            var likedUsers = buf.Item2?.LikedUsers;
            if (likedUsers == null) throw new NullReferenceException();

            var previousCount = likedUsers.Count;

            if (eval == Evaluation.Like)
            {
                if (!likedUsers.Contains(user.Id)) likedUsers.Add(user.Id);
            }
            else likedUsers.Remove(user.Id);

            var newPosition = ratingList.ListOfObjects.Sort(obj, currentPosition, eval);

            await UpdateList(ratingList, currentPosition, newPosition);
            await DataManager.RatingChannels.SaveAsync();

        }

        public static async Task ReverseAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

            var channel = ((ITextChannel)BotClientManager.MainBot.Guild.GetChannel(ulong.Parse(argument)));
            var list = DataManager.RatingChannels.Value[channel.Id];

            list.ListOfMessageIds.IsReversed = !list.ListOfMessageIds.IsReversed;

            await UpdateList(list, 0, list.ListOfObjects.Count);
            await DataManager.RatingChannels.SaveAsync();
        }

        public static async Task UpdateList(RatingList list, int previousPosition, int currentPosition)
        {
            var channel = ((ITextChannel)BotClientManager.MainBot.Guild.GetChannel(list.Id));
            var objects = list.ListOfObjects;

            int sign = previousPosition <= currentPosition ? 1 : -1;

            for (int i = previousPosition; i != currentPosition + sign; i += sign)
            {
                await Task.Delay(100);
                var listObj = list.ListOfObjects[i];
                var messageObj = await channel.GetMessageAsync(list.ListOfMessageIds[i]);

                var embedBuilder = new EmbedBuilder()
                     .WithDescription("**" + listObj.Name + "**")
                     .WithFooter($"Количество лайков: {listObj.LikedUsers.Count} ❤️")
                     .WithColor(Color.Green);
                if (listObj.ThumbnailUrl != "") embedBuilder.WithThumbnailUrl(listObj.ThumbnailUrl);
                if (listObj.Url != "") embedBuilder.WithUrl(listObj.Url);

                await ((IUserMessage)messageObj).ModifyAsync((messageProperties) => messageProperties.Embed = embedBuilder.Build());
            }
        }

        public static string ConvertMessageToRatingListObject(IUserMessage message)
        {
            return message.Embeds.First().Description
                .Substring(2, message.Embeds.First().Description.Length - 4);   // "**...**" - 4 лишних знака(форматирования)
        }
    }
}

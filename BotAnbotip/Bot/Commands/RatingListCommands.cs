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
        public static Dictionary<RatingListType, string> TypeEmodji = new Dictionary<RatingListType, string>()
        {
            {RatingListType.Game, "🎮"},
            {RatingListType.Music, "🎵"}
        };

        public static async Task AddListAsync(SocketMessage message, string argument)
        {
            try
            {
                await message.DeleteAsync();
                if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

                var userRoles = ((IGuildUser)message.Author).RoleIds;

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
                    OverwritePermissions.DenyAll(newRatingChannel).Modify(PermValue.Allow,
                        null, null, PermValue.Allow, null, null, null, null, null, PermValue.Allow));
                await newRatingChannel.AddPermissionOverwriteAsync(BotClientManager.MainBot.Guild.GetRole((ulong)RoleIds.Модератор),
                    OverwritePermissions.DenyAll(newRatingChannel).Modify(PermValue.Allow,
                        null, null, PermValue.Allow, null, null, null, null, null, PermValue.Allow));
                await newRatingChannel.AddPermissionOverwriteAsync(BotClientManager.MainBot.Guild.GetRole((ulong)RoleIds.Администратор),
                    OverwritePermissions.DenyAll(newRatingChannel).Modify(PermValue.Allow,
                        null, null, PermValue.Allow, null, null, null, null, null, PermValue.Allow));
                await newRatingChannel.AddPermissionOverwriteAsync(BotClientManager.MainBot.Guild.GetRole((ulong)RoleIds.Заместитель),
                    OverwritePermissions.DenyAll(newRatingChannel).Modify(PermValue.Allow,
                        null, null, PermValue.Allow, null, null, null, null, null, PermValue.Allow));

                DataManager.RatingChannels.Value.Add(newRatingChannel.Id,
                    new RatingList(newRatingChannel.Id, newRatingChannel.Name, listType));

                await DataManager.RatingChannels.SaveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task RemoveListAsync(SocketMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

            var userRoles = ((IGuildUser) message.Author).RoleIds;

            if (userRoles.Contains((ulong) RoleIds.Основатель))
            {
                ulong id = ulong.Parse(argument);
                var ratingChannel = BotClientManager.MainBot.Guild.GetChannel(id);
                if (ratingChannel != null) await ratingChannel.DeleteAsync();

                DataManager.RemoveRatingList(id);
            }

            await DataManager.RatingChannels.SaveAsync();
        }

        public static async Task AddValueAsync(SocketMessage message, string argument, bool hasLink, bool hasImage)
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
            ratingList.ListObjects.Add(objName, sendedMessage.Id, url, thumbnailUrl);

            if (ratingList.Type != RatingListType.Other)
            {
                await sendedMessage.AddReactionAsync(new Emoji(TypeEmodji[ratingList.Type]));
            }

            await SortAsync(message.Channel, DataManager.RatingChannels.Value[message.Channel.Id].ListObjects[objName]); //не эффективно по аргументу
            await DataManager.RatingChannels.SaveAsync();
        }

        public static async Task RemoveValueAsync(SocketMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

            if (DataManager.RatingChannels.Value[message.Channel.Id].ListObjects[argument] != null)
            {
                var foundedMessage =
                    await message.Channel.GetMessageAsync(DataManager.RatingChannels.Value[message.Channel.Id]
                        .ListObjects[argument].MessageId);
                await foundedMessage.DeleteAsync();

                DataManager.RatingChannels.Value[message.Channel.Id].ListObjects.Remove(argument);
                await DataManager.RatingChannels.SaveAsync();
            }
        }

        public static async Task ChangeRatingAsync(IUserMessage message, IUser user, Evaluation eval)
        {
            if (!CommandManager.CheckPermission((IGuildUser)user, RoleIds.Активный_Участник)) return;

            var objName = ConvertMessageToRatingListObject(message);

            var likedObject = DataManager.RatingChannels.Value[message.Channel.Id].ListObjects[objName];

            //Проверка на наличие пользователя в массиве и сравнение его оценки
            if (!(likedObject.LikedUsers.Contains(user.Id) &&
                  likedObject.UserEvaluation[user.Id] == eval))
            {
                likedObject.ChangeEvaluation(user.Id, eval);
                likedObject.LastEvaluation = eval;

                await message.ModifyAsync((messageProperties) =>
                {
                    var messageEmbed = message.Embeds.First();
                    var embedBuilder = messageEmbed.ToEmbedBuilder()
                        .WithFooter("Количество лайков: " + likedObject.NumberOfLikes + " ❤️");
                    messageProperties.Embed = embedBuilder.Build();
                });
                await SortAsync(message.Channel, likedObject);
                await DataManager.RatingChannels.SaveAsync();
            }
        }

        public static async Task ReverseAsync(SocketMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;
            await DataManager.ReverseSign.SaveAsync(!DataManager.ReverseSign);

            var channel = ((ITextChannel)BotClientManager.MainBot.Guild.GetChannel(ulong.Parse(argument)));

            var list = DataManager.RatingChannels.Value[channel.Id].ListObjects;

            list.ReverseMessageIds();

            for (int i = 0; i < list.Count; i++)
            {
                var listObj = DataManager.RatingChannels.Value[channel.Id].ListObjects[i];
                var messageObj = await channel.GetMessageAsync(listObj.MessageId);

                await ((IUserMessage)messageObj).ModifyAsync((messageProperties) =>
                {
                    var embedBuilder = new EmbedBuilder()
                    .WithDescription("**" + listObj.Name + "**")
                    .WithFooter($"Количество лайков: {listObj.NumberOfLikes} ❤️")
                    .WithColor(Color.Green);
                    if (listObj.ThumbnailUrl != "") embedBuilder.WithThumbnailUrl(listObj.ThumbnailUrl);
                    if (listObj.Url != "") embedBuilder.WithUrl(listObj.Url);

                    messageProperties.Embed = embedBuilder.Build();
                });

                await Task.Delay(300);
            }
        }

        private static async Task SortAsync(IMessageChannel channel, RatingListObject obj)
        {
            var previousPosition = obj.Position;
            DataManager.RatingChannels.Value[channel.Id].ListObjects.Sort(obj);

            if (obj.Position != previousPosition)
            {
                int eval = (int) obj.LastEvaluation;

                for (int i = previousPosition; i != obj.Position - eval; i -= eval)
                {
                    var listObj = DataManager.RatingChannels.Value[channel.Id].ListObjects[i];
                    var message = await channel.GetMessageAsync(listObj.MessageId);

                    await ((IUserMessage)message).ModifyAsync((messageProperties) =>
                    {
                        var embedBuilder = new EmbedBuilder()
                        .WithDescription("**" + listObj.Name + "**")
                        .WithFooter($"Количество лайков: {listObj.NumberOfLikes} ❤️")
                        .WithColor(Color.Green);
                        if (listObj.ThumbnailUrl != "") embedBuilder.WithThumbnailUrl(listObj.ThumbnailUrl);
                        if (listObj.Url != "") embedBuilder.WithUrl(listObj.Url);

                        messageProperties.Embed = embedBuilder.Build();
                    });

                    await Task.Delay(300);
                }
            }
        }

        public static string ConvertMessageToRatingListObject(IUserMessage message)
        {
            return message.Embeds.First().Description
                .Substring(2, message.Embeds.First().Description.Length - 4);   // "**...**" - 4 лишних знака(форматирования)
        }
    }
}

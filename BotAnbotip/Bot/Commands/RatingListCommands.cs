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

        public static async Task AddListAsync(IMessage message, string argument)
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
                    new RatingList(newRatingChannel.Id, listType));               
                await DataManager.RatingChannels.SaveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task RemoveListAsync(IMessage message, string argument)
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

            await SortAsync(message.Channel, obj, ratingList.ListOfObjects.Count-1, Evaluation.None);
            await DataManager.RatingChannels.SaveAsync();
        }

        public static async Task RemoveValueAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

            var list = DataManager.RatingChannels.Value[message.Channel.Id];

            var buf = list.ListOfObjects.FindByName(argument);

            if (buf.Item2 != null)
            {
                var messageId = list.ListOfMessageIds[buf.Item1];
                var foundedMessage = await message.Channel.GetMessageAsync(messageId);
                await foundedMessage.DeleteAsync();

                list.ListOfObjects.Remove(argument);
                list.ListOfMessageIds.Remove(messageId);
                await DataManager.RatingChannels.SaveAsync();
            }
        }

        public static async Task ChangeRatingAsync(IUserMessage message, IUser user, Evaluation eval)
        {
            if (!CommandManager.CheckPermission((IGuildUser)user, RoleIds.Активный_Участник)) return;

            var objName = ConvertMessageToRatingListObject(message);

            var buf = DataManager.RatingChannels.Value[message.Channel.Id].ListOfObjects.FindByName(objName);
            var position = buf.Item1;
            var likedUsers = buf.Item2?.LikedUsers;
            if (likedUsers == null) throw new NullReferenceException();

            var previousCount = likedUsers.Count;

            if (eval == Evaluation.Like)
            {
                if (!likedUsers.Contains(user.Id)) likedUsers.Add(user.Id);
            }
            else likedUsers.Remove(user.Id);

            if (previousCount != likedUsers.Count)
            {
                await message.ModifyAsync((messageProperties) =>
                {
                    var messageEmbed = message.Embeds.First();
                    var embedBuilder = messageEmbed.ToEmbedBuilder()
                        .WithFooter("Количество лайков: " + likedUsers.Count + " ❤️");
                    messageProperties.Embed = embedBuilder.Build();
                });
                await SortAsync(message.Channel, buf.Item2, position, eval);
                await DataManager.RatingChannels.SaveAsync();
            }
        }

        public static async Task ReverseAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

            var channel = ((ITextChannel)BotClientManager.MainBot.Guild.GetChannel(ulong.Parse(argument)));
            var list = DataManager.RatingChannels.Value[channel.Id];

            list.ListOfMessageIds.IsReversed = !list.ListOfMessageIds.IsReversed;

            await UpdateList(list, 0, list.ListOfObjects.Count, 1);
            await DataManager.RatingChannels.SaveAsync();
        }

        private static async Task SortAsync(IMessageChannel channel, RLObject obj, int position, Evaluation eval)
        {
            var list = DataManager.RatingChannels.Value[channel.Id];
            var newPosition = list.ListOfObjects.Sort(obj, position, eval);

            if (newPosition != position)
                await UpdateList(list, position, newPosition - (int)eval, (int)eval * (-1));
            await DataManager.RatingChannels.SaveAsync();
        }

        public static async Task UpdateList(RatingList list, int from, int to, int sign)
        {
            var channel = ((ITextChannel)BotClientManager.MainBot.Guild.GetChannel(list.Id));
            var objects = list.ListOfObjects;

            for (int i = from; i != to; i += sign)
            {
                var listObj = list.ListOfObjects[i];
                var messageObj = await channel.GetMessageAsync(list.ListOfMessageIds[i]);

                await ((IUserMessage)messageObj).ModifyAsync((messageProperties) =>
                 {
                     var embedBuilder = new EmbedBuilder()
                     .WithDescription("**" + listObj.Name + "**")
                     .WithFooter($"Количество лайков: {listObj.LikedUsers.Count} ❤️")
                     .WithColor(Color.Green);
                     if (listObj.ThumbnailUrl != "") embedBuilder.WithThumbnailUrl(listObj.ThumbnailUrl);
                     if (listObj.Url != "") embedBuilder.WithUrl(listObj.Url);

                     messageProperties.Embed = embedBuilder.Build();
                 });
                await ((IUserMessage)messageObj).AddReactionAsync(new Emoji("💙"));
                await ((IUserMessage)messageObj).AddReactionAsync(new Emoji("❌"));

                if (list.Type != RatingListType.Other)
                {
                    await ((IUserMessage)messageObj).AddReactionAsync(new Emoji(TypeEmodji[list.Type]));
                }
                await Task.Delay(300);
            }
        }

        public static string ConvertMessageToRatingListObject(IUserMessage message)
        {
            return message.Embeds.First().Description
                .Substring(2, message.Embeds.First().Description.Length - 4);   // "**...**" - 4 лишних знака(форматирования)
        }
    }
}

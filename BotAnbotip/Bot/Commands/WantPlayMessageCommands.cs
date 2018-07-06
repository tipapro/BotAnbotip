using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using Discord;
using Discord.WebSocket;
using BotAnbotip.Bot.Data.Group;
using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data.CustomClasses;
using BotAnbotip.Bot.Data.CustomEnums;

namespace BotAnbotip.Bot.Commands
{
    class WantPlayMessageCommands : CommandsBase
    {
        public WantPlayMessageCommands() : base
            (
            (TransformMessageToSendAsync,
            new string[] { "хочуиграть", "wantplay" }),
            (TransformMessageToRemoveAsync,
            new string[] { "удалиприглашение", "далиприглос", "removeinviting" })
            ){ }

        private static async Task TransformMessageToSendAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Активный_Участник)) return;

            string gameImage = null;
            string gameUrl = null;
            var argumentList = CommandManager.ClearAndGetCommandArguments(ref argument);
            foreach (var (arg, str) in argumentList)
            {
                switch (arg)
                {
                    case 'и':
                    case 'i': gameImage = str; break;
                    case 'с':
                    case 'l': gameUrl = str; break;
                }
            }
            await CommandManager.WantPlayMessage.SendAsync(message.Author, argument, gameImage, gameUrl);
        }

        private static async Task TransformMessageToRemoveAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Активный_Участник)) return;
            await CommandManager.WantPlayMessage.RemoveAsync(message.Author, ulong.Parse(argument));
        }

        public async Task SendAsync(IUser user, string gameName, string gameImage = null, string gameUrl = null)
        {
            if (gameName.Length > 64) return;
            var embedBuilder = new EmbedBuilder()
                .WithTitle(MessageTitles.Titles[TitleType.WantPlay])
                .WithDescription("Пользователь " + user.Mention + " приглашает в игру **" + gameName + "**.")
                .WithThumbnailUrl(gameImage)
                .WithColor(Color.DarkBlue);

            if (gameImage != null) embedBuilder.WithThumbnailUrl(gameImage);
            if (gameUrl != null) embedBuilder.WithUrl(gameUrl);


            var sendedMessage = await ((ISocketMessageChannel)BotClientManager.MainBot.Guild.GetChannel((ulong)ChannelIds.чат_игровой)).SendMessageAsync("", false, embedBuilder.Build());

            await sendedMessage.ModifyAsync((messageProperties) =>
            {
                messageProperties.Embed = embedBuilder.WithFooter(new EmbedFooterBuilder().WithText("ID Сообщения: " + sendedMessage.Id)).Build();
            });

            await sendedMessage.AddReactionAsync(new Emoji("✅"));
            await sendedMessage.AddReactionAsync(new Emoji("📩"));

            DataManager.AgreeingToPlayUsers.Value.Add(sendedMessage.Id, (sendedMessage.Timestamp, new List<ulong> { user.Id }));
            await DataManager.AgreeingToPlayUsers.SaveAsync();
        }

        private async Task RemoveAsync(IUser user, ulong messageId)
        {
            if (DataManager.AgreeingToPlayUsers.Value[messageId].Item2[0] == user.Id)
            {
                var foundedMessage = await ((ISocketMessageChannel)BotClientManager.MainBot.Guild.GetChannel((ulong)ChannelIds.чат_игровой)).GetMessageAsync(messageId);
                await foundedMessage.DeleteAsync();
                DataManager.AgreeingToPlayUsers.Value.Remove(messageId);
                await DataManager.AgreeingToPlayUsers.SaveAsync();
            }
        }

        public static async Task NotifySubscribedUsersAsync(ulong userId, string gameName)
        {
            var channel = BotClientManager.MainBot.Guild.GetChannel((ulong)ChannelIds.чат_игровой);
            var invite = await channel.CreateInviteAsync();
            var embedBuilder = new EmbedBuilder()
                .WithColor(Color.DarkBlue)
                .WithTitle(MessageTitles.Titles[TitleType.WantPlay])
                .WithDescription("Пользователь <@!" + userId + "> приглашает в игру **" + gameName + "**.\n" +
                "Принять приглашение можно здесь: " + invite)
                .WithFooter("Чтобы отключить оповещение или добавить ещё источников оповещения, напишите в этом чате: =моиподписки");
            var embed = embedBuilder.Build();
            //if (CheckConditions(userId, gameName))
                foreach (ulong subscriber in DataManager.Subscribers.Value[userId][gameName])
                    try { await BotClientManager.MainBot.Guild.GetUser(subscriber).SendMessageAsync("", false, embed); }
                    finally { }
            //if (CheckConditions(userId, "__AnyGame"))
                foreach (ulong subscriber in DataManager.Subscribers.Value[userId]["__AnyGame"])
                    try { await BotClientManager.MainBot.Guild.GetUser(subscriber).SendMessageAsync("", false, embed); }
                    finally { }
            //if (CheckConditions(0, gameName))
                foreach (ulong subscriber in DataManager.Subscribers.Value[0][gameName])
                    try { await BotClientManager.MainBot.Guild.GetUser(subscriber).SendMessageAsync("", false, embed); }
                    finally { }
            //if (CheckConditions(0, "__AnyGame"))
                foreach (ulong subscriber in DataManager.Subscribers.Value[0]["__AnyGame"])
                    try { await BotClientManager.MainBot.Guild.GetUser(subscriber).SendMessageAsync("", false, embed); }
                    finally { }
        }

        public static async Task AddUserAcceptedAsync(IMessage message, IUser user)
        {
            if (!DataManager.AgreeingToPlayUsers.Value.ContainsKey(message.Id)) return;
            if (DataManager.AgreeingToPlayUsers.Value[message.Id].Item2[0] == user.Id) return;
            if (DataManager.AgreeingToPlayUsers.Value[message.Id].Item2.Contains(user.Id)) return;
            DataManager.AgreeingToPlayUsers.Value[message.Id].Item2.Add(user.Id);
            await DataManager.AgreeingToPlayUsers.SaveAsync();
            string str = "";
            foreach(var userId in DataManager.AgreeingToPlayUsers.Value[message.Id].Item2)
            {
                if (userId == DataManager.AgreeingToPlayUsers.Value[message.Id].Item2[0]) continue;
                str += "<@!" + userId + ">, ";
            }
            if (str.Length > 2)
                {
                    str = str.Substring(0, str.Length - 2);
                    str = "\n\n" + "Приняли приглашение: " + str + ".";
                }
            await ((IUserMessage)message).ModifyAsync((messageProperties) => {
                var embed = message.Embeds.First();
                messageProperties.Embed = embed.ToEmbedBuilder().WithDescription(embed.Description.Split('\n')[0] + str).Build();
            });
        }

        public static async Task SendOptionsOfSubscriptionAsync(IMessage message, IUser user)
        {           
            if(!CommandManager.CheckPermission((IGuildUser)user, RoleIds.Активный_Участник)) return;

            ulong authorOfInvitingId = ulong.Parse(message.Embeds.First().Description.Substring(16, 18));
            if (user.Id == authorOfInvitingId) return;
            string game = message.Embeds.First().Description.Split('*')[2];
            var embedBuilder = new EmbedBuilder()
                .WithColor(Color.LighterGrey)
                .WithTitle(MessageTitles.Titles[TitleType.SubscriptionManager])
                .WithDescription("Выберите действие:\n\n" +
                $":one:Подписаться на приглашения от <@!{authorOfInvitingId}> в любую игру;\n" +
                $":two:Подписаться на приглашения от <@!{authorOfInvitingId}> только в **{game}**;\n" +
                $":three:Подписаться на приглашения ото всех только в **{game}**;\n" +
                $":four:Подписаться на приглашения ото всех в любую игру.\n" +
                $":five:Отписаться от приглашения от <@!{authorOfInvitingId}> в любую игру;\n" +
                $":six:Отписаться от приглашения от <@!{authorOfInvitingId}> только в **{game}**;\n" +
                $":seven:Отписаться от приглашения ото всех только в **{game}**;\n" +
                $":eight:Отписаться от приглашения ото всех в любую игру.");
            var sendedMessage = await BotClientManager.MainBot.Guild.GetUser(user.Id).SendMessageAsync("", false, embedBuilder.Build());

            await sendedMessage.AddReactionAsync(new Emoji("1\u20E3"));
            await sendedMessage.AddReactionAsync(new Emoji("2\u20E3"));
            await sendedMessage.AddReactionAsync(new Emoji("3\u20E3"));
            await sendedMessage.AddReactionAsync(new Emoji("4\u20E3"));
            await sendedMessage.AddReactionAsync(new Emoji("5\u20E3"));
            await sendedMessage.AddReactionAsync(new Emoji("6\u20E3"));
            await sendedMessage.AddReactionAsync(new Emoji("7\u20E3"));
            await sendedMessage.AddReactionAsync(new Emoji("8\u20E3"));
        }

        public static async Task RemoveUserAcceptedAsync(IMessage message, IUser user)
        {
            if (!DataManager.AgreeingToPlayUsers.Value.ContainsKey(message.Id)) return;
            if (DataManager.AgreeingToPlayUsers.Value[message.Id].Item2[0] == user.Id) return;
            if (!DataManager.AgreeingToPlayUsers.Value[message.Id].Item2.Contains(user.Id)) return;
            DataManager.AgreeingToPlayUsers.Value[message.Id].Item2.Remove(user.Id);
            await DataManager.AgreeingToPlayUsers.SaveAsync();
            string str = "";
            foreach (var userId in DataManager.AgreeingToPlayUsers.Value[message.Id].Item2)
            {
                if (userId == DataManager.AgreeingToPlayUsers.Value[message.Id].Item2[0]) continue;
                str += "<@!" + userId + ">, ";
            }
            if (str.Length > 2)
            {
                str = str.Substring(0, str.Length - 2);
                str = "\n\n" + "Приняли приглашение: " + str + ".";
            }
            await ((IUserMessage)message).ModifyAsync((messageProperties) => {
                var embed = message.Embeds.First();
                messageProperties.Embed = embed.ToEmbedBuilder().WithDescription(embed.Description.Split('\n')[0] + str).Build();
            });
        }

        public static async Task AddUserSubscriptionAsync(IMessage message, IUser user, int type)
        {
            ulong authorId = ulong.Parse(message.Embeds.First().Description.Split('!')[1].Split('>')[0]);
            string gameName = message.Embeds.First().Description.Split('*')[2];

            switch (type)
            {
                case 1:
                    await SubscribeAsync(authorId, "__AnyGame", user.Id); break;
                case 2:
                    await SubscribeAsync(authorId, gameName, user.Id); break;
                case 3:
                    await SubscribeAsync(0, gameName, user.Id); break;
                case 4:
                    await SubscribeAsync(0, "__AnyGame", user.Id); break;
            }           
        }

        public static async Task RemoveUserSubscriptionAsync(IMessage message, IUser user, int type)
        {
            ulong authorId = ulong.Parse(message.Embeds.First().Description.Split('!')[1].Split('>')[0]);
            string gameName = message.Embeds.First().Description.Split('*')[2];

            switch (type)
            {
                case 5:
                    await UnsubscribeAsync(authorId, "__AnyGame", user.Id); break;
                case 6:
                    await UnsubscribeAsync(authorId, gameName, user.Id); break;
                case 7:
                    await UnsubscribeAsync(0, gameName, user.Id); break;
                case 8:
                    await UnsubscribeAsync(0, "__AnyGame", user.Id); break;
            }
        }



        public static async Task SubscribeAsync(ulong userId, string gameName, ulong subscriber)
        {
            if (!DataManager.Subscribers.Value.ContainsKey(userId))
                DataManager.Subscribers.Value.Add(userId, new Dictionary<string, List<ulong>>());
            var buf1 = DataManager.Subscribers.Value[userId];
            if (!buf1.ContainsKey(gameName))
                buf1.Add(gameName, new List<ulong>());
            var buf2 = buf1[gameName];
            if (!buf2.Contains(subscriber))
                DataManager.Subscribers.Value[userId][gameName].Add(subscriber);
            await DataManager.Subscribers.SaveAsync();
        }
        public static async Task UnsubscribeAsync(ulong userId, string gameName, ulong subscriber)
        {
            if ((DataManager.Subscribers.Value.ContainsKey(userId))
                && (DataManager.Subscribers.Value[userId].ContainsKey(gameName))
                && (DataManager.Subscribers.Value[userId][gameName].Contains(subscriber)))
            {
                DataManager.Subscribers.Value[userId][gameName].Remove(subscriber);
                await DataManager.Subscribers.SaveAsync();
            }
        }
    }
}

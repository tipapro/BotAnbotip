using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using System.Linq;
using BotAnbotip.Bot.Data.Group;
using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data.CustomClasses;
using BotAnbotip.Bot.Data.CustomEnums;
using System.Collections.Generic;
using System;

namespace BotAnbotip.Bot.Commands
{
    public class AnonymousMessageCommands : CommandsBase
    {
        public AnonymousMessageCommands() : base
            (
            (TransformMessageToSendAsync, 
            new string[] { "анон", "анонимно", "anon", "anonymously" }),
            (TransformMessageToDeleteAsync,
            new string[] { "-анон", "удалианон", "-anon", "deleteanon", "deleteanonymousmessage" }),
            (TransformMessageToGetAuthorAsync,
            new string[] { "ктоанон", "whoisanon" })
            ){ }
        
        private static async Task TransformMessageToSendAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.DELETED_Active_Member)) return;
            await CommandManager.AnonymousMessage.SendAsync(message.Author, message.Channel, argument);
        }

        private static async Task TransformMessageToDeleteAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.DELETED_Active_Member)) return;
            ulong messageId = ulong.Parse(argument);
            await CommandManager.AnonymousMessage.DeleteAsync(message.Author, message.Channel, messageId);
        }

        private static async Task TransformMessageToGetAuthorAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Founder)) return;
            ulong messageId = ulong.Parse(argument);
            await CommandManager.AnonymousMessage.GetAuthorAsync(message.Author, messageId);
        }

        public async Task SendAsync(IUser user, IMessageChannel channel, string text)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle(MessageTitles.Titles[TitleType.Anonymous])
                .WithDescription(text)
                .WithColor(Color.DarkGrey);

            var sendedMessage = await channel.SendMessageAsync("", false, embedBuilder.Build());
            await sendedMessage.ModifyAsync((messageProperties) =>
            {
                messageProperties.Embed = embedBuilder.WithFooter(new EmbedFooterBuilder().WithText("ID Сообщения: " + sendedMessage.Id)).Build();
            });

            DataManager.AnonymousMessages.Value.Add(sendedMessage.Id, user.Id);
            await DataManager.AnonymousMessages.SaveAsync();
        }

        public async Task DeleteAsync(IUser user, IMessageChannel channel, ulong messageId)
        {
            if (DataManager.AnonymousMessages.Value[messageId] != user.Id) return;
            var foundedMessage = await channel.GetMessageAsync(messageId);
            await foundedMessage.DeleteAsync();
        }

        public async Task GetAuthorAsync(IUser user, ulong messageId)
        {
            ulong userId = DataManager.AnonymousMessages.Value[messageId];
            await user.SendMessageAsync(BotClientManager.MainBot.Guild.GetUser(userId).Mention);
        }
    }
}

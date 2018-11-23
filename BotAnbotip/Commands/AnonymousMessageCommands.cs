using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using BotAnbotip.Data;
using System.Linq;
using BotAnbotip.Data.Group;
using BotAnbotip.Clients;
using BotAnbotip.Data.CustomClasses;
using BotAnbotip.Data.CustomEnums;
using System.Collections.Generic;
using System;

namespace BotAnbotip.Commands
{
    public class AnonymousMessageCommands : CommandBase
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
            if (!CommandControlManager.CheckPermission((IGuildUser)message.Author, RoleIds.DELETED_Active_Member)) return;
            await CommandControlManager.AnonymousMessage.SendAsync(message.Author, message.Channel, argument);
        }

        private static async Task TransformMessageToDeleteAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandControlManager.CheckPermission((IGuildUser)message.Author, RoleIds.DELETED_Active_Member)) return;
            ulong messageId = ulong.Parse(argument);
            await CommandControlManager.AnonymousMessage.DeleteAsync(message.Author, message.Channel, messageId);
        }

        private static async Task TransformMessageToGetAuthorAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandControlManager.CheckPermission((IGuildUser)message.Author, RoleIds.Founder)) return;
            ulong messageId = ulong.Parse(argument);
            await CommandControlManager.AnonymousMessage.GetAuthorAsync(message.Author, messageId);
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

            DataControlManager.AnonymousMessages.Value.Add(sendedMessage.Id, user.Id);
            await DataControlManager.AnonymousMessages.SaveAsync();
        }

        public async Task DeleteAsync(IUser user, IMessageChannel channel, ulong messageId)
        {
            if (DataControlManager.AnonymousMessages.Value[messageId] != user.Id) return;
            var foundedMessage = await channel.GetMessageAsync(messageId);
            await foundedMessage.DeleteAsync();
        }

        public async Task GetAuthorAsync(IUser user, ulong messageId)
        {
            ulong userId = DataControlManager.AnonymousMessages.Value[messageId];
            await user.SendMessageAsync(ClientControlManager.MainBot.Guild.GetUser(userId).Mention);
        }
    }
}

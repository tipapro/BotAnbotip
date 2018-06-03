using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using System.Linq;
using BotAnbotip.Bot.Data.Group;
using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data.CustomClasses;
using BotAnbotip.Bot.Data.CustomEnums;

namespace BotAnbotip.Bot.Commands
{
    public class AnonymousMessageCommands
    {
        public static async Task SendAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Активный_Участник)) return;

            var embedBuilder = new EmbedBuilder()
                .WithTitle(MessageTitles.Titles[TitleType.Anonymous])
                .WithDescription(argument)
                .WithColor(Color.DarkGrey);

            var sendedMessage = await message.Channel.SendMessageAsync("", false, embedBuilder.Build());
            await sendedMessage.ModifyAsync(
                (messageProperties) => { messageProperties.Embed = embedBuilder.WithFooter(new EmbedFooterBuilder().WithText("MessageID: " + sendedMessage.Id)).Build(); });

            DataManager.AnonymousMessages.Value.Add(sendedMessage.Id, message.Author.Id);
            await DataManager.AnonymousMessages.SaveAsync();
        }

        public static async Task DeleteAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();

            ulong soughtForMessageId = ulong.Parse(argument);
            if (DataManager.AnonymousMessages.Value[soughtForMessageId] == message.Author.Id)
            {
                var foundedMessage = await message.Channel.GetMessageAsync(soughtForMessageId);
                await foundedMessage.DeleteAsync();
            }
        }

        public static async Task GetAnonymousUserAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

            ulong messageId = ulong.Parse(argument);
            ulong userId = DataManager.AnonymousMessages.Value[messageId];
            await message.Author.SendMessageAsync(BotClientManager.MainBot.Guild.GetUser(userId).Mention);
        }
    }
}

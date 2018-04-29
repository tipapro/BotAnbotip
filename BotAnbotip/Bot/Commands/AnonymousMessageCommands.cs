using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using System.Linq;
using BotAnbotip.Bot.Data.Group;

namespace BotAnbotip.Bot.Commands
{
    public class AnonymousMessageCommands
    {
        public static async Task SendAsync(SocketMessage message, string argument)
        {
            await message.DeleteAsync();

            var embedBuilder = new EmbedBuilder()
                .WithTitle(":spy:Анонимное сообщение:spy:")
                .WithDescription(argument)
                .WithColor(Color.DarkGrey);

            var sendedMessage = await message.Channel.SendMessageAsync("", false, embedBuilder.Build());
            await sendedMessage.ModifyAsync(
                (messageProperties) => { messageProperties.Embed = embedBuilder.WithFooter(new EmbedFooterBuilder().WithText("MessageID: " + sendedMessage.Id)).Build(); });

            DataManager.AnonymousMessages.Value.Add(sendedMessage.Id, message.Author.Id);
            await DataManager.AnonymousMessages.SaveAsync();
        }

        public static async Task DeleteAsync(SocketMessage message, string argument)
        {
            await message.DeleteAsync();

            ulong soughtForMessage = ulong.Parse(argument);
            if (DataManager.AnonymousMessages.Value[soughtForMessage] == message.Author.Id)
            {
                var foundedMessage = await message.Channel.GetMessageAsync(soughtForMessage);
                await foundedMessage.DeleteAsync();
            }
        }

        public static async Task GetAnonymousUserAsync(SocketMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

            ulong messageId = ulong.Parse(argument);
            ulong userId = DataManager.AnonymousMessages.Value[messageId];
            await message.Author.SendMessageAsync(ConstInfo.GroupGuild.GetUser(userId).Mention);
        }
    }
}

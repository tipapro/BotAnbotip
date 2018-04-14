using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using System.Linq;

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

            DataManager.anonymousMessagesAndUsersIds.Add(sendedMessage.Id, message.Author.Id);
            await DataManager.SaveDataAsync();
        }

        public static async Task DeleteAsync(SocketMessage message, string argument)
        {
            await message.DeleteAsync();

            ulong soughtForMessage = ulong.Parse(argument);
            if (DataManager.anonymousMessagesAndUsersIds[soughtForMessage] == message.Author.Id)
            {
                var foundedMessage = await message.Channel.GetMessageAsync(soughtForMessage);
                await foundedMessage.DeleteAsync();
            }
        }

        public static async Task GetAnonymousUserAsync(SocketMessage message, string argument)
        {
            await message.DeleteAsync();

            var userRoles = ((IGuildUser)message.Author).RoleIds;

            if (userRoles.Contains((ulong)RoleIds.Основатель))
            {
                ulong messageId = ulong.Parse(argument);

                ulong userId = DataManager.anonymousMessagesAndUsersIds[messageId];

                await message.Author.SendMessageAsync(Info.GroupGuild.GetUser(userId).Mention);
            }
        }
    }
}

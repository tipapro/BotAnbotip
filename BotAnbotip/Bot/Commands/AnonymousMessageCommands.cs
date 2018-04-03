using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;

namespace BotAnbotip.Bot.Commands
{
    public class AnonymousMessageCommands
    {
        public static async Task SendAsync(SocketMessage message, string argument)
        {
            await message.DeleteAsync();

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Анонимное сообщение")
                .WithDescription(argument)
                .WithColor(Color.Green);

            var sendedMessage = await message.Channel.SendMessageAsync("", false, embedBuilder.Build());
            await sendedMessage.ModifyAsync(
                (messageProperties) => { messageProperties.Embed = embedBuilder.WithFooter(new EmbedFooterBuilder().WithText("MessageID: " + sendedMessage.Id)).Build(); });

            DataManager.anonymousMessagesAndUsersIds.Add(sendedMessage.Id, message.Author.Id);
            DataManager.SaveData();
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
    }
}

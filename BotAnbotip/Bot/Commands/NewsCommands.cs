using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.Commands
{
    class NewsCommands
    {
        public static async Task SendAsync(SocketMessage message, string argument, bool hasImage = false)
        {
            await message.DeleteAsync();

            var embedBuilder = new EmbedBuilder()
                .WithTitle("📰Новость📰")
                .WithColor(Color.Orange);

            if (hasImage)
            {
                var str = argument.Split(' ');
                var imageUrl = str[0];
                embedBuilder.WithImageUrl(imageUrl);
                argument = argument.Substring(imageUrl.Length);
            }

            embedBuilder.WithDescription(argument);

            var sendedMessage = await message.Channel.SendMessageAsync("", false, embedBuilder.Build());

            await sendedMessage.AddReactionAsync(new Emoji("👍"));
            await sendedMessage.AddReactionAsync(new Emoji("👎"));
        }
    }
}

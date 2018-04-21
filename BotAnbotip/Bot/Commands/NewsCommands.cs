using BotAnbotip.Bot.Data.Group;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.Commands
{
    class NewsCommands
    {
        public static async Task SendAsync(SocketMessage message, string argument, bool hasImage = false, bool fromYT = false)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Модератор)) return;

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

            if (fromYT)
            {
                var str = argument.Split(' ');
                var url = str[0];
                string videoId = "";
                foreach (string bufStr in url.Split('/'))
                {
                    if (bufStr == "youtu.be") videoId = url.Substring(url.Length - 11);
                }

                if (videoId == "") videoId = url.Split('=')[1].Substring(0, 11);

                embedBuilder.WithImageUrl($"https://img.youtube.com/vi/{videoId}/hqdefault.jpg");
                argument = argument.Substring(url.Length);

                var newUrl = $"https://youtu.be/{videoId}";
                embedBuilder.WithDescription(argument).AddField(new string('-', 40), newUrl);           
            }


            embedBuilder.WithDescription(argument);

            var sendedMessage = await message.Channel.SendMessageAsync("", false, embedBuilder.Build());


            await sendedMessage.AddReactionAsync(new Emoji("👍"));
            await sendedMessage.AddReactionAsync(new Emoji("👎"));

        }
    }
}

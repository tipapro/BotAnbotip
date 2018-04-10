using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.Commands
{
    class AnnouncementCommands
    {
        public static async Task SendAsync(SocketMessage message, string argument)
        {
            await message.DeleteAsync();

            var embedBuilder = new EmbedBuilder()
                .WithTitle(":loudspeaker:Объявление:loudspeaker:")
                .WithDescription(argument)
                .WithColor(Color.Magenta);

            await message.Channel.SendMessageAsync("", false, embedBuilder.Build());

        }
    }
}

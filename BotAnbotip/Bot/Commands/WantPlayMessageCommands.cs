using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using Discord;
using Discord.WebSocket;

namespace BotAnbotip.Bot.Commands
{
    class WantPlayMessageCommands
    {
        public static async Task SenAsync(string game, SocketMessage message = null, IUser user = null, string gamePictureUrl = null, string url = null)
        {
            if (game.Length > 64) return;
            string username = "";
            var embedBuilder = new EmbedBuilder()                
                .WithColor(Color.DarkBlue).WithTitle("Приглашение в игру");


            if (message != null)
            {
                await message.DeleteAsync();
                username = message.Author.Mention;
            }
            if (user != null) username = user.Mention;
            if (gamePictureUrl != null) embedBuilder.WithThumbnailUrl(gamePictureUrl);
            if (url != null) embedBuilder.WithUrl(url);

            embedBuilder.WithDescription("Пользователь " + username + " приглашает в игру **" + game + "**.");

            var sendedMessage = await ((ISocketMessageChannel)Info.GroupGuild.GetChannel((ulong)ChannelIds.чат_игровой)).SendMessageAsync("", false, embedBuilder.Build());

            await sendedMessage.AddReactionAsync(new Emoji("✅"));
        }
    }
}

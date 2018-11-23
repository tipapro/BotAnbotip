using BotAnbotip.Data.CustomClasses;
using BotAnbotip.Data.CustomEnums;
using BotAnbotip.Data.Group;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Commands
{
    class AnnouncementCommands : CommandBase
    {
        public AnnouncementCommands() : base
            (
            (TransformMessageToSendAsync,
            new string[] { "объяви", "анонс", "announce" })
            ){ }

        private static async Task TransformMessageToSendAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandControlManager.CheckPermission((IGuildUser)message.Author, RoleIds.Admin)) return;
            await CommandControlManager.Announcement.SendAsync(message.Channel, argument);
        }

        public async Task SendAsync(IMessageChannel channel, string text)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle(MessageTitles.Titles[TitleType.Announcement])
                .WithDescription(text)
                .WithColor(Color.Magenta);

            await channel.SendMessageAsync("", false, embedBuilder.Build());
        }
    }
}

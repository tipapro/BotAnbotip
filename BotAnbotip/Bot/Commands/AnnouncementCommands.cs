using BotAnbotip.Bot.Data.CustomClasses;
using BotAnbotip.Bot.Data.CustomEnums;
using BotAnbotip.Bot.Data.Group;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.Commands
{
    class AnnouncementCommands : CommandsBase
    {
        public AnnouncementCommands() : base
            (
            (TransformMessageToSendAsync,
            new string[] { "объяви", "анонс", "announce" })
            ){ }

        private static async Task TransformMessageToSendAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Администратор)) return;
            await CommandManager.Announcement.SendAsync(message.Channel, argument);
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

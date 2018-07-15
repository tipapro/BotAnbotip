using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using Discord.WebSocket;
using BotAnbotip.Bot.Data.Group;
using BotAnbotip.Bot.Clients;
using Discord;
using BotAnbotip.Bot.Data.CustomClasses;
using BotAnbotip.Bot.Data.CustomEnums;
using System.Linq;

namespace BotAnbotip.Bot.Commands
{
    class RoleManagementCommands : CommandsBase
    {
        public RoleManagementCommands() : base
            (
            (TransformMessageToSendGreetingMessage,
            new string[] { "gr" }),
            (TransformMessageToSendRoleManageMessage,
            new string[] { "rm" })
            ){ }

        private static async Task TransformMessageToSendGreetingMessage(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;
            await CommandManager.RoleManagement.SendGreetingMessage(message.Channel);
        }

        private static async Task TransformMessageToSendRoleManageMessage(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;
            await CommandManager.RoleManagement.SendRoleManageMessage(message.Channel);
        }

        public async Task SendGreetingMessage(IMessageChannel channel)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle(MessageTitles.Titles[TitleType.Greeting])
                .WithDescription("**Выберите необходимый чат:**")
                .AddField("Игровая тематика", $"<#{(ulong)ChannelIds.чат_игровой}>", true)
                .AddField("Внеигровая тематика", $"<#{(ulong)ChannelIds.чат_флудилка}>", true)
                .WithColor(Color.Purple);

            await channel.SendMessageAsync("", false, embedBuilder.Build());
        }

        public async Task SendRoleManageMessage(IMessageChannel channel)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle(MessageTitles.Titles[TitleType.ManageRole])
                .WithDescription("Если вы хотите получить доступ к тематическим чатам, то нажми-те на нужные реакции:")
                .AddField(":musical_note:(9+ уровень)", $"<#{(ulong)ChannelIds.чат_музыкальный}>", true)
                .AddField(":u5272:(6+ уровень)", $"<#{(ulong)ChannelIds.чат_аниме}>", true)
                .WithColor(Color.Purple);

            var sendedMessage = await channel.SendMessageAsync("", false, embedBuilder.Build());
            await sendedMessage.AddReactionAsync(new Emoji("🎵"));
            await sendedMessage.AddReactionAsync(new Emoji("🈹"));
        }

        public async Task GetAsync(IUser user, ulong roleId)
        {
            await ((IGuildUser)user).AddRoleAsync(BotClientManager.MainBot.Guild.GetRole(roleId));
        }

        public async Task RemoveAsync(IUser user, ulong roleId)
        {
            await ((IGuildUser)user).RemoveRoleAsync(BotClientManager.MainBot.Guild.GetRole(roleId));
        }
    }
}

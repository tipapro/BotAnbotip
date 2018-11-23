using System;
using System.Threading.Tasks;
using BotAnbotip.Data.Group;
using BotAnbotip.Clients;
using Discord;
using BotAnbotip.Data.CustomClasses;
using BotAnbotip.Data.CustomEnums;
using System.Linq;

namespace BotAnbotip.Commands
{
    class RoleManagementCommands : CommandBase
    {
        public RoleManagementCommands() : base
            (
            (TransformMessageToSendGreetingMessage,
            new string[] { "gr" }),
            (TransformMessageToSendRoleManageMessage,
            new string[] { "rm" }),
            (TransformMessageToGiveRole,
            new string[] { "giverole", "дайроль" })
            ){ }

        private static async Task TransformMessageToSendGreetingMessage(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandControlManager.CheckPermission((IGuildUser)message.Author, RoleIds.Founder)) return;
            await CommandControlManager.RoleManagement.SendGreetingMessage(message.Channel);
        }

        private static async Task TransformMessageToSendRoleManageMessage(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandControlManager.CheckPermission((IGuildUser)message.Author, RoleIds.Founder)) return;
            await CommandControlManager.RoleManagement.SendRoleManageMessage(message.Channel);
        }

        private static async Task TransformMessageToGiveRole(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandControlManager.CheckPermission((IGuildUser)message.Author, RoleIds.Moderator)) return;
            var strArray = argument.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var userId = ulong.Parse(new string((from c in strArray[0]
                                                 where char.IsWhiteSpace(c) || char.IsNumber(c)
                                                 select c
                                                 ).ToArray()));
            var roleId = ulong.Parse(new string((from c in strArray[1]
                                                 where char.IsWhiteSpace(c) || char.IsNumber(c)
                                                 select c
                                                 ).ToArray()));
            await CommandControlManager.RoleManagement.GiveRoleAsync((IGuildUser)message.Author, userId, roleId);
        }

        public async Task SendGreetingMessage(IMessageChannel channel)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle(MessageTitles.Titles[TitleType.Greeting])
                .WithDescription("**Выберите необходимый чат:**")
                .AddField("Игровая тематика", $"<#{(ulong)ChannelIds.chat_gaming}>", true)
                .AddField("Внеигровая тематика", $"<#{(ulong)ChannelIds.chat_offtop}>", true)
                .WithColor(Color.Purple);

            await channel.SendMessageAsync("", false, embedBuilder.Build());
        }

        public async Task SendRoleManageMessage(IMessageChannel channel)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle(MessageTitles.Titles[TitleType.ManageRole])
                .WithDescription("Если вы хотите получить доступ к тематическим чатам, то нажмите на нужные реакции:")
                .AddField(":musical_note:(9+ уровень)", $"<#{(ulong)ChannelIds.chat_music}>", true)
                .AddField(":u5272:(6+ уровень)", $"<#{(ulong)ChannelIds.chat_anime}>", true)
                .WithColor(Color.Purple);

            var sendedMessage = await channel.SendMessageAsync("", false, embedBuilder.Build());
            await sendedMessage.AddReactionAsync(new Emoji("🎵"));
            await sendedMessage.AddReactionAsync(new Emoji("🈹"));
        }

        public async Task GetAsync(IUser user, ulong roleId)
        {
            await ((IGuildUser)user).AddRoleAsync(ClientControlManager.MainBot.Guild.GetRole(roleId));
        }

        public async Task RemoveAsync(IUser user, ulong roleId)
        {
            await ((IGuildUser)user).RemoveRoleAsync(ClientControlManager.MainBot.Guild.GetRole(roleId));
        }

        public async Task GiveRoleAsync(IGuildUser user, ulong userId, ulong roleId)
        {
            if (CommandControlManager.GetUserPermLevel(user.RoleIds) > CommandControlManager.GetRolePermLevel(roleId))
                await ClientControlManager.MainBot.Guild.GetUser(userId)
                    .AddRoleAsync(ClientControlManager.MainBot.Guild.GetRole(roleId));
        }
    }
}

using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.CustomClasses;
using BotAnbotip.Bot.Data.CustomEnums;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.Commands
{
    class UserProfileCommands : CommandsBase
    {
        public UserProfileCommands() : base
            (
            (TransformMessageToShowPointsAndLevel,
            new string[] { "мойуровень", "mylevel" })
            )
        { }

        private static async Task TransformMessageToShowPointsAndLevel(IMessage message, string argument)
        {
            await message.DeleteAsync();
            await CommandManager.UserProfile.ShowPointsAndLevel(message.Author);
        }

        public async Task ShowPointsAndLevel(IUser user)
        {
            if (!DataManager.UserProfiles.Value.ContainsKey(user.Id)) DataManager.UserProfiles.Value.Add(user.Id, new UserProfile(user.Id));
            var profile = DataManager.UserProfiles.Value[user.Id];
            var role = BotClientManager.MainBot.Guild.GetRole((ulong)LevelInfo.RoleList[profile.Level]);
            var embedBuilder = new EmbedBuilder()
                .WithTitle(MessageTitles.Titles[TitleType.UserLevel])
                .AddField("Профиль", user.Mention, true)
                .AddField("Звание", LevelInfo.RoleList[profile.Level], true)
                .AddField("Уровень", profile.Level, true)
                .AddField("Очки", profile.Points, true)
                .WithThumbnailUrl(user.GetAvatarUrl())
                .WithColor(role.Color);

            await user.SendMessageAsync("", false, embedBuilder.Build());
            await DataManager.UserProfiles.SaveAsync();
        }
    }
}

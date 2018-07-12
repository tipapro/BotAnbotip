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
            await CommandManager.UserProfile.ShowPointsAndLevel(message.Author, message.Channel);
        }

        public async Task ShowPointsAndLevel(IUser user, IMessageChannel channel)
        {
            if (!DataManager.UserProfiles.Value.ContainsKey(user.Id)) DataManager.UserProfiles.Value.Add(user.Id, new UserProfile(user.Id));
            var profile = DataManager.UserProfiles.Value[user.Id];
            var embedBuilder = new EmbedBuilder()
                .WithTitle(MessageTitles.Titles[TitleType.UserLevel])
                .WithDescription("Пользователь: " + user.Mention)
                .AddField("Ваш уровень", profile.Level, true)
                .AddField("Ваши очки", profile.Points, true)
                .WithColor(Color.Orange);

            await channel.SendMessageAsync("", false, embedBuilder.Build());
        }
    }
}

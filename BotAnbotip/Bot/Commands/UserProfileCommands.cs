using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.CustomClasses;
using BotAnbotip.Bot.Data.CustomEnums;
using BotAnbotip.Bot.Data.Group;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.Commands
{
    class UserProfileCommands : CommandsBase
    {
        public UserProfileCommands() : base
            (
            (TransformMessageToShowPointsAndLevel,
            new string[] { "мойуровень", "mylevel" }),
            (TransformMessageToFine,
            new string[] { "штраф", "fine" }),
            (TransformMessageToReward,
            new string[] { "награди", "reward"})
            )
        { }

        private static async Task TransformMessageToShowPointsAndLevel(IMessage message, string argument)
        {
            await message.DeleteAsync();
            await CommandManager.UserProfile.ShowPointsAndLevel(message.Author);
        }

        private static async Task TransformMessageToFine(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;
            var strArray = argument.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var userId = ulong.Parse(new string((from c in strArray[0]
                                              where char.IsNumber(c)
                                              select c).ToArray()));
            var points = long.Parse(strArray[1]);
            await DataManager.UserProfiles.Value[userId].RemovePoints(points);
            await DataManager.UserProfiles.SaveAsync();
            await BotClientManager.MainBot.Log(new LogMessage(LogSeverity.Info, 
                "LevelChange: Remove", "Who: " + message.Author.Id + " How much: " + points + " To whom: " + userId));
        }

        private static async Task TransformMessageToReward(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;
            var strArray = argument.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var userId = ulong.Parse(new string((from c in strArray[0]
                                                 where char.IsNumber(c)
                                                 select c).ToArray()));
            var points = long.Parse(strArray[1]);
            await DataManager.UserProfiles.Value[userId].AddPoints(points > 100000 ? 100000 : points);
            await DataManager.UserProfiles.SaveAsync();
            await BotClientManager.MainBot.Log(new LogMessage(LogSeverity.Info,
                "LevelChange: Add", "Who: " + message.Author.Id + " How much: " + points + " To whom: " + userId));
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

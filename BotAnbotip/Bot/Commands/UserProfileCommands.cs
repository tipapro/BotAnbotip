using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.CustomClasses;
using BotAnbotip.Bot.Data.CustomEnums;
using BotAnbotip.Bot.Data.Group;
using BotAnbotip.Bot.OtherModules;
using Discord;
using Discord.WebSocket;
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
            new string[] { "награди", "reward" }),
            (TransformMessageToUpdateAll,
            new string[] { "обновиуровни" })
            )
        { }

        private static async Task TransformMessageToShowPointsAndLevel(IMessage message, string argument)
        {
            if (message.Channel.Id != (ulong)ChannelIds.chat_botcmd) return;
            await message.DeleteAsync();
            await CommandManager.UserProfile.ShowPointsAndLevel(message.Author);
        }

        private static async Task TransformMessageToFine(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Founder)) return;
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
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Founder)) return;
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

        private static async Task TransformMessageToUpdateAll(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Founder)) return;
            await CommandManager.UserProfile.UpdateAll();
        }

        public async Task ShowPointsAndLevel(IUser user)
        {
            if (!DataManager.UserProfiles.Value.ContainsKey(user.Id)) DataManager.UserProfiles.Value.Add(user.Id, new UserProfile(user.Id));
            var profile = DataManager.UserProfiles.Value[user.Id];
            var role = BotClientManager.MainBot.Guild.GetRole((ulong)LevelInfo.RoleList[profile.Level]);
            var nextLevelPoints = LevelInfo.RoleList.Length > profile.Level + 1 ?
                LevelInfo.Points[LevelInfo.RoleList[profile.Level + 1]] : profile.Points;
            var curLevelPoints = LevelInfo.Points[LevelInfo.RoleList[profile.Level]];
            var toNextLevelPoints = nextLevelPoints - curLevelPoints;
            var scoredPoints = profile.Points - curLevelPoints;

            var embedBuilder = new EmbedBuilder()
                .WithTitle(MessageTitles.Titles[TitleType.UserLevel])
                .WithDescription("``" + OtherMethods.GenerateTextProgressBar(scoredPoints, toNextLevelPoints) + "\n" +
                scoredPoints.ToString("N0", new System.Globalization.CultureInfo("ru-ru")) + " / " +
                toNextLevelPoints.ToString("N0", new System.Globalization.CultureInfo("ru-ru")) + " (" +
                Math.Round(scoredPoints * 100f / toNextLevelPoints, 2).ToString("N2", new System.Globalization.CultureInfo("ru-ru")) + "%)``")
                .AddField("Профиль", user.Mention, true)
                .AddField("Звание", "<@&" + (ulong)LevelInfo.RoleList[profile.Level] + ">", true)
                .AddField("Уровень", profile.Level, true)
                .AddField("Всего очков", profile.Points.ToString("N0", new System.Globalization.CultureInfo("ru-ru")), true)

                .WithThumbnailUrl(user.GetAvatarUrl())
                .WithColor(role.Color);

            await ((ITextChannel)BotClientManager.MainBot.Guild.GetChannel((ulong)ChannelIds.chat_botcmd)).SendMessageAsync("", false, embedBuilder.Build());
            await DataManager.UserProfiles.SaveAsync();
        }

        public async Task UpdateAll()
        {
            var toRemove = new List<ulong>();
            foreach (var (userId, userProfile) in DataManager.UserProfiles.Value)
            {
                var user = BotClientManager.MainBot.Guild.GetUser(userId);
                if (user.IsBot)
                {
                    toRemove.Add(userId);
                    continue;
                }
                if (user is null) continue;
                var userRoles = user.Roles;
                foreach (var role in userRoles)
                    if (LevelInfo.RoleList.Contains((LevelRoleIds)role.Id)) await user.RemoveRoleAsync(role);
                for (int i = 1; i <= LevelInfo.RoleList.Length; i++)
                {
                    if (LevelInfo.Points[LevelInfo.RoleList[i]] < userProfile.Points) continue;
                    await user.AddRoleAsync(BotClientManager.MainBot.Guild.GetRole((ulong)LevelInfo.RoleList[i - 1]));
                    await userProfile.UpdateLevel();
                    break;
                }
                await Task.Delay(100);
            }
            foreach (var id in toRemove) DataManager.UserProfiles.Value.Remove(id);
            await DataManager.UserProfiles.SaveAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BotAnbotip.Clients;
using BotAnbotip.Data;
using BotAnbotip.Data.CustomClasses;
using BotAnbotip.Data.CustomEnums;
using BotAnbotip.Data.Group;
using Discord;

namespace BotAnbotip.Services
{
    class TopUpdater : ServiceBase
    {
        const int AmountOfTopUsers = 100;

        public TopUpdater(ClientBase botClient, string serviceName) : base(botClient, serviceName)
        {
        }

        protected override async Task Cycle(CancellationToken token)
        {
            List<ulong> lastUsers = new List<ulong>();
            while (IsStarted)
            {
                await Task.Delay(new TimeSpan(0, 5, 0), token);
                    DataControlManager.UserTopList.Value = new List<(ulong, long, int)>();
                foreach (var (id, user) in DataControlManager.UserProfiles.Value)
                {
                    if (DataControlManager.UserTopList.Value.Contains((id, user.Points, user.Level)))
                        continue;
                    if (DataControlManager.UserTopList.Value.Count < AmountOfTopUsers)
                    {
                        DataControlManager.UserTopList.Value.Add((id, user.Points, user.Level));
                        if (DataControlManager.UserTopList.Value.Count == AmountOfTopUsers) DataControlManager.UserTopList.Value.Sort(
                            new Comparison<(ulong, long, int)>((firstObj, secondObj) => secondObj.Item2.CompareTo(firstObj.Item2)));
                    }
                    else
                    {
                        for (int i = DataControlManager.UserTopList.Value.Count - 1; i >= 0; i--)
                        {
                            if (user.Points > DataControlManager.UserTopList.Value[i].Points)
                            {
                                for (int j = DataControlManager.UserTopList.Value.Count - 1; j > i; j--)
                                {
                                    DataControlManager.UserTopList.Value[j] = DataControlManager.UserTopList.Value[j - 1];
                                }
                                DataControlManager.UserTopList.Value[i] = (id, user.Points, user.Level);
                                break;
                            }
                        }
                    }
                    DataControlManager.UserTopList.Value.Sort(
                            new Comparison<(ulong, long, int)>((firstObj, secondObj) => secondObj.Item2.CompareTo(firstObj.Item2)));
                }             
                await DataControlManager.UserTopList.SaveAsync();

                DisplayTop();
            }
        }

        private async void DisplayTop()
        {
            var channel = ClientControlManager.MainBot.Guild.GetTextChannel((ulong)ChannelIds.usertop);
            var messages = (await channel.GetMessagesAsync(DataControlManager.UserTopList.Value.Count).FlattenAsync()).ToList();

            for (int i = 0; i < DataControlManager.UserTopList.Value.Count; i++)
            {
                var (userId, points, level) = DataControlManager.UserTopList.Value[i];
                var role = ClientControlManager.MainBot.Guild.GetRole((ulong)LevelInfo.RoleList[level]);
                var user = ClientControlManager.MainBot.Guild.GetUser(userId);
                var str = $"**{i + 1})** `{points.ToString("N0", new System.Globalization.CultureInfo("ru-ru"))}` " +
                    $"<@&{role.Id}> ";
                var embedBuilder = new EmbedBuilder()
                .WithAuthor(user?.Username ?? "#Unknown" + user?.Discriminator ?? "User", user?.GetAvatarUrl())
                .WithDescription(str)
                .WithColor(role.Color);
                
                if (i == DataControlManager.UserTopList.Value.Count - 1)
                    embedBuilder = embedBuilder.WithFooter("Последнее обновление: ").WithCurrentTimestamp();

                if (messages.Count() <= i) await channel.SendMessageAsync("", false, embedBuilder.Build());
                else await ((IUserMessage)messages[messages.Count() - i - 1]).ModifyAsync((prop) => { prop.Embed = embedBuilder.Build(); });
            }
            
        }
    }
}

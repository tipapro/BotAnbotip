using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.CustomClasses;
using BotAnbotip.Bot.Data.CustomEnums;
using BotAnbotip.Bot.Data.Group;
using Discord;

namespace BotAnbotip.Bot.Services
{
    class TopUpdatingService : ServiceBase
    {
        const int AmountOfTopUsers = 10;

        public TopUpdatingService(BotClientBase botClient, string errorMessage, string startMessage, string stopMessage) :
            base(botClient, errorMessage, startMessage, stopMessage)
        {
        }

        protected override async Task Cycle(CancellationToken token)
        {
            List<ulong> lastUsers = new List<ulong>();
            while (IsStarted)
            {
                await Task.Delay(new TimeSpan(0, 5, 0), token);
                
                DataManager.UserTopList.Value = DataManager.UserTopList.Value.Count == 0 ? DataManager.UserTopList.Value : 
                    new List<(ulong, long, int)>();
                foreach(var (id , user) in DataManager.UserProfiles.Value)
                {
                    if (DataManager.UserTopList.Value.Contains((id, user.Points, user.Level))) continue;
                    if (DataManager.UserTopList.Value.Count < AmountOfTopUsers)
                    {
                        DataManager.UserTopList.Value.Add((id, user.Points, user.Level));
                        if (DataManager.UserTopList.Value.Count == AmountOfTopUsers) DataManager.UserTopList.Value.Sort(
                            new Comparison<(ulong, long, int)>((firstObj, secondObj) => secondObj.Item2.CompareTo(firstObj.Item2)));
                    }
                    else
                    {
                        for (int i = DataManager.UserTopList.Value.Count - 1; i >= 0; i--)
                        {
                            if (user.Points > DataManager.UserTopList.Value[i].Item2)
                            {
                                for (int j = DataManager.UserTopList.Value.Count - 1; j > i; j--)
                                {
                                    DataManager.UserTopList.Value[j] = DataManager.UserTopList.Value[j - 1];
                                }
                                DataManager.UserTopList.Value[i] = (id, user.Points, user.Level);
                                break;
                            }
                        }
                    }
                    DataManager.UserTopList.Value.Sort(
                            new Comparison<(ulong, long, int)>((firstObj, secondObj) => secondObj.Item2.CompareTo(firstObj.Item2)));
                }              
                await DataManager.UserTopList.SaveAsync();

                DisplayTop();
            }
        }

        private async void DisplayTop()
        {
            var resultStr = "";
            for (int i = 0; i < DataManager.UserTopList.Value.Count; i++)
            {
                resultStr += "**" + (i + 1) + ")** `[" + DataManager.UserTopList.Value[i].Item2.ToString("N0", new System.Globalization.CultureInfo("ru-ru")) +
                    "]` <@&" + (ulong)LevelInfo.RoleList[DataManager.UserTopList.Value[i].Item3] + "> " +
                    " <@" + DataManager.UserTopList.Value[i].Item1 + ">\n";
            }
            var embedBuilder = new EmbedBuilder()
                .WithTitle(MessageTitles.Titles[TitleType.UsersTop])
                .WithDescription(resultStr)
                .WithColor(Color.DarkTeal)
                .WithCurrentTimestamp()
                .WithFooter("Последнее обновление: ");
            var channel = BotClientManager.MainBot.Guild.GetTextChannel((ulong)ChannelIds.usertop);
            var message = await channel.GetMessagesAsync(1).FlattenAsync();
            if (message.Count() == 0) await channel.SendMessageAsync("", false, embedBuilder.Build());
            else await((IUserMessage)message.First()).ModifyAsync((prop) => { prop.Embed = embedBuilder.Build(); });
        }
    }
}

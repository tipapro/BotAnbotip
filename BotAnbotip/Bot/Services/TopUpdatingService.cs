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
        const int AmountOfTopUsers = 20;

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
                string resultString = "";
                DataManager.UserTopList.Value = DataManager.UserTopList.Value.Count == 0 ? DataManager.UserTopList.Value : new List<(ulong, long)>();
                foreach(var pair in DataManager.UserProfiles.Value)
                {
                    if (DataManager.UserTopList.Value.Contains((pair.Key, pair.Value.Points))) continue;
                    if (DataManager.UserTopList.Value.Count < AmountOfTopUsers)
                    {
                        DataManager.UserTopList.Value.Add((pair.Key, pair.Value.Points));
                    }
                    else
                    {
                        for (int i = 0; i < DataManager.UserTopList.Value.Count - 1; i--)
                        {
                            if (pair.Value.Points > DataManager.UserTopList.Value[i].Item2)
                            {
                                DataManager.UserTopList.Value[i] = (pair.Key, pair.Value.Points);
                            }
                        }
                    }
                }
                DataManager.UserTopList.Value.Sort(
                    new Comparison<(ulong, long)>((firstObj, secondObj) => secondObj.Item2.CompareTo(firstObj.Item2)));
                await DataManager.UserTopList.SaveAsync();
                var embedBuilder = new EmbedBuilder()
                .WithTitle(MessageTitles.Titles[TitleType.Anonymous])
                .WithColor(Color.DarkGrey);
                for (int i = 0; i < DataManager.UserTopList.Value.Count; i++)
                {
                    embedBuilder.AddField("", (i + 1) + ") " + 
                        DataManager.UserTopList.Value[i].Item2 + " <@" + DataManager.UserTopList.Value[i].Item1 + ">");
                }
                var channel = BotClientManager.MainBot.Guild.GetTextChannel((ulong)ChannelIds.top20);
                var message = (IUserMessage)(await channel.GetMessagesAsync(1).FlattenAsync())?.First();
                if (message == null) await channel.SendMessageAsync("", false, embedBuilder.Build());
                else await message.ModifyAsync((prop) => { prop.Embed = embedBuilder.Build(); });
            }
        }
    }
}

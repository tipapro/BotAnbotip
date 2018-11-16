﻿using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.CustomClasses;
using BotAnbotip.Bot.Data.CustomEnums;
using BotAnbotip.Bot.Data.Group;
using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.Services
{
    class VipRoleGiveawayService : ServiceBase
    {
        private static Random random = new Random();
        public static string RandomOrgURL = "";

        public VipRoleGiveawayService(BotClientBase botClient, string serviceName) : base(botClient, serviceName)
        {
        }

        protected override async Task Cycle(CancellationToken token)
        {
            while (IsStarted)
            {
                if (((DateTime.UtcNow + new TimeSpan(3, 0, 0)).DayOfWeek != DayOfWeek.Monday) && (DataManager.DidRoleGiveawayBegin == true))
                    await DataManager.DidRoleGiveawayBegin.SaveAsync(false);
                while ((DateTime.UtcNow + new TimeSpan(3, 0, 0)).DayOfWeek != DayOfWeek.Monday && !DataManager.DebugTriger[0])
                    await Task.Delay(new TimeSpan(0, 10, 0), token);
                DataManager.DebugTriger[0] = false;
                if (!DataManager.DidRoleGiveawayBegin.Value && !DataManager.DebugTriger[1])
                {
                    DataManager.DebugTriger[1] = false;
                    await DataManager.DidRoleGiveawayBegin.SaveAsync(true);
                    
                    var winnerId = await ChooseTheWinner();

                    DisplayResult(winnerId);

                    if (!DataManager.UserProfiles.Value.ContainsKey(winnerId))
                        DataManager.UserProfiles.Value.Add(winnerId, new UserProfile(winnerId));
                    await DataManager.UserProfiles.Value[winnerId].AddPoints((long)ActionsCost.Percents_VIPWin, true);
                    await DataManager.UserProfiles.SaveAsync();
                }
            }
        }

        private async void DisplayResult(ulong winnerId)
        {
            var winnerText = winnerId == 0 ? "Победитель не определён из-за нехватки участников." :
                        $"Победитель этой недели: <@!{winnerId}>.";

            var giveawayText = "\nА наш розыгрыш продолжается.\n" +
                "Условия:\n" +
                "```1) Поставьте лайк этому посту;\n" +
                "2) Ждать понедельника.\n```" +
                $"В понедельник бот выберет случайного лайкнувшего этот пост пользователя и выдаст ему " +
                $"<@&{(ulong)RoleIds.VIP}> роль на неделю + {(int)ActionsCost.Percents_VIPWin}% очков.";

            var embedBuilder = new EmbedBuilder()
                .WithTitle(MessageTitles.Titles[TitleType.VipGiveaway])
                .WithDescription(winnerText + giveawayText)
                .WithColor(Color.Blue);

            var sendedMessage = await BotClientManager.MainBot.Guild
                .GetTextChannel((ulong)ChannelIds.giveaways).SendMessageAsync("", false, embedBuilder.Build());
            await sendedMessage.AddReactionAsync(new Emoji("💙"));
            await sendedMessage.PinAsync(); 
        }

        public static async Task<ulong> ChooseTheWinner()
        {
            if (!DataManager.ParticipantsOfTheGiveaway.Value.ContainsKey(GiveawayType.VIP)
                        || DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP].Count == 0)
                return 0;
            ulong winner;
            if (DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP].Count != 1)
            {

                var maxRand = DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP].Count - 1;
                var randomNum = await GetRandomNumber(0, maxRand);

                Console.WriteLine("Рандомное число: " + randomNum + " из " + maxRand);
                winner = DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP][randomNum];
                if (DataManager.LastWinner.Value.ContainsKey(GiveawayType.VIP) && DataManager.LastWinner.Value[GiveawayType.VIP] == winner)
                {
                    randomNum = await GetRandomNumber(0, maxRand);
                    Console.WriteLine("Рандомное число: " + randomNum + " из " + maxRand);
                    winner = DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP][randomNum];
                }
            }
            else winner = DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP][0];
            await BotClientManager.MainBot.Guild.GetUser(winner).AddRoleAsync(BotClientManager.MainBot.Guild.GetRole((ulong)RoleIds.VIP));
            if (DataManager.LastWinner.Value.ContainsKey(GiveawayType.VIP) && DataManager.LastWinner.Value[GiveawayType.VIP] != 0)
                await BotClientManager.MainBot.Guild.GetUser(DataManager.LastWinner.Value[GiveawayType.VIP])
                 .RemoveRoleAsync(BotClientManager.MainBot.Guild.GetRole((ulong)RoleIds.VIP));
            DataManager.LastWinner.Value[GiveawayType.VIP] = winner;
            await DataManager.LastWinner.SaveAsync();

            if (DataManager.ParticipantsOfTheGiveaway.Value.ContainsKey(GiveawayType.VIP))
                DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP] = new List<ulong>();
            else DataManager.ParticipantsOfTheGiveaway.Value.Add(GiveawayType.VIP, new List<ulong>());
            await DataManager.ParticipantsOfTheGiveaway.SaveAsync();

            return winner;
        }

        public static async Task<int> GetRandomNumber(int min, int max)
        {
            int counter = 0;
            int randomNum = 0;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                "https://" + $"www.random.org/integers/?num=1&min={min}&max={max}&col=1&base=10&format=plain&rnd=new");
            try
            {
                while (counter < 5)
                {
                    var resp = (HttpWebResponse)await request.GetResponseAsync();
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    {
                        if (int.TryParse(sr.ReadToEnd().Trim(), out randomNum))
                            return randomNum;
                        else
                        {
                            counter++;
                            await Task.Delay(5000);
                            continue;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                new ExceptionLogger().Log(ex, "Ошибка при запросе к Random.org, будет использован метод Random.Next()");
                return random.Next(min, max + 1);
            }
            if (counter > 4) randomNum = random.Next(min, max + 1);
            return randomNum;
        }
    }
}

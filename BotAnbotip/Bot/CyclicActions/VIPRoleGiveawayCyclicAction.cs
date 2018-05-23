using BotAnbotip.Bot.Clients;
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

namespace BotAnbotip.Bot.CyclicActions
{
    class VipRoleGiveawayCyclicAction : CyclicActionBase
    {
        private static Random random = new Random();
        public static string RandomOrgURL = "";

        public VipRoleGiveawayCyclicAction(BotClientBase botClient, string errorMessage, string stopMessage) :
    base(botClient, errorMessage, stopMessage)
        {
            _cycleMethod = Cycle;
        }

        private async Task Cycle(CancellationToken token)
        {
            while (IsStarted)
            {
                if (!DataManager.DidRoleGiveawayBegin.Value)
                {
                    await Task.Delay(new TimeSpan(0, 10, 0), token);
                    if (DateTime.Now.DayOfWeek != DayOfWeek.Friday && !DataManager.DebugTriger[0]) continue;
                    DataManager.DebugTriger[0] = false;

                    var embedBuilder1 = new EmbedBuilder()
                        .WithTitle(":gift:Еженедельный розыгрыш VIP роли:gift:")
                        .WithDescription("Правила розыгрыша таковы:\n" +
                        "```1) Поставьте лайк этому посту;\n" +
                        "2) Ждать понедельника.\n```" +
                        "В понедельник бот выберет случайного лайкнувшего этот пост пользователя и выдаст ему VIP роль на неделю.")
                        .WithColor(Color.Blue);
                    var sendedMessage = await BotClientManager.MainBot.Guild.GetTextChannel((ulong)ChannelIds.чат_флудилка).SendMessageAsync("", false, embedBuilder1.Build());
                    await sendedMessage.AddReactionAsync(new Emoji("💙"));

                    DataManager.ParticipantsOfTheGiveaway.Value.Add(GiveawayType.VIP, new List<ulong>());
                    await DataManager.DidRoleGiveawayBegin.SaveAsync(true);
                    await DataManager.ParticipantsOfTheGiveaway.SaveAsync();
                }

                while (DateTime.Now.DayOfWeek != DayOfWeek.Monday && !DataManager.DebugTriger[1]) await Task.Delay(new TimeSpan(0, 10, 0), token);
                var embedBuilder2 = new EmbedBuilder();
                if (!DataManager.ParticipantsOfTheGiveaway.Value.ContainsKey(GiveawayType.VIP)
                    || DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP].Count == 0)
                {
                    embedBuilder2
                        .WithTitle(":gift:Итоги еженедельного розыгрыша VIP роли:gift:")
                        .WithDescription("Розыгрыш не состоялся вследствие нехватки участников.")
                        .WithColor(Color.Blue);
                }
                else if (DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP].Count > 0)
                {
                    var maxRand = DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP].Count - 1;


                    var randomNum = await GetRandomNumber(0, maxRand);
                    Console.WriteLine("Рандомное число: " + randomNum + " из " + maxRand);
                    ulong winner = DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP][randomNum];
                    if (DataManager.LastWinner.Value.ContainsKey(GiveawayType.VIP) && DataManager.LastWinner.Value[GiveawayType.VIP] == winner)
                    {
                        randomNum = await GetRandomNumber(0, maxRand);
                        Console.WriteLine("Рандомное число: " + randomNum + " из " + maxRand);
                        winner = DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP][randomNum];
                    }
                    await BotClientManager.MainBot.Guild.GetUser(winner).AddRoleAsync(BotClientManager.MainBot.Guild.GetRole((ulong)RoleIds.VIP));
                    if (DataManager.LastWinner.Value.ContainsKey(GiveawayType.VIP) && DataManager.LastWinner.Value[GiveawayType.VIP] != 0)
                        await BotClientManager.MainBot.Guild.GetUser(DataManager.LastWinner.Value[GiveawayType.VIP])
                         .RemoveRoleAsync(BotClientManager.MainBot.Guild.GetRole((ulong)RoleIds.VIP));
                    DataManager.LastWinner.Value[GiveawayType.VIP] = winner;
                    await DataManager.LastWinner.SaveAsync();

                    embedBuilder2
                        .WithTitle(MessageTitles.Titles[TitleType.Giveaway])
                        .WithDescription("Победитель этой недели: <@!" + winner + ">.")
                        .WithColor(Color.Blue);
                }
                DataManager.ParticipantsOfTheGiveaway.Value.Remove(GiveawayType.VIP);
                await DataManager.ParticipantsOfTheGiveaway.SaveAsync();
                await DataManager.DidRoleGiveawayBegin.SaveAsync(false);

                await BotClientManager.MainBot.Guild.GetTextChannel((ulong)ChannelIds.чат_флудилка).SendMessageAsync("", false, embedBuilder2.Build());
            }

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
                    HttpWebResponse resp = (HttpWebResponse)await request.GetResponseAsync();
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

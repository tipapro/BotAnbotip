using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.CustomEnums;
using BotAnbotip.Bot.Data.Group;
using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.CyclicActions
{
    class RainbowRoleGiveaway
    {
        private static bool flag;
        private static Random random = new Random();
        public static string RandomOrgURL = "";
        public static async void Run()
        {
            flag = true;
            States.RainbowRoleGiveawayIsRunning = true;

            try
            {
                while (flag)
                {
                    if (!DataManager.DidRoleGiveawayBegin.Value)
                    {
                        await Task.Delay(new TimeSpan(0, 0, 10, 0));
                        if (DateTime.Now.DayOfWeek != DayOfWeek.Friday && !DataManager.DebugTriger[0]) continue;
                        DataManager.DebugTriger[0] = false;

                        var embedBuilder1 = new EmbedBuilder()
                            .WithTitle(":gift:Еженедельный розыгрыш VIP роли:gift:")
                            .WithDescription("Правила розыгрыша таковы:\n" +
                            "```1) Поставьте лайк этому посту;\n" +
                            "2) Ждать понедельника.\n```" +
                            "В понедельник бот выберет случайного лайкнувшего этот пост пользователя и выдаст ему VIP роль на неделю.")
                            .WithColor(Color.Blue);
                        var sendedMessage = await ConstInfo.GroupGuild.GetTextChannel((ulong)ChannelIds.чат_флудилка).SendMessageAsync("", false, embedBuilder1.Build());
                        await sendedMessage.AddReactionAsync(new Emoji("💙"));

                        DataManager.DidRoleGiveawayBegin.Value = true;
                        DataManager.ParticipantsOfTheGiveaway.Value.Add(GiveawayType.VIP, new List<ulong>());
                        await DataManager.ParticipantsOfTheGiveaway.SaveAsync();

                        while (DateTime.Now.DayOfWeek != DayOfWeek.Monday && !DataManager.DebugTriger[1]) await Task.Delay(new TimeSpan(0, 0, 10, 0));
                    }

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
                        var maxRand = DataManager.ParticipantsOfTheGiveaway.Value.Count - 1;


                        var randomNum = await GetRandomNumber(0, maxRand);
                        Console.WriteLine("Рандомное число: " + randomNum + " из " + maxRand);
                        ulong winner = DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP][randomNum];
                        if (DataManager.LastWinner.Value.ContainsKey(GiveawayType.VIP) && DataManager.LastWinner.Value[GiveawayType.VIP] == winner)
                        {
                            randomNum = await GetRandomNumber(0, maxRand);
                            Console.WriteLine("Рандомное число: " + randomNum + " из " + maxRand);
                            winner = DataManager.ParticipantsOfTheGiveaway.Value[GiveawayType.VIP][randomNum];
                        }
                        await ConstInfo.GroupGuild.GetUser(winner).AddRoleAsync(ConstInfo.GroupGuild.GetRole((ulong)RoleIds.VIP));
                        if (DataManager.LastWinner.Value.ContainsKey(GiveawayType.VIP) && DataManager.LastWinner.Value[GiveawayType.VIP] != 0)
                            await ConstInfo.GroupGuild.GetUser(DataManager.LastWinner.Value[GiveawayType.VIP])
                             .RemoveRoleAsync(ConstInfo.GroupGuild.GetRole((ulong)RoleIds.VIP));
                        DataManager.LastWinner.Value[GiveawayType.VIP] = winner;
                        await DataManager.LastWinner.SaveAsync();

                        embedBuilder2
                            .WithTitle(":gift:Итоги еженедельного розыгрыша VIP роли:gift: ")
                            .WithDescription("Победитель этой недели: <@!" + winner + ">, ")
                            .WithColor(Color.Blue);
                    }
                    DataManager.ParticipantsOfTheGiveaway.Value.Remove(GiveawayType.VIP);
                    DataManager.DidRoleGiveawayBegin.Value = false;
                    await DataManager.ParticipantsOfTheGiveaway.SaveAsync();
                    await DataManager.DidRoleGiveawayBegin.SaveAsync();

                    await ConstInfo.GroupGuild.GetTextChannel((ulong)ChannelIds.чат_флудилка).SendMessageAsync("", false, embedBuilder2.Build());

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                States.RainbowRoleGiveawayIsRunning = false;
                if (!DataManager.DebugTriger[3]) CyclicalMethodsManager.RunRainbowRoleGiveaway();
            }
            States.RainbowRoleGiveawayIsRunning = false;
        }

        public static void Stop()
        {
            flag = false;           
        }

        public static async Task<int> GetRandomNumber(int min, int max)
        {
            int counter = 0;
            int randomNum = 0;
            while (counter < 5)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://" + $"www.random.org/integers/?num=1&min={min}&max={max}&col=1&base=10&format=plain&rnd=new");
                HttpWebResponse resp = (HttpWebResponse)await request.GetResponseAsync();
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    if (int.TryParse(sr.ReadToEnd().Trim(), out randomNum))
                    {
                        counter++;
                        continue;
                    }
                }
            }
            if (counter > 4) randomNum = random.Next(min, max + 1);
            return randomNum;
        }
    }
}

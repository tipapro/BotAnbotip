using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data;
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
    class VIPRoleGiveaway
    {
        private static Random random = new Random();
        public static string RandomOrgURL = "";
        private static CancellationTokenSource cts;
        public static async void Run()
        {            
            try
            {
                if ((States.VIPRoleGiveawayIsRunning) || (cts != null)) return;
                States.VIPRoleGiveawayIsRunning = true;
                cts = new CancellationTokenSource();

                while (States.VIPRoleGiveawayIsRunning)
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
                        var sendedMessage = await ConstInfo.MainGroupGuild.GetTextChannel((ulong)ChannelIds.чат_флудилка).SendMessageAsync("", false, embedBuilder1.Build());
                        await sendedMessage.AddReactionAsync(new Emoji("💙"));

                        DataManager.DidRoleGiveawayBegin.Value = true;
                        DataManager.ParticipantsOfTheGiveaway.Value.Add(GiveawayType.VIP, new List<ulong>());
                        await DataManager.DidRoleGiveawayBegin.SaveAsync();
                        await DataManager.ParticipantsOfTheGiveaway.SaveAsync();                       
                    }

                    while (DateTime.Now.DayOfWeek != DayOfWeek.Monday && !DataManager.DebugTriger[1]) await Task.Delay(new TimeSpan(0, 0, 10, 0));
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
                        await ConstInfo.MainGroupGuild.GetUser(winner).AddRoleAsync(ConstInfo.MainGroupGuild.GetRole((ulong)RoleIds.VIP));
                        if (DataManager.LastWinner.Value.ContainsKey(GiveawayType.VIP) && DataManager.LastWinner.Value[GiveawayType.VIP] != 0)
                            await ConstInfo.MainGroupGuild.GetUser(DataManager.LastWinner.Value[GiveawayType.VIP])
                             .RemoveRoleAsync(ConstInfo.MainGroupGuild.GetRole((ulong)RoleIds.VIP));
                        DataManager.LastWinner.Value[GiveawayType.VIP] = winner;
                        await DataManager.LastWinner.SaveAsync();

                        embedBuilder2
                            .WithTitle(":gift:Итоги еженедельного розыгрыша VIP роли:gift: ")
                            .WithDescription("Победитель этой недели: <@!" + winner + ">.")
                            .WithColor(Color.Blue);
                    }
                    DataManager.ParticipantsOfTheGiveaway.Value.Remove(GiveawayType.VIP);
                    DataManager.DidRoleGiveawayBegin.Value = false;
                    await DataManager.ParticipantsOfTheGiveaway.SaveAsync();
                    await DataManager.DidRoleGiveawayBegin.SaveAsync();

                    await ConstInfo.MainGroupGuild.GetTextChannel((ulong)ChannelIds.чат_флудилка).SendMessageAsync("", false, embedBuilder2.Build());
                }
                cts = null;
                States.VIPRoleGiveawayIsRunning = false;
            }
            catch (OperationCanceledException ex)
            {
                bool isDeliberately;
                if (cts != null)
                {
                    if (ex.CancellationToken == cts.Token) isDeliberately = true;
                    else isDeliberately = false;
                }
                else isDeliberately = false;
                cts = null;
                States.VIPRoleGiveawayIsRunning = false;
                
                if (isDeliberately)
                {
                    new ExceptionLogger().Log(ex, "Авторозыгрыш отменён");
                }
                else
                {
                    new ExceptionLogger().Log(ex, "Ошибка авторозыгрыша VIP роли");
                    CyclicalMethodsManager.RunVIPGiveaway();
                }
            }
            catch (Exception ex)
            {
                cts = null;
                States.VIPRoleGiveawayIsRunning = false;
                new ExceptionLogger().Log(ex, "Ошибка авторозыгрыша VIP роли");                
                CyclicalMethodsManager.RunVIPGiveaway();
            }
        }

        public static void Stop()
        {
            if (cts != null) cts.Cancel();
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
                        if (!int.TryParse(sr.ReadToEnd().Trim(), out randomNum))
                        {
                            counter++;
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

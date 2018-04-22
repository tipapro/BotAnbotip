using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.CustomEnums;
using BotAnbotip.Bot.Data.Group;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.CyclicActions
{
    class RainbowRoleGiveaway
    {
        private static bool flag;
        private static Random random = new Random();

        public static async void Run()
        {
            flag = true;
            States.RainbowRoleGiveawayIsRunning = true;

            try
            {
                while (flag)
                {
                    if (!DataManager.DidRoleGiveawayBegin)
                    {
                        await Task.Delay(new TimeSpan(0, 0, 0, 1));
                        if (DateTime.Now.DayOfWeek != DayOfWeek.Friday && !DataManager.DebugTriger[0]) continue;

                        var embedBuilder1 = new EmbedBuilder()
                            .WithTitle(":gift:Еженедельный розыгрыш VIP роли:gift:")
                            .WithDescription("Правила розыгрыша таковы:\n" +
                            "```1) Поставьте лайк этому посту;\n" +
                            "2) Ждать понедельника.\n```" +
                            "В понедельник бот выберет случайного лайкнувшего этот пост пользователя и выдаст ему VIP роль на неделю.")
                            .WithColor(Color.Blue);
                        var sendedMessage = await ConstInfo.GroupGuild.GetTextChannel((ulong)ChannelIds.чат_флудилка).SendMessageAsync("", false, embedBuilder1.Build());
                        await sendedMessage.AddReactionAsync(new Emoji("💙"));

                        DataManager.DidRoleGiveawayBegin = true;
                        DataManager.ParticipantsOfTheGiveaway.Add(GiveawayType.VIP, new List<ulong>());
                        await DataManager.SaveDataAsync(DataManager.ParticipantsOfTheGiveaway, nameof(DataManager.ParticipantsOfTheGiveaway));

                        while (DateTime.Now.DayOfWeek != DayOfWeek.Monday && !DataManager.DebugTriger[1]) await Task.Delay(new TimeSpan(0, 0, 0, 1));
                    }

                    var embedBuilder2 = new EmbedBuilder();
                    if (DataManager.ParticipantsOfTheGiveaway[GiveawayType.VIP].Count == 0)
                    {
                        embedBuilder2
                            .WithTitle(":gift:Итоги еженедельного розыгрыша VIP роли:gift:")
                            .WithDescription("Розыгрыш не состоялся вследствие нехватки участников.")
                            .WithColor(Color.Blue);
                    }
                    else
                    {
                        ulong winner = DataManager.ParticipantsOfTheGiveaway[GiveawayType.VIP][random.Next(DataManager.ParticipantsOfTheGiveaway.Count - 1)];
                        await ConstInfo.GroupGuild.GetUser(winner).AddRoleAsync(ConstInfo.GroupGuild.GetRole((ulong)RoleIds.VIP));
                        if (DataManager.LastWinner[GiveawayType.VIP] != 0) await ConstInfo.GroupGuild.GetUser(DataManager.LastWinner[GiveawayType.VIP])
                             .RemoveRoleAsync(ConstInfo.GroupGuild.GetRole((ulong)RoleIds.VIP));
                        DataManager.LastWinner[GiveawayType.VIP] = winner;

                        embedBuilder2
                            .WithTitle(":gift:Итоги еженедельного розыгрыша VIP роли:gift: ")
                            .WithDescription("Победитель этой недели: <@!" + winner + ">, ")
                            .WithColor(Color.Blue);
                    }
 
                    DataManager.ParticipantsOfTheGiveaway.Remove(GiveawayType.VIP);
                    await DataManager.SaveDataAsync(DataManager.ParticipantsOfTheGiveaway, nameof(DataManager.ParticipantsOfTheGiveaway));
                    await ConstInfo.GroupGuild.GetTextChannel((ulong)ChannelIds.чат_флудилка).SendMessageAsync("", false, embedBuilder2.Build());
                    DataManager.DidRoleGiveawayBegin = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                States.RainbowRoleGiveawayIsRunning = false;
                CyclicalMethodsManager.RunRainbowRoleGiveaway();
            }
            States.RainbowRoleGiveawayIsRunning = false;
        }

        public static void Stop()
        {
            flag = false;           
        }
    }
}

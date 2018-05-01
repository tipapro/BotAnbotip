using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.Group;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.CyclicActions
{
    class WantPlayAutoRemoving
    {
        private static bool flag;

        public static async void Run()
        {
            flag = true;
            States.WantPlayAutoRemovingIsRunning = true;

            try
            {
                while (flag)
                {

                    await Task.Delay(new TimeSpan(0, 5, 0));
                    foreach (var pair in DataManager.AgreeingToPlayUsers.Value)
                    {
                        if ((DateTime.Now - pair.Value.Item1.DateTime).Duration() > new TimeSpan(1, 0, 0, 0))
                        {
                            var message = await ((IMessageChannel)ConstInfo.GroupGuild.GetChannel((ulong)ChannelIds.чат_игровой)).GetMessageAsync(pair.Key);
                            await message.DeleteAsync();
                            DataManager.AgreeingToPlayUsers.Value.Remove(pair.Key);
                            await DataManager.AgreeingToPlayUsers.SaveAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                States.WantPlayAutoRemovingIsRunning = false;
                if (!DataManager.DebugTriger[4]) CyclicalMethodsManager.RunWantPlayAutoRemoving();
            }
            States.WantPlayAutoRemovingIsRunning = false;
        }

        public static void Stop()
        {
            flag = false;
        }
    }
}

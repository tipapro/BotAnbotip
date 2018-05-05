using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.Group;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.CyclicActions
{
    class WantPlayAutoRemoving
    {
        private static CancellationTokenSource cts;

        public static async void Run()
        {       
            try
            {
                States.WantPlayAutoRemovingIsRunning = true;
                cts = new CancellationTokenSource();

                while (States.WantPlayAutoRemovingIsRunning)
                {

                    await Task.Delay(new TimeSpan(0, 5, 0));
                    foreach (var pair in DataManager.AgreeingToPlayUsers.Value)
                    {
                        if ((DateTime.Now - pair.Value.Item1.DateTime).Duration() > new TimeSpan(1, 0, 0, 0))
                        {
                            var message = await ((IMessageChannel)ConstInfo.MainGroupGuild.GetChannel((ulong)ChannelIds.чат_игровой)).GetMessageAsync(pair.Key);
                            await message.DeleteAsync();
                            DataManager.AgreeingToPlayUsers.Value.Remove(pair.Key);
                            await DataManager.AgreeingToPlayUsers.SaveAsync();
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                cts = null;
                States.WantPlayAutoRemovingIsRunning = false;
                Console.WriteLine("Автоудаление приглашений в игру отменено.");
            }
            catch (Exception ex)
            {                
                cts = null;
                States.WantPlayAutoRemovingIsRunning = false;
                new ExceptionLogger().Log(ex, "Ошибка автоудаления приглашений");
                if (!DataManager.DebugTriger[4]) CyclicalMethodsManager.RunWantPlayAutoRemoving();
            }
            States.WantPlayAutoRemovingIsRunning = false;
        }

        public static void Stop()
        {
            if (cts != null) cts.Cancel();
        }
    }
}

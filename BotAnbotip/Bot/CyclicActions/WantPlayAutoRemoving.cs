using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.Group;
using Discord;
using Discord.WebSocket;
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
                if ((States.WantPlayAutoRemovingIsRunning) || (cts != null)) return;
                States.WantPlayAutoRemovingIsRunning = true;
                cts = new CancellationTokenSource();

                while (States.WantPlayAutoRemovingIsRunning)
                {
                    await Task.Delay(new TimeSpan(0, 5, 0));
                    List<ulong> toDelete = new List<ulong>();
                    foreach (var pair in DataManager.AgreeingToPlayUsers.Value)
                    {
                        if ((DateTime.Now - pair.Value.Item1.DateTime).Duration() > new TimeSpan(1, 0, 0, 0))
                        {
                            var message = await ((IMessageChannel)ConstInfo.MainGroupGuild.GetChannel((ulong)ChannelIds.чат_игровой)).GetMessageAsync(pair.Key);
                            if (message != null) await message.DeleteAsync();
                            toDelete.Add(pair.Key);
                        }
                    }
                    foreach (var id in toDelete)
                    {
                        if (DataManager.AgreeingToPlayUsers.Value.ContainsKey(id))
                        {
                            DataManager.AgreeingToPlayUsers.Value.Remove(id);
                            await DataManager.AgreeingToPlayUsers.SaveAsync();
                        }
                    }
                }
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
                States.WantPlayAutoRemovingIsRunning = false;
                new ExceptionLogger().Log(ex, "Автоудаление приглашений в игру отменено");
                if (!isDeliberately) CyclicalMethodsManager.RunWantPlayAutoRemoving();
            }
            catch (Exception ex)
            {                
                cts = null;
                States.WantPlayAutoRemovingIsRunning = false;
                new ExceptionLogger().Log(ex, "Ошибка автоудаления приглашений");
                CyclicalMethodsManager.RunWantPlayAutoRemoving();
            }
            States.WantPlayAutoRemovingIsRunning = false;
        }

        public static void Stop()
        {
            if (cts != null) cts.Cancel();
        }
    }
}

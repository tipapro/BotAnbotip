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
    class HackerChannelAutoChange
    {        
        private const int Length = 10;
        private const int DelayTime = 25;

        private static Random random = new Random();
        private static CancellationTokenSource cts;

        public static async void Run()
        {
            try
            {
                if ((States.HackerChannelAutoChangeIsRunning) || (cts != null)) return;
                States.HackerChannelAutoChangeIsRunning = true;

                while (!AuxiliaryBotClient.BotLoaded) await Task.Delay(1000);

                cts = new CancellationTokenSource();
                await ((ITextChannel)ConstInfo.AuxiliaryGroupGuild.GetChannel((ulong)ChannelIds.test)).SendMessageAsync("Автосмена названия запущена " + DateTime.Now);
                while (States.HackerChannelAutoChangeIsRunning)
                {
                    await Task.Delay(DelayTime);
                    await ConstInfo.AuxiliaryGroupGuild.GetChannel(DataManager.HackerChannelId.Value).ModifyAsync((channelProperties) =>
                    {
                        channelProperties.Name = GetRandomString(Length);
                    });
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
                States.HackerChannelAutoChangeIsRunning = false;
                
                if (isDeliberately)
                {
                    new ExceptionLogger().Log(ex, "Автосмена названия отменена");
                }
                else
                {
                    new ExceptionLogger().Log(ex, "Ошибка при автосмене названия");
                    CyclicalMethodsManager.RunHackerChannelAutoChange();
                }
            }
            catch (Exception ex)
            {               
                cts = null;
                States.HackerChannelAutoChangeIsRunning = false;
                new ExceptionLogger().Log(ex, "Ошибка при автосмене названия");                
                CyclicalMethodsManager.RunHackerChannelAutoChange();
            }
            cts = null;
            States.HackerChannelAutoChangeIsRunning = false;
        }

        public static void Stop()
        {
            if (cts != null) cts.Cancel();
        }

        public static string GetRandomString(int lenght)
        {
            string resultString = "";
            for (int i = 0; i < lenght; i++)
            {
                resultString += GetRandomChar();
            }
            return resultString;
        }

        public static string GetRandomChar()
        {
            return char.ConvertFromUtf32(random.Next(400));
        }
    }
}

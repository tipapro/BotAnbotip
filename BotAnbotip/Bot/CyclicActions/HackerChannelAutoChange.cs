﻿using BotAnbotip.Bot.Clients;
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
                if ((DataManager.HackerChannelIsRunning.Value)|| (cts != null)) return;
                DataManager.HackerChannelIsRunning.Value = true;
                await DataManager.HackerChannelIsRunning.SaveAsync();

                while (!AuxiliaryBotClient.BotLoaded) await Task.Delay(1000);

                cts = new CancellationTokenSource();
                await ((ITextChannel)ConstInfo.AuxiliaryGroupGuild.GetChannel((ulong)ChannelIds.test)).SendMessageAsync("Автосмена названия запущена " + DateTime.Now);
                while (DataManager.HackerChannelIsRunning.Value)
                {
                    await Task.Delay(DelayTime);
                    await ConstInfo.AuxiliaryGroupGuild.GetChannel(DataManager.HackerChannelId.Value).ModifyAsync((channelProperties) =>
                    {
                        channelProperties.Name = GetRandomString(Length);
                    });
                }
                DataManager.HackerChannelIsRunning.Value = false;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Автосмена названия отменена.");
            }
            catch (Exception ex)
            {
                new ExceptionLogger().Log(ex, "Ошибка при автосмене названия");
                DataManager.HackerChannelIsRunning.Value = false;
                CyclicalMethodsManager.RunHackerChannelAutoChange();
            }
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
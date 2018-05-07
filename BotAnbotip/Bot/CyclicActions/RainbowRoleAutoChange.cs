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
    class RainbowRoleAutoChange
    {
        private const double speed = 19;
        private const double segment = 100;
        private const double period = 6 * segment;
        private const int DelayTime = 15;

        public static double globalX;
        private static CancellationTokenSource cts;

        public static async void Run()
        {
            try
            {
                if ((DataManager.RainbowRoleIsRunning.Value) || (cts != null)) return;
                DataManager.RainbowRoleIsRunning.Value = true;
                await DataManager.RainbowRoleIsRunning.SaveAsync();

                while (!AuxiliaryBotClient.BotLoaded) await Task.Delay(1000);

                globalX = 0;

                cts = new CancellationTokenSource();
                await ((ITextChannel)ConstInfo.AuxiliaryGroupGuild.GetChannel((ulong)ChannelIds.test)).SendMessageAsync("Автосмена цвета запущена " + DateTime.Now);               
                while (DataManager.RainbowRoleIsRunning.Value)
                {
                    await Task.Delay(DelayTime);
                    await ConstInfo.AuxiliaryGroupGuild.GetRole(DataManager.RainbowRoleId.Value).ModifyAsync((roleProperties) =>
                    {
                        roleProperties.Color = GetNextColor();
                    });
                }
            }
            catch (OperationCanceledException ex)
            {
                //
                DataManager.RainbowRoleIsRunning.Value = false;
                cts = null;
                new ExceptionLogger().Log(ex, "Автосмена цвета отменена");
            }
            catch (Exception ex)
            {               
                DataManager.RainbowRoleIsRunning.Value = false;
                cts = null;
                new ExceptionLogger().Log(ex, "Ошибка при автосмене роли");
                CyclicalMethodsManager.RunRainbowRoleAutoChange();
            }
        }

        public static void Stop()
        {
            if (cts != null) cts.Cancel();
        }

        private static Color GetNextColor()
        {
            var color = new Color(GetColor(RainbowColor.Red), GetColor(RainbowColor.Green), GetColor(RainbowColor.Blue));
            globalX += speed;
            if (globalX >= period) globalX -= period;
            return color;
        }
       
        public static float GetColor(RainbowColor color)
        {
            var colorOffset = segment * (int)color;
            var x = globalX + colorOffset;
            while (x >= period) x -= period;
            while (x < 0) x += period;
            return F(x);
        }

        public static float F(double x)
        {
            if ((x >= 0) && (x <= segment)) return (float)(x / segment);
            if ((x > segment) && (x < 3 * segment)) return 1;
            if ((x >= 3 * segment) && (x <= 4 * segment)) return (float)(4 - x / segment);
            if ((x > 4 * segment) && (x < 6 * segment)) return 0;

            Console.WriteLine("Сбой цвета");
            return 0;
        }
    }

    public enum RainbowColor
    {
        Red = 2,
        Green = 0,
        Blue = 4
    }
}

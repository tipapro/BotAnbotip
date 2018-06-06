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
    class RainbowRoleAutoChangeCyclicAction : CyclicActionBase
    {
        private const double speed = 19;
        private const double segment = 100;
        private const double period = 6 * segment;
        private const int DelayTime = 15;

        public static double globalX;

        public RainbowRoleAutoChangeCyclicAction(BotClientBase botClient, string errorMessage, string startMessage, string stopMessage) :
            base(botClient, errorMessage, startMessage, stopMessage)
        {
            _cycleMethod = Cycle;
        }

        private async Task Cycle(CancellationToken token)
        {
            globalX = 0;
            while (IsStarted)
            {
                await Task.Delay(DelayTime, token);
                await BotClientManager.AuxiliaryBot.Guild.GetRole(DataManager.RainbowRoleId.Value).ModifyAsync((roleProperties) =>
                {
                    roleProperties.Color = GetNextColor();
                });
            }
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

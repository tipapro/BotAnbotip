using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using Discord;
using Discord.WebSocket;
using BotAnbotip.Bot.Data.Group;
using BotAnbotip.Bot.Client;

namespace BotAnbotip.Bot.Commands
{
    class RainbowRoleCommands
    {
        private const byte ColorChangingSpeed = 40;
        private const int DelayTime = 25;
        private static bool flag;
        public static byte red, green, blue;
        public static bool redFlag, greenFlag, blueFlag;

        public static async Task ChangeStateOfTheRainbowRoleAsync(string argument, SocketMessage message = null)
        {
            if (message != null)
            {
                await message.DeleteAsync();
                if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;
            }

            var str = argument.Split(' ');
            if (str.Length > 1)
            {
                DataManager.RainbowRoleId = ulong.Parse(str[1]);
                argument = str[0];
                await DataManager.SaveDataAsync(DataManager.RainbowRoleId, nameof(DataManager.RainbowRoleId));
            }

            if ((argument == "вкл") && (!DataManager.RainbowRoleIsRunning))
            {
                DataManager.RainbowRoleIsRunning = true;
                await DataManager.SaveDataAsync(DataManager.RainbowRoleIsRunning, nameof(DataManager.RainbowRoleIsRunning));
                Task.Run(() => RunRainbowRoleAsync()).GetAwaiter().GetResult();
            }
            else if (argument == "выкл")
            {
                flag = false;
                DataManager.RainbowRoleIsRunning = false;
                await DataManager.SaveDataAsync(DataManager.RainbowRoleIsRunning, nameof(DataManager.RainbowRoleIsRunning));
            }
        }

        private static async void RunRainbowRoleAsync()
        {
            try
            {
                if (!BotClient.BotLoaded)
                {
                    return;
                }

                SetDefault();
                flag = true;


                while (flag)
                {
                    await Task.Delay(DelayTime);
                    await ConstInfo.GroupGuild.GetRole(DataManager.RainbowRoleId).ModifyAsync((roleProperties) =>
                    {
                        roleProperties.Color = GetNextColor(); ;
                    });
                }
                DataManager.RainbowRoleIsRunning = false;
            }
            catch (Exception ex)
            {    
                Console.WriteLine(ex.Message);
                if(ex.InnerException != null) Console.WriteLine(ex.InnerException.Message);
                Task.Run(() => RunRainbowRoleAsync()).GetAwaiter().GetResult();
            }
}

        private static Color GetNextColor()
        {
            if ((green <= byte.MaxValue - ColorChangingSpeed) && !greenFlag)
            {
                green += ColorChangingSpeed;
                return new Color(red, green, blue);
            }

            if ((red >= ColorChangingSpeed) && redFlag)
            {
                red -= ColorChangingSpeed;
                return new Color(red, green, blue);
            }

            if ((blue <= byte.MaxValue - ColorChangingSpeed) && !blueFlag)
            {
                blue += ColorChangingSpeed;
                return new Color(red, green, blue);
            }

            if (green >= ColorChangingSpeed)
            {
                greenFlag = true;
                green -= ColorChangingSpeed;
                return new Color(red, green, blue);
            }

            if ((red <= byte.MaxValue - ColorChangingSpeed))
            {
                redFlag = false;
                red += ColorChangingSpeed;
                return new Color(red, green, blue);
            }

            if (blue >= ColorChangingSpeed)
            {
                blueFlag = true;
                blue -= ColorChangingSpeed;
                return new Color(red, green, blue);
            }
            else
            {
                SetDefault();
                return new Color(red, green, blue);
            }
        }

        public static void SetDefault()
        {
            redFlag = true;
            greenFlag = false;
            blueFlag = false;
            red = 255;
            green = 0;
            blue = 0;
        }
    }
}

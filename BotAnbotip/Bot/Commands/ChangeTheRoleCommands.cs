using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using Discord;
using Discord.WebSocket;

namespace BotAnbotip.Bot.Commands
{
    class ChangeTheRoleCommands
    {
        private const byte ColorChangingSpeed = 50;
        private static bool flag;

        public static async Task SetTheRoleColorAutoChangingAsync(string argument, SocketMessage message = null)
        {
            if (message != null) await message.DeleteAsync();

            var str = argument.Split(' ');
            if (str.Length > 1)
            {
                DataManager.RoleColorAutoChangingId = ulong.Parse(str[1]);
                argument = str[0];
            }

            if ((argument == "вкл") && (!DataManager.RoleColorAutoChangingIsSwitchedOn))
            {
                DataManager.RoleColorAutoChangingIsSwitchedOn = true;
                await DataManager.SaveDataAsync();
                Task.Run(() => LaunchRoleColorAutoChangingAsync()).GetAwaiter().GetResult();
            }
            else if (argument == "выкл") flag = false;
        }

        private static async void LaunchRoleColorAutoChangingAsync()
        {
            flag = true;
            byte red = 255, green = 0, blue = 0;
            bool redFlag = true, greenFlag = false, blueFlag = false;

            while (flag)
            {
                if (!Info.BotLoaded) continue;

                await Info.GroupGuild.GetRole(DataManager.RoleColorAutoChangingId).ModifyAsync((roleProperties) =>
                {
                    roleProperties.Color = GetNextColor(ref red, ref green, ref blue, ref redFlag, ref greenFlag, ref blueFlag); ;
                });
            }
            DataManager.RoleColorAutoChangingIsSwitchedOn = false;
        }

        private static Color GetNextColor(ref byte red, ref byte green, ref byte blue, ref bool redFlag, ref bool greenFlag, ref bool blueFlag)
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
                redFlag = true;
                greenFlag = false;
                blueFlag = false;
                red = 255;
                green = 0;
                blue = 0;
                return new Color(red, green, blue);
            }
        }
    }
}

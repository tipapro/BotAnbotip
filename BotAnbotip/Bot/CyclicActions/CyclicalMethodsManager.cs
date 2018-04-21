using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.Group;
using Discord;

namespace BotAnbotip.Bot.CyclicActions
{
    class CyclicalMethodsManager
    {       
        public static void RunAll()
        {
            RunWantPlayAutoRemoving();
            RunRainbowRoleGiveaway();
        }

        public static void RunWantPlayAutoRemoving() => Task.Run(() => WantPlayAutoRemoving.Run()).GetAwaiter().GetResult();
        
        public static void RunRainbowRoleGiveaway() => Task.Run(() => RainbowRoleGiveaway.Run()).GetAwaiter().GetResult();
    }
}

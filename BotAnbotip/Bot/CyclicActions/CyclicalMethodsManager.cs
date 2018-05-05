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
            RunVIPGiveaway();
        }

        public static async Task TurnOffAuxiliary()
        {
            HackerChannelAutoChange.Stop();
            DataManager.HackerChannelIsRunning.Value = false;
            await DataManager.HackerChannelIsRunning.SaveAsync();

            RainbowRoleAutoChange.Stop();
            DataManager.RainbowRoleIsRunning.Value = false;
            await DataManager.RainbowRoleIsRunning.SaveAsync();
        }

        public static Task TurnOffMain()
        {
            VIPRoleGiveaway.Stop();
            States.RainbowRoleGiveawayIsRunning = false;

            WantPlayAutoRemoving.Stop();
            States.WantPlayAutoRemovingIsRunning = false;

            return Task.CompletedTask;
        }

        public static void RunWantPlayAutoRemoving() => Task.Run(() => WantPlayAutoRemoving.Run()).GetAwaiter().GetResult();
        
        public static void RunVIPGiveaway() => Task.Run(() => VIPRoleGiveaway.Run()).GetAwaiter().GetResult();

        public static void RunRainbowRoleAutoChange() => Task.Run(() => RainbowRoleAutoChange.Run()).GetAwaiter().GetResult();

        public static void RunHackerChannelAutoChange() => Task.Run(() => HackerChannelAutoChange.Run()).GetAwaiter().GetResult();
    }
}

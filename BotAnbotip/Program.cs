using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data;
using System.Threading.Tasks;

namespace BotAnbotip
{    public class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();
        public static async Task MainAsync()
        {
            PrivateData.Read();
            await DataManager.ReadAllDataAsync();

            await BotClientManager.MainBot.PrepareAsync();
            await BotClientManager.AuxiliaryBot.PrepareAsync();

            bool mainLaunchResult = false;
            while (!mainLaunchResult) mainLaunchResult = await BotClientManager.MainBot.Launch();

            //bool auxiliaryLaunchResult = false;
            //while (!auxiliaryLaunchResult) auxiliaryLaunchResult = await BotClientManager.AuxiliaryBot.Launch();

            await Task.Delay(-1);
        }
    }
}

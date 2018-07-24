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
            foreach (var pair in DataManager.RatingChannels.Value)
                pair.Value.ListOfObjects.Test();
            await DataManager.RatingChannels.SaveAsync();

            await BotClientManager.MainBot.PrepareAsync();

            bool mainLaunchResult = false;
            while (!mainLaunchResult) mainLaunchResult = await BotClientManager.MainBot.Launch();

            await Task.Delay(-1);
        }
    }
}

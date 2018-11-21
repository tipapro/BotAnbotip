using BotAnbotip.Bot.Data.CustomEnums;
using Microsoft.Extensions.Logging;

namespace BotAnbotip.Bot.Clients
{
    class BotClientManager
    {
        public static MainBotClient MainBot { get; private set; }

        public static void Prepare(ILoggerFactory loggerFactory)
        {
            MainBot = new MainBotClient(loggerFactory);
        }
    }
}

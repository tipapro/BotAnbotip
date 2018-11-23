using BotAnbotip.Data.CustomEnums;
using Microsoft.Extensions.Logging;

namespace BotAnbotip.Clients
{
    class ClientControlManager
    {
        public static MainClient MainBot { get; private set; }

        public static void Prepare(ILoggerFactory loggerFactory)
        {
            MainBot = new MainClient(loggerFactory);
        }
    }
}

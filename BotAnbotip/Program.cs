using System;

namespace BotAnbotip
{    public class Program
    {
        public static void Main(string[] args)
        {
            new Bot.Clients.MainBotClient().MainAsync().GetAwaiter().GetResult();
            
        }

    }
}

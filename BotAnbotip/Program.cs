﻿using System;

namespace BotAnbotip
{    public class Program
    {
        public static void Main(string[] args)
        {
            new Bot.Client.Client().MainAsync().GetAwaiter().GetResult();
        }

    }
}

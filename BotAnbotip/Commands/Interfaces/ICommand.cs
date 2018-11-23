using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotAnbotip.Commands.Interfaces
{
    interface ICommand
    {
        void Execute(SocketMessage msg, string arg);
    }
}

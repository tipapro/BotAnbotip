using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Commands;
using Discord;
using BotAnbotip.Data;
using BotAnbotip.Data.CustomEnums;
using BotAnbotip.Clients;
using BotAnbotip.OtherModules;
using BotAnbotip.Data.CustomClasses;

namespace BotAnbotip.Handlers
{
    class MessageHandler
    {
        private readonly ulong _botId;
        private readonly char _prefix;

        private CommandControlManager _cmdManager;

        public MessageHandler(ulong botId, char prefix)
        {
            _botId = botId;
            _prefix = prefix;
            _cmdManager = new CommandControlManager(botId);
        }


        public async void ProcessTheMessage(SocketMessage message)
        {
            try
            {
                await Task.Run(async () =>
                {
                    if (!(message is SocketUserMessage) || (message.Content == "") || (message.Content == null) || (message.Content.ToCharArray()[0] != _prefix)) return;
                    string[] buf = message.Content.Substring(1).Split(' ');
                    string command = buf[0];
                    string argument = "";
                    if (buf.Length > 1) argument = message.Content.Substring((_prefix + command + " ").ToCharArray().Length);
                    await _cmdManager.RunCommand(command.ToLower(), argument, message);
                });
            }
            catch (Exception ex)
            {
                new ExceptionLogger().Log(ex, "Ошибка при обработке отправленного сообщения");
            }
        }

        public async void AddMessagePoints(SocketMessage message)
        {
            await Task.Run(async () =>
            {
                if (!DataControlManager.UserProfiles.Value.ContainsKey(message.Author.Id))
                    DataControlManager.UserProfiles.Value.Add(message.Author.Id, new UserProfile(message.Author.Id));
                await DataControlManager.UserProfiles.Value[message.Author.Id].AddPoints((long)ActionsCost.Message);
                await DataControlManager.UserProfiles.SaveAsync();
            });
        }
    }
}

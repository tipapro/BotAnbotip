using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Commands;
using Discord;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.CustomEnums;
using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.OtherModules;
using BotAnbotip.Bot.Data.CustomClasses;

namespace BotAnbotip.Bot.Handlers
{
    class MessageHandler
    {
        private ulong botId;
        private char prefix;

        private CommandManager _cmdManager;

        public MessageHandler(ulong botId, char prefix)
        {
            this.botId = botId;
            this.prefix = prefix;
            _cmdManager = new CommandManager(botId);
        }


        public async void ProcessTheMessage(SocketMessage message)
        {
            try
            {
                await Task.Run(async () =>
                {
                    if (!(message is SocketUserMessage) || (message.Content == "") || (message.Content == null) || (message.Content.ToCharArray()[0] != prefix)) return;
                    string[] buf = message.Content.Substring(1).Split(' ');
                    string command = buf[0];
                    string argument = "";
                    if (buf.Length > 1) argument = message.Content.Substring((prefix + command + " ").ToCharArray().Length);
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
                if (!DataManager.UserProfiles.Value.ContainsKey(message.Author.Id))
                    DataManager.UserProfiles.Value.Add(message.Author.Id, new UserProfile(message.Author.Id));
                await DataManager.UserProfiles.Value[message.Author.Id].AddPoints((int)ActionsCost.Message);
                await DataManager.UserProfiles.SaveAsync();
            });
        }
    }
}

using System;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using Discord.WebSocket;
using BotAnbotip.Bot.Data.Group;
using BotAnbotip.Bot.Clients;
using Discord;
using BotAnbotip.Bot.Commands;
using BotAnbotip.Bot.OtherModules;
using BotAnbotip.Bot.Services;

namespace BotAnbotip.Bot.Commands
{
    class HackerChannelCommands : CommandsBase
    {
        public HackerChannelCommands() : base
            (
            (TransformMessageToChangeStateAsync,
            new string[] { "хакерканал", "hackerchannel", "hackerch" })
            ){ }

        private static async Task TransformMessageToChangeStateAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

            var strArray = argument.Split(' ');

            ulong roleId = 0;
            if (strArray.Length > 1)
            {
                argument = strArray[0];
                roleId = ulong.Parse(strArray[1]);
            }

            bool changedState = false;
            switch (argument)
            {
                case "вкл":
                case "+":
                case "on": changedState = true; break;
                case "выкл":
                case "-":
                case "off": changedState = false; break;
                default: throw new ArgumentException("Неопознанный аргумент", "changedState");
            }
            await CommandManager.HackerChannel.ChangeStateAsync(changedState, roleId);
        }

        public async Task ChangeStateAsync(bool changedState, ulong roleId = 0)
        {
            if (roleId != 0) await DataManager.HackerChannelId.SaveAsync(roleId);

            if (changedState)
            {
                //ServiceManager.HackerChannelAutoChange.Run();
                await DataManager.HackerChannelIsRunning.SaveAsync(true);
            }
            else 
            {
                //ServiceManager.HackerChannelAutoChange.Stop();
                await DataManager.HackerChannelIsRunning.SaveAsync(false);
            }
        }
    }
}

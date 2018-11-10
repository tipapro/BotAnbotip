using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.Group;
using BotAnbotip.Bot.Clients;

namespace BotAnbotip.Bot.Commands
{
    class BotControlCommands : CommandsBase
    {
        public BotControlCommands() : base
            (
            (TransformMessageToStopAsync,
            new string[] { "стоп", "stop"}),
            (TransformMessageToClearDataAsync,
            new string[] { "удалиданные", "сотриданные", "cleardata", "formatdata" })
            ){ }
        private static async Task TransformMessageToStopAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Founder)) return;
            await CommandManager.BotControl.StopAsync();
        }

        private static async Task TransformMessageToClearDataAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Founder)) return;
            await CommandManager.BotControl.ClearDataAsync();
        }

        public async Task StopAsync()
        {
            await DataManager.SaveAllDataAsync();
            await BotClientManager.MainBot.Client.StopAsync();
            Environment.Exit(0);
        }

        public async Task ClearDataAsync()
        {
            DataManager.InitializeAllVariables();
            await DataManager.SaveAllDataAsync();
        }
    }
}

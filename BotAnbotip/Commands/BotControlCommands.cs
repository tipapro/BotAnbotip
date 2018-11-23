using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Data;
using BotAnbotip.Data.Group;
using BotAnbotip.Clients;

namespace BotAnbotip.Commands
{
    class BotControlCommands : CommandBase
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
            if (!CommandControlManager.CheckPermission((IGuildUser)message.Author, RoleIds.Founder)) return;
            await CommandControlManager.BotControl.StopAsync();
        }

        private static async Task TransformMessageToClearDataAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandControlManager.CheckPermission((IGuildUser)message.Author, RoleIds.Founder)) return;
            await CommandControlManager.BotControl.ClearDataAsync();
        }

        public async Task StopAsync()
        {
            await DataControlManager.SaveAllDataAsync();
            await ClientControlManager.MainBot.Client.StopAsync();
            Environment.Exit(0);
        }

        public async Task ClearDataAsync()
        {
            DataControlManager.InitializeAll();
            await DataControlManager.SaveAllDataAsync();
        }
    }
}

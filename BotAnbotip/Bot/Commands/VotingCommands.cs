﻿using BotAnbotip.Bot.Data;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data.Group;

namespace BotAnbotip.Bot.Commands
{
    class VotingCommands
    {
        static public Dictionary<int, string> Numerals = new Dictionary<int, string> {
            { 1, "1\u20E3" }, { 2, "2\u20E3" }, { 3, "3\u20E3" }, { 4, "4\u20E3" }, { 5, "5\u20E3" },
            { 6, "6\u20E3" }, { 7, "7\u20E3" }, { 8, "8\u20E3" }, { 9, "9\u20E3" }, { 10, "🔟" }, };


        public static async Task AddVotingdAsync(SocketMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Модератор)) return;

            var str = argument.Split('|');

            int numOfLines = str.Length;
            string resultStr = "**" + str[0] + "**\n";
            var embedBuilder = new EmbedBuilder()
                .WithTitle(":bar_chart:Голосование:bar_chart:")
                .WithColor(Color.Gold);

            for (int i = 1; i < numOfLines; i++)
            {
                resultStr += new Emoji(Numerals[i]) + "`" + str[i] + "`\n";
            }

           embedBuilder.WithDescription(resultStr);

            var sendedMessage = await message.Channel.SendMessageAsync("", false, embedBuilder.Build());

            for (int i = 1; i < numOfLines; i++)
            {
                await sendedMessage.AddReactionAsync(new Emoji(Numerals[i]));
            }
            

            DataManager.VotingLists.Add(sendedMessage.Id, new List<Tuple<string, int>>());
            await DataManager.SaveDataAsync(DataManager.VotingLists, nameof(DataManager.RatingChannels));
        }

        public static async Task DeleteVotingAsync(SocketMessage message, string argument)
        {
            await message.DeleteAsync();

            ulong soughtForMessage = ulong.Parse(argument);

            var userRoles = ((IGuildUser)message.Author).RoleIds;
            if (userRoles.Contains((ulong)RoleIds.Основатель))
            {
                var foundedMessage = await message.Channel.GetMessageAsync(soughtForMessage);
                await foundedMessage.DeleteAsync();
                DataManager.VotingLists.Remove(foundedMessage.Id);
                await DataManager.SaveDataAsync(DataManager.VotingLists, nameof(DataManager.RatingChannels));
            }
        }
    }
}
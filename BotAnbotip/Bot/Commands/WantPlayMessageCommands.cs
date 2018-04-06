using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using Discord;
using Discord.WebSocket;

namespace BotAnbotip.Bot.Commands
{
    class WantPlayMessageCommands
    {
        public static async Task SenAsync(string game, SocketMessage message = null, IUser user = null, string gamePictureUrl = null, string url = null)
        {
            if (game.Length > 64) return;
            string username = "";
            ulong userId = 0;
            var embedBuilder = new EmbedBuilder()                
                .WithColor(Color.DarkBlue);


            if (message != null)
            {
                await message.DeleteAsync();
                username = message.Author.Mention;
                userId = message.Author.Id;
            }
            if (user != null)
            {
                username = user.Mention;
                userId = user.Id;
            }
            if (gamePictureUrl != null) embedBuilder.WithThumbnailUrl(gamePictureUrl);
            if (url != null) embedBuilder.WithUrl(url);

            embedBuilder.WithTitle("Приглашение в игру").WithDescription("Пользователь " + username + " приглашает в игру **" + game + "**.");

            var sendedMessage = await ((ISocketMessageChannel)Info.GroupGuild.GetChannel((ulong)ChannelIds.чат_игровой)).SendMessageAsync("", false, embedBuilder.Build());

            await sendedMessage.AddReactionAsync(new Emoji("✅"));

            DataManager.agreeingToPlayUsers.Add(sendedMessage.Id, new List<ulong> { userId });
            await DataManager.SaveDataAsync();
        }

        public static async Task AddUserAcceptedAsync(IUserMessage message, IUser user)
        {
            if (!DataManager.agreeingToPlayUsers.ContainsKey(message.Id)) return;
            if (DataManager.agreeingToPlayUsers[message.Id][0] == user.Id) return;
            if (DataManager.agreeingToPlayUsers[message.Id].Contains(user.Id)) return;
            DataManager.agreeingToPlayUsers[message.Id].Add(user.Id);
            await DataManager.SaveDataAsync();
            string str = "";
            foreach(var userId in DataManager.agreeingToPlayUsers[message.Id])
            {
                if (userId == DataManager.agreeingToPlayUsers[message.Id][0]) continue;
                str += "<@!" + userId + ">, ";
            }
            if (str.Length > 2)
                {
                    str = str.Substring(0, str.Length - 2);
                    str = "\n\n" + "Приняли приглашение: " + str + ".";
                }
            await message.ModifyAsync((messageProperties) => {
                var embed = message.Embeds.First();
                messageProperties.Embed = embed.ToEmbedBuilder().WithDescription(embed.Description.Split('\n')[0] + str).Build();
            });
        }

        public static async Task RemoveUserAcceptedAsync(IUserMessage message, IUser user)
        {
            if (!DataManager.agreeingToPlayUsers.ContainsKey(message.Id)) return;
            if (DataManager.agreeingToPlayUsers[message.Id][0] == user.Id) return;
            if (!DataManager.agreeingToPlayUsers[message.Id].Contains(user.Id)) return;
            DataManager.agreeingToPlayUsers[message.Id].Remove(user.Id);
            await DataManager.SaveDataAsync();
            string str = "";
            foreach (var userId in DataManager.agreeingToPlayUsers[message.Id])
            {
                if (userId == DataManager.agreeingToPlayUsers[message.Id][0]) continue;
                str += "<@!" + userId + ">, ";
            }
            if (str.Length > 2)
            {
                str = str.Substring(0, str.Length - 2);
                str = "\n\n" + "Приняли приглашение: " + str + ".";
            }
            await message.ModifyAsync((messageProperties) => {
                var embed = message.Embeds.First();
                messageProperties.Embed = embed.ToEmbedBuilder().WithDescription(embed.Description.Split('\n')[0] + str).Build();
            });
        }
    }
}

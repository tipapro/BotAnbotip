using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using Discord;
using Discord.WebSocket;
using BotAnbotip.Bot.Data.Group;

namespace BotAnbotip.Bot.Commands
{
    class WantPlayMessageCommands
    {
        public static async Task SendAsync(string game, SocketMessage message = null, IUser user = null, string gamePictureUrl = null, string url = null)
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

            embedBuilder.WithTitle(":video_game:Приглашение в игру:video_game:").WithDescription("Пользователь " + username + " приглашает в игру **" + game + "**.");

            var sendedMessage = await ((ISocketMessageChannel)ConstInfo.GroupGuild.GetChannel((ulong)ChannelIds.чат_игровой)).SendMessageAsync("", false, embedBuilder.Build());

            await sendedMessage.AddReactionAsync(new Emoji("✅"));

            DataManager.AgreeingToPlayUsers.Value.Add(sendedMessage.Id, new Tuple<DateTimeOffset, List<ulong>>(sendedMessage.Timestamp, new List<ulong> { userId }));
            await DataManager.AgreeingToPlayUsers.SaveAsync();
        }

        public static async Task AddUserAcceptedAsync(IUserMessage message, IUser user)
        {
            if (!DataManager.AgreeingToPlayUsers.Value.ContainsKey(message.Id)) return;
            if (DataManager.AgreeingToPlayUsers.Value[message.Id].Item2[0] == user.Id) return;
            if (DataManager.AgreeingToPlayUsers.Value[message.Id].Item2.Contains(user.Id)) return;
            DataManager.AgreeingToPlayUsers.Value[message.Id].Item2.Add(user.Id);
            await DataManager.AgreeingToPlayUsers.SaveAsync();
            string str = "";
            foreach(var userId in DataManager.AgreeingToPlayUsers.Value[message.Id].Item2)
            {
                if (userId == DataManager.AgreeingToPlayUsers.Value[message.Id].Item2[0]) continue;
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
            if (!DataManager.AgreeingToPlayUsers.Value.ContainsKey(message.Id)) return;
            if (DataManager.AgreeingToPlayUsers.Value[message.Id].Item2[0] == user.Id) return;
            if (!DataManager.AgreeingToPlayUsers.Value[message.Id].Item2.Contains(user.Id)) return;
            DataManager.AgreeingToPlayUsers.Value[message.Id].Item2.Remove(user.Id);
            await DataManager.AgreeingToPlayUsers.SaveAsync();
            string str = "";
            foreach (var userId in DataManager.AgreeingToPlayUsers.Value[message.Id].Item2)
            {
                if (userId == DataManager.AgreeingToPlayUsers.Value[message.Id].Item2[0]) continue;
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

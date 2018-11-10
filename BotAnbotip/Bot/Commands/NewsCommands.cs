using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.CustomClasses;
using BotAnbotip.Bot.Data.CustomEnums;
using BotAnbotip.Bot.Data.Group;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.Commands
{
    class NewsCommands : CommandsBase
    {
        public NewsCommands() : base
            (
            (TransformMessageToSendAsync,
            new string[] { "новость", "добавьновость", "news", "addnews" })
            ){ }

        private static async Task TransformMessageToSendAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Moderator)) return;
            string  imageUrl = null, videoUrl = null;
            var argumentList = CommandManager.ClearAndGetCommandArguments(ref argument);
            foreach(var (arg, str) in argumentList)
            {
                switch (arg)
                {
                    case 'и':
                    case 'i': imageUrl = str;  break;
                    case 'в':
                    case 'v': videoUrl = str; break;                        
                }
            }
            await CommandManager.News.SendAsync(message.Author, message.Channel, argument, imageUrl, videoUrl);
        }

        public async Task SendAsync(IUser user, IMessageChannel channel, string text, string imageUrl = null, string videoUrl = null)
        { 
            var username = BotClientManager.MainBot.Guild.GetUser(user.Id).Nickname;
            if (username == null) username = user.Username;
            var embedBuilder = new EmbedBuilder()
                .WithTitle(MessageTitles.Titles[TitleType.News])
                .WithFooter(username, user.GetAvatarUrl())
                .WithTimestamp(DateTimeOffset.Now)
                .WithColor(Color.Orange);

            if (imageUrl != null)
            {
                embedBuilder.WithImageUrl(imageUrl);
            }
            if (videoUrl != null)
            {
                string videoId = "";
                foreach (string bufStr in videoUrl.Split('/'))
                    if (bufStr == "youtu.be") videoId = videoUrl.Substring(videoUrl.Length - 11);

                if (videoId == "") videoId = videoUrl.Split('=')[1].Substring(0, 11);               

                var newUrl = $"https://youtu.be/{videoId}";

                embedBuilder.WithImageUrl($"https://img.youtube.com/vi/{videoId}/maxresdefault.jpg");
                embedBuilder.AddField(new string('-', 40), newUrl);
            }

            embedBuilder.WithDescription(text);

            var sendedMessage = await channel.SendMessageAsync("", false, embedBuilder.Build());
            await sendedMessage.AddReactionAsync(new Emoji("👍"));
            await sendedMessage.AddReactionAsync(new Emoji("👎"));

            if (!user.IsBot)
            {
                if (!DataManager.UserProfiles.Value.ContainsKey(user.Id))
                    DataManager.UserProfiles.Value.Add(user.Id, new UserProfile(user.Id));
                await DataManager.UserProfiles.Value[user.Id].AddPoints((long)ActionsCost.Percents_SendedNews, true);
                await DataManager.UserProfiles.SaveAsync();
            }
        }
    }
}

using BotAnbotip.Clients;
using BotAnbotip.Data;
using BotAnbotip.Data.CustomClasses;
using BotAnbotip.Data.CustomEnums;
using BotAnbotip.Data.Group;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Commands
{
    class NewsCommands : CommandBase
    {
        public NewsCommands() : base
            (
            (TransformMessageToSendAsync,
            new string[] { "новость", "добавьновость", "news", "addnews" })
            ){ }

        private static async Task TransformMessageToSendAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandControlManager.CheckPermission((IGuildUser)message.Author, RoleIds.Moderator)) return;
            string  imageUrl = null, videoUrl = null;
            var argumentList = CommandControlManager.ClearAndGetCommandArguments(ref argument);
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
            await CommandControlManager.News.SendAsync(message.Author, message.Channel, argument, imageUrl, videoUrl);
        }

        public async Task SendAsync(IUser user, IMessageChannel channel, string text, string imageUrl = null, string videoUrl = null)
        { 
            var username = ClientControlManager.MainBot.Guild.GetUser(user.Id).Nickname;
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
                if (!DataControlManager.UserProfiles.Value.ContainsKey(user.Id))
                    DataControlManager.UserProfiles.Value.Add(user.Id, new UserProfile(user.Id));
                await DataControlManager.UserProfiles.Value[user.Id].AddPoints((long)ActionsCost.Percents_SendedNews, true);
                await DataControlManager.UserProfiles.SaveAsync();
            }
        }
    }
}

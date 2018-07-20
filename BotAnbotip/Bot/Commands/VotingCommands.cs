using BotAnbotip.Bot.Data;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data.Group;
using BotAnbotip.Bot.Data.CustomClasses;
using BotAnbotip.Bot.Data.CustomEnums;

namespace BotAnbotip.Bot.Commands
{
    class VotingCommands : CommandsBase
    {
        public VotingCommands() : base
            (
            (TransformMessageToAddVotingdAsync,
            new string[] { "голосование", "+голосование", "добавьголосование", "voting", "+voting", "addvoting" }),
            (TransformMessageToDeleteVotingAsync,
            new string[] { "-голосование", "удалиголосование", "-voting", "deletevoting" })
            ){ }

        static private Dictionary<int, string> Numerals = new Dictionary<int, string> {
            { 1, "1\u20E3" }, { 2, "2\u20E3" }, { 3, "3\u20E3" }, { 4, "4\u20E3" }, { 5, "5\u20E3" },
            { 6, "6\u20E3" }, { 7, "7\u20E3" }, { 8, "8\u20E3" }, { 9, "9\u20E3" }, { 10, "🔟" }, };


        private static async Task TransformMessageToAddVotingdAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Модератор)) return;
            List<string> subjects = new List<string>();
            string imageUrl = null;
            foreach(var (arg, str) in CommandManager.ClearAndGetCommandArguments(ref argument))
            {
                switch (arg)
                {
                    case 'и':
                    case 'i': imageUrl = str; break;
                    case 'о':
                    case 's': subjects.Add(str); break;
                }
            }
            await CommandManager.Voting.AddVotingdAsync(message.Author, message.Channel, argument, subjects, imageUrl);
        }

        private static async Task TransformMessageToDeleteVotingAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Модератор)) return;
            ulong messageId = ulong.Parse(argument);
            await CommandManager.Voting.DeleteVotingAsync(message.Channel, messageId);
        }

        public async Task AddVotingdAsync(IUser user, IMessageChannel channel, string topic, List<string> subjects, string imageUrl = null)
        {
            string resultStr = "**" + topic + "**\n";

            for (int i = 0; i < subjects.Count; i++)
                resultStr += new Emoji(Numerals[i + 1]) + "`" + subjects[i] + "`\n";

            var embedBuilder = new EmbedBuilder()
                .WithTitle(MessageTitles.Titles[TitleType.Voting])
                .WithColor(Color.Gold)
                .WithDescription(resultStr);

            if (imageUrl != null)
            {
                embedBuilder.WithImageUrl(imageUrl);
            }

            var sendedMessage = await channel.SendMessageAsync("", false, embedBuilder.Build());

            await sendedMessage.ModifyAsync((messageProperties) =>
            {
                messageProperties.Embed = embedBuilder.WithFooter(new EmbedFooterBuilder().WithText("ID Сообщения: " + sendedMessage.Id)).Build();
            });

            for (int i = 0; i < subjects.Count; i++)
                await sendedMessage.AddReactionAsync(new Emoji(Numerals[i + 1]));

            DataManager.VotingLists.Value.Add(sendedMessage.Id, new List<(string, int)>());

            await DataManager.VotingLists.SaveAsync();
            if (!user.IsBot)
            {
                if (!DataManager.UserProfiles.Value.ContainsKey(user.Id))
                    DataManager.UserProfiles.Value.Add(user.Id, new UserProfile(user.Id));
                await DataManager.UserProfiles.Value[user.Id].AddPoints((int)ActionsCost.SendedVoting);
                await DataManager.UserProfiles.SaveAsync();
            }
        }

        public async Task DeleteVotingAsync(IMessageChannel channel, ulong messageId)
        {
            var foundedMessage = await channel.GetMessageAsync(messageId);
            await foundedMessage.DeleteAsync();
            DataManager.VotingLists.Value.Remove(foundedMessage.Id);
            await DataManager.VotingLists.SaveAsync();
        }
    }
}

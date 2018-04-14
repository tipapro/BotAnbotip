using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Commands;
using Discord;
using BotAnbotip.Bot.Data;

namespace BotAnbotip.Bot.Client
{
    class MessageHandler
    {
        public async Task MessageReceived(SocketMessage message)
        {
            if (message.Author.Id == Client.BotClient.CurrentUser.Id) return;

            if (message.Content.ToCharArray()[0] == PrivateData.Prefix)
            {
                string command = message.Content.Substring(1).Split(' ')[0];
                string argument = "";
                if (message.Content.Length >= (PrivateData.Prefix + command + " ").ToCharArray().Length)
                {
                    argument = message.Content.Substring((PrivateData.Prefix + command + " ").ToCharArray().Length);
                }
                if (argument != "")
                {
                    switch (command)
                    {
                        case "анон":
                        case "анонимно": await Task.Run(() => AnonymousMessageCommands.SendAsync(message, argument)); break;
                        case "удалианон": await Task.Run(() => AnonymousMessageCommands.DeleteAsync(message, argument)); break;
                        case "ктоанон": await Task.Run(() => AnonymousMessageCommands.GetAnonymousUserAsync(message, argument)); break;

                        case "объяви": await Task.Run(() => AnnouncementCommands.SendAsync(message, argument)); break;

                        case "новость": await Task.Run(() => NewsCommands.SendAsync(message, argument)); break;
                        case "новость+к": await Task.Run(() => NewsCommands.SendAsync(message, argument, true)); break;

                        case "дайроль": await Task.Run(() => ManageTheRolesCommands.GetAsync(message, argument)); break;

                        case "хочуиграть": await Task.Run(() => WantPlayMessageCommands.SendAsync(argument, message)); break;

                        case "голосование": await Task.Run(() => VotingCommands.AddVotingdAsync(message, argument)); break;
                        case "удалиголосование": await Task.Run(() => VotingCommands.DeleteVotingAsync(message, argument)); break;

                        case "радуга": await Task.Run(() => ChangeTheRoleCommands.SetTheRoleColorAutoChangingAsync(argument, message)); break;
                        case "хакерканал": await Task.Run(() => ChangeTheChannelCommands.SetTheChannelNameAutoChangingAsync(argument, message)); break;

                        case "добавьлист": await Task.Run(() => RatingListCommands.AddListAsync(message, argument)); break;
                        case "удалилист": await Task.Run(() => RatingListCommands.RemoveListAsync(message, argument)); break;

                        case "добавьоб": await Task.Run(() => RatingListCommands.AddValueAsync(message, argument)); break;
                        case "удалиоб": await Task.Run(() => RatingListCommands.RemoveValueAsync(message, argument)); break;

                        default:
                            await message.DeleteAsync();
                            await message.Author.SendMessageAsync($"Команда {command} не определена.\nВаше запрос: " + message.Content);
                            break;
                    }
                }
                else
                {
                    switch (command)
                    {
                        case "стоп": await Task.Run(() => BotControlCommands.Stop(message, Client.BotClient)); break;
                        case "удалиданные": await Task.Run(() => BotControlCommands.ClearData(message, Client.BotClient)); break;

                        default:
                            await message.DeleteAsync();
                            await message.Author.SendMessageAsync($"Неаргументированная команда {command} не определена.\nВаше запрос: " + message.Content);
                            break;
                    }
                }
            }
        }
    }
}

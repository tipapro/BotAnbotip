using BotAnbotip.Bot.Client;
using BotAnbotip.Bot.Data;
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
    class CommandManager
    {
        public static async Task RunCommand(string command, string argument, SocketMessage message)
        {
            if (argument != "")
            {
                switch (command)
                {
                    case "ктоанон": await Task.Run(() => AnonymousMessageCommands.GetAnonymousUserAsync(message, argument)); break;

                    case "добавьлист": await Task.Run(() => RatingListCommands.AddListAsync(message, argument)); break;
                    case "удалилист": await Task.Run(() => RatingListCommands.RemoveListAsync(message, argument)); break;

                    case "радуга": await Task.Run(() => RainbowRoleCommands.ChangeStateOfTheRainbowRoleAsync(argument, message)); break;
                    case "хакерканал": await Task.Run(() => HackerChannelCommands.ChangeStateOfTheHackerChannelAsync(argument, message)); break;

                    case "добавьоб": await Task.Run(() => RatingListCommands.AddValueAsync(message, argument)); break;
                    case "удалиоб": await Task.Run(() => RatingListCommands.RemoveValueAsync(message, argument)); break;

                    case "новость": await Task.Run(() => NewsCommands.SendAsync(message, argument)); break;
                    case "новость+к": await Task.Run(() => NewsCommands.SendAsync(message, argument, true)); break;
                    case "новость+ю": await Task.Run(() => NewsCommands.SendAsync(message, argument, false, true)); break;

                    case "голосование": await Task.Run(() => VotingCommands.AddVotingdAsync(message, argument)); break;
                    case "удалиголосование": await Task.Run(() => VotingCommands.DeleteVotingAsync(message, argument)); break;

                    case "анон":
                    case "анонимно": await Task.Run(() => AnonymousMessageCommands.SendAsync(message, argument)); break;
                    case "удалианон": await Task.Run(() => AnonymousMessageCommands.DeleteAsync(message, argument)); break;

                    case "объяви": await Task.Run(() => AnnouncementCommands.SendAsync(message, argument)); break;

                    case "дайроль": await Task.Run(() => ManageTheRolesCommands.GetAsync(message, argument)); break;

                    case "хочуиграть": await Task.Run(() => WantPlayMessageCommands.SendAsync(argument, message)); break;

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
                    case "стоп": await Task.Run(() => BotControlCommands.Stop(message, BotClient.Client)); break;
                    case "удалиданные": await Task.Run(() => BotControlCommands.ClearData(message, BotClient.Client)); break;

                    case "debug0": DebugCommands.ChangeFlag(message, 0); break;
                    case "debug1": DebugCommands.ChangeFlag(message, 1); break;
                    case "debug2": DebugCommands.ChangeFlag(message, 2); break;
                    case "debug3": DebugCommands.ChangeFlag(message, 3); break;
                    case "debug4": DebugCommands.ChangeFlag(message, 4); break;
                    default:
                        await message.DeleteAsync();
                        await message.Author.SendMessageAsync($"Неаргументированная команда {command} не определена.\nВаше запрос: " + message.Content);
                        break;
                }
            }
        }

        public static bool CheckPermission(IGuildUser user, RoleIds minimalRole)
        {
            var userRoles = user.RoleIds;
            byte reqPermLevel = GetRequiredPermLevel(minimalRole);
            byte userPermLevel = (byte)PermLevelOfRole.everyone;

            if (userRoles.Contains((ulong)RoleIds.Основатель)) userPermLevel = (byte)PermLevelOfRole.Основатель;
            else if (userRoles.Contains((ulong)RoleIds.Заместитель)) userPermLevel = (byte)PermLevelOfRole.Заместитель;
            else if (userRoles.Contains((ulong)RoleIds.Администратор)) userPermLevel = (byte)PermLevelOfRole.Администратор;
            else if (userRoles.Contains((ulong)RoleIds.Модератор)) userPermLevel = (byte)PermLevelOfRole.Модератор;
            else if (userRoles.Contains((ulong)RoleIds.Активный_Участник)) userPermLevel = (byte)PermLevelOfRole.Активный_Участник;
            else if (userRoles.Contains((ulong)RoleIds.Участник)) userPermLevel = (byte)PermLevelOfRole.Участник;

            return reqPermLevel <= userPermLevel;
        }

        public static byte GetRequiredPermLevel(RoleIds minimalRole)
        {
            switch (minimalRole)
            {
                case RoleIds.Участник: return (byte)PermLevelOfRole.Участник;
                case RoleIds.Активный_Участник: return (byte)PermLevelOfRole.Активный_Участник;
                case RoleIds.Модератор: return (byte)PermLevelOfRole.Модератор;
                case RoleIds.Администратор: return (byte)PermLevelOfRole.Администратор;
                case RoleIds.Заместитель: return (byte)PermLevelOfRole.Заместитель;
                case RoleIds.Основатель: return (byte)PermLevelOfRole.Основатель;
                default: return 0;
            }
        }

        internal static bool CheckPermission(IGuildUser author, object основатель)
        {
            throw new NotImplementedException();
        }
    }
}

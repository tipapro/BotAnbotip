using BotAnbotip.Bot.Clients;
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
        private ulong botId;

        public CommandManager(ulong botId)
        {
            this.botId = botId;
        }

        public async Task RunCommand(string command, string argument, SocketMessage message)
        {
            if (argument != "")
            {
                if (botId == BotClientManager.MainBot.Id)
                {
                    switch (command)
                    {
                        case "ктоанон":
                        case "whoisanon": await AnonymousMessageCommands.GetAnonymousUserAsync(message, argument); break;

                        case "+лист":
                        case "добавьлист":
                        case "+list":
                        case "addlist":
                            RatingListCommands.AwaitedTask?.Wait();
                            RatingListCommands.AwaitedTask = RatingListCommands.AddListAsync(message, argument); break;

                        case "-лист":
                        case "удалилист":
                        case "-list":
                        case "removelist":
                            RatingListCommands.AwaitedTask?.Wait();
                            RatingListCommands.AwaitedTask = RatingListCommands.RemoveListAsync(message, argument); break;

                        case "ревёрс":
                        case "ревёрслист":
                        case "reverselist":
                            RatingListCommands.AwaitedTask?.Wait();
                            RatingListCommands.AwaitedTask = RatingListCommands.ReverseAsync(message, argument); break;

                        case "+об":
                        case "добавьоб":
                        case "+obj":
                        case "addobj":
                            RatingListCommands.AwaitedTask?.Wait();
                            RatingListCommands.AwaitedTask = RatingListCommands.AddValueAsync(message, argument, false, false); break;

                        case "+об+с":
                        case "добавьоб+с":
                        case "+obj+l":
                        case "addobj+l":
                            RatingListCommands.AwaitedTask?.Wait();
                            RatingListCommands.AwaitedTask = RatingListCommands.AddValueAsync(message, argument, true, false); break;

                        case "+об+к":
                        case "добавьоб+к":
                        case "+obj+i":
                        case "addobj+i":
                            RatingListCommands.AwaitedTask?.Wait();
                            RatingListCommands.AwaitedTask = RatingListCommands.AddValueAsync(message, argument, false, true); break;

                        case "+об+с+к":
                        case "+об+к+с":
                        case "добавьоб+с+к":
                        case "добавьоб+к+с":
                        case "+obj+l+i":
                        case "+obj+i+l":
                        case "addobj+i+l":
                        case "addobj+l+i":
                            RatingListCommands.AwaitedTask?.Wait();
                            RatingListCommands.AwaitedTask = RatingListCommands.AddValueAsync(message, argument, true, true); break;

                        case "-об":
                        case "удалиоб":
                        case "-obj":
                        case "removeobj": await RatingListCommands.RemoveValueAsync(message, argument); break;

                        case "новость":
                        case "news": await NewsCommands.SendAsync(message, argument); break;

                        case "новость+к":
                        case "news+p": await NewsCommands.SendAsync(message, argument, true); break;

                        case "новость+ю":
                        case "news+y": await NewsCommands.SendAsync(message, argument, false, true); break;

                        case "голосование":
                        case "voting": await VotingCommands.AddVotingdAsync(message, argument); break;

                        case "-голосование":
                        case "удалиголосование":
                        case "-voting":
                        case "deletevoting": await VotingCommands.DeleteVotingAsync(message, argument); break;

                        case "анон":
                        case "анонимно":
                        case "anon":
                        case "anonymously": await AnonymousMessageCommands.SendAsync(message, argument); break;

                        case "-анон":
                        case "удалианон":
                        case "-anon":
                        case "deleteanon":
                        case "deleteanonymousmessage": await AnonymousMessageCommands.DeleteAsync(message, argument); break;

                        case "объяви":
                        case "анонс":
                        case "announce": await AnnouncementCommands.SendAsync(message, argument); break;

                        case "дайроль":
                        case "giveme":
                        case "giverole": await ManageTheRolesCommands.GetAsync(message, argument); break;

                        case "хочуиграть":
                        case "wantplay": await WantPlayMessageCommands.SendAsync(argument, message); break;

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
                        case "радуга":
                        case "rainbow": await RainbowRoleCommands.ChangeRainbowRoleState(message, argument); break;

                        case "хакерканал":
                        case "hakerch":
                        case "hakerchannel": await HackerChannelCommands.ChangeStateOfTheHackerChannelAsync(message, argument); break;

                        default:
                            await message.DeleteAsync();
                            await message.Author.SendMessageAsync($"Команда {command} не определена.\nВаше запрос: " + message.Content);
                            break;
                    }
                }
            }
            else
            {
                switch (command)
                {
                    case "стоп":
                    case "stop": BotControlCommands.Stop(message, BotClientManager.MainBot.Client); break;

                    case "удалиданные":
                    case "cleardata": BotControlCommands.ClearData(message, BotClientManager.MainBot.Client); break;

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

            if (reqPermLevel <= userPermLevel) return true;
            user.SendMessageAsync("У вас недостаточно прав: минимальный уровень - " + minimalRole).GetAwaiter().GetResult();
            return false;
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
    }
}

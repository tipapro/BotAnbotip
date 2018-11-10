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
        public const char ArgumentPrefix = '/';

        private readonly List<CommandsBase> _commandsCollection;

        public static AnnouncementCommands Announcement { get; private set; }
        public static AnonymousMessageCommands AnonymousMessage { get; private set; }
        public static BotControlCommands BotControl { get; private set; }
        public static DebugCommands Debug { get; private set; }
        public static RoleManagementCommands RoleManagement { get; private set; }
        public static NewsCommands News { get; private set; }
        public static RatingListCommands RatingList { get; private set; }
        public static VotingCommands Voting { get; private set; }
        public static WantPlayMessageCommands WantPlayMessage { get; private set; }
        public static UserProfileCommands UserProfile { get; private set; }
        

        public static HackerChannelCommands HackerChannel { get; private set; }

        public CommandManager(ulong botId)
        {
            Announcement = new AnnouncementCommands();
            AnonymousMessage = new AnonymousMessageCommands();
            BotControl = new BotControlCommands();
            Debug = new DebugCommands();
            RoleManagement = new RoleManagementCommands();
            News = new NewsCommands();
            RatingList = new RatingListCommands();
            Voting = new VotingCommands();
            WantPlayMessage = new WantPlayMessageCommands();
            UserProfile = new UserProfileCommands();

            HackerChannel = new HackerChannelCommands();

            if (botId == BotClientManager.MainBot.Id)
                _commandsCollection = new List<CommandsBase> { Announcement, AnonymousMessage, BotControl, Debug, RoleManagement, News,
                RatingList, Voting, WantPlayMessage, UserProfile };
        }

        public async Task RunCommand(string commandName, string argument, SocketMessage message)// !!!!!!!!!!
        {
            foreach (var commands in _commandsCollection)
            {
                var command = commands[commandName];
                if (command != null) await command.Invoke(message, argument);
            }
        }

        public static bool CheckPermission(IGuildUser user, RoleIds minimalRole)
        {
            var userRoles = user.RoleIds;
            byte reqPermLevel = GetRolePermLevel((ulong)minimalRole);
            var userPermLevel = GetUserPermLevel(userRoles);

            if (reqPermLevel <= userPermLevel) return true;
            user.SendMessageAsync("У вас недостаточно прав: минимальный уровень - " + minimalRole).GetAwaiter().GetResult();
            return false;
        }

        public static byte GetUserPermLevel(IEnumerable<ulong> userRoles)
        {
            byte userPermLevel = (byte)PermLevelOfRole.everyone;

            if (userRoles.Contains((ulong)RoleIds.Founder)) userPermLevel = (byte)PermLevelOfRole.Founder;
            else if (userRoles.Contains((ulong)RoleIds.Co_founder)) userPermLevel = (byte)PermLevelOfRole.Co_founder;
            else if (userRoles.Contains((ulong)RoleIds.Admin)) userPermLevel = (byte)PermLevelOfRole.Admin;
            else if (userRoles.Contains((ulong)RoleIds.Moderator)) userPermLevel = (byte)PermLevelOfRole.Moderator;
            else if (userRoles.Contains((ulong)RoleIds.DELETED_Active_Member)) userPermLevel = (byte)PermLevelOfRole.DELETED_Active_Member;
            else if (userRoles.Contains((ulong)RoleIds.Member)) userPermLevel = (byte)PermLevelOfRole.Member;
            return userPermLevel;
        }

        public static byte GetRolePermLevel(ulong minimalRole)
        {
            switch (minimalRole)
            {
                case (ulong)RoleIds.Member: return (byte)PermLevelOfRole.Member;
                case (ulong)RoleIds.DELETED_Active_Member: return (byte)PermLevelOfRole.DELETED_Active_Member;
                case (ulong)RoleIds.Moderator: return (byte)PermLevelOfRole.Moderator;
                case (ulong)RoleIds.Admin: return (byte)PermLevelOfRole.Admin;
                case (ulong)RoleIds.Co_founder: return (byte)PermLevelOfRole.Co_founder;
                case (ulong)RoleIds.Founder: return (byte)PermLevelOfRole.Founder;
                default: return byte.MaxValue;
            }
        }


        public static List<(char, string)> ClearAndGetCommandArguments(ref string text)
        {
            List<(char, string)> resultList = new List<(char, string)>();
            int startIndex = 0;
            char argument;
            string resultString;
            var charArray = text.ToCharArray();
            for (var i = 0; i < charArray.Length - 3; i++)
            {
                if ((charArray[i] != ArgumentPrefix) || (charArray[i + 1] == ArgumentPrefix)) continue;
                resultString = "";
                argument = charArray[i + 1];
                if ((i + 4 < charArray.Length) && (charArray[i + 2] == ':') && (charArray[i + 3] == '"'))
                {
                    for (var j = i + 4; j < charArray.Length; j++)
                    {
                        if (charArray[j] != '"') continue;
                        resultString = text.Substring(i + 4, j - i - 4);
                        i = j;
                        startIndex = j + 1;
                        break;
                    }
                }
                else
                {
                    if (charArray[i + 2] != ' ') continue;
                    startIndex = i + 2;
                }
                resultList.Add((argument, resultString));
            }
            text = text.Substring(startIndex).Trim();
            return resultList;
        }
    }
}

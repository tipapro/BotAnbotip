﻿using BotAnbotip.Bot.Clients;
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

            if (userRoles.Contains((ulong)RoleIds.Основатель)) userPermLevel = (byte)PermLevelOfRole.Основатель;
            else if (userRoles.Contains((ulong)RoleIds.Заместитель)) userPermLevel = (byte)PermLevelOfRole.Заместитель;
            else if (userRoles.Contains((ulong)RoleIds.Администратор)) userPermLevel = (byte)PermLevelOfRole.Администратор;
            else if (userRoles.Contains((ulong)RoleIds.Модератор)) userPermLevel = (byte)PermLevelOfRole.Модератор;
            else if (userRoles.Contains((ulong)RoleIds.Активный_Участник)) userPermLevel = (byte)PermLevelOfRole.Активный_Участник;
            else if (userRoles.Contains((ulong)RoleIds.Участник)) userPermLevel = (byte)PermLevelOfRole.Участник;
            return userPermLevel;
        }

        public static byte GetRolePermLevel(ulong minimalRole)
        {
            switch (minimalRole)
            {
                case (ulong)RoleIds.Участник: return (byte)PermLevelOfRole.Участник;
                case (ulong)RoleIds.Активный_Участник: return (byte)PermLevelOfRole.Активный_Участник;
                case (ulong)RoleIds.Модератор: return (byte)PermLevelOfRole.Модератор;
                case (ulong)RoleIds.Администратор: return (byte)PermLevelOfRole.Администратор;
                case (ulong)RoleIds.Заместитель: return (byte)PermLevelOfRole.Заместитель;
                case (ulong)RoleIds.Основатель: return (byte)PermLevelOfRole.Основатель;
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

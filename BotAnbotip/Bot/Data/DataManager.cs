using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data.CustomClasses;
using BotAnbotip.Bot.Data.CustomEnums;

namespace BotAnbotip.Bot.Data
{
    public class DataManager
    {
        public static DropboxData<Dictionary<ulong, ulong>> AnonymousMessages;    //MessageId -- UserId
        public static DropboxData<Dictionary<ulong, RatingList>> RatingChannels;    //ChannelId -- Rating List
        public static DropboxData<Dictionary<ulong, Tuple<DateTimeOffset, List<ulong>>>> AgreeingToPlayUsers;    //MessageId -- List of UserIds
        public static DropboxData<Dictionary<ulong, List<Tuple<string, int>>>> VotingLists;     //
        public static DropboxData<Dictionary<GiveawayType, List<ulong>>> ParticipantsOfTheGiveaway;     //GiveawayType -- List of UserIds
        public static DropboxData<Dictionary<GiveawayType, ulong>> LastWinner;   //GiveawayType -- UserId
        public static DropboxData<bool> RainbowRoleIsRunning;
        public static DropboxData<ulong> RainbowRoleId;
        public static DropboxData<bool> HackerChannelIsRunning;
        public static DropboxData<ulong> HackerChannelId;
        public static DropboxData<bool> DidRoleGiveawayBegin;

        public static bool[] DebugTriger = new bool[5];


        public static DropboxData<T> InitializeDropboxData<T>(DropboxData<T> obj, string fileName) => new DropboxData<T>(fileName);
        public static void InitializeAllVariables()
        {
            AnonymousMessages = InitializeDropboxData(AnonymousMessages, nameof(AnonymousMessages));
            RatingChannels = InitializeDropboxData(RatingChannels, nameof(RatingChannels));
            AgreeingToPlayUsers = InitializeDropboxData(AgreeingToPlayUsers, nameof(AgreeingToPlayUsers));
            ParticipantsOfTheGiveaway = InitializeDropboxData(ParticipantsOfTheGiveaway, nameof(ParticipantsOfTheGiveaway));
            LastWinner = InitializeDropboxData(LastWinner, nameof(LastWinner));
            RainbowRoleIsRunning = InitializeDropboxData(RainbowRoleIsRunning, nameof(RainbowRoleIsRunning));
            RainbowRoleId = InitializeDropboxData(RainbowRoleId, nameof(RainbowRoleId));
            HackerChannelIsRunning = InitializeDropboxData(HackerChannelIsRunning, nameof(HackerChannelIsRunning));
            HackerChannelId = InitializeDropboxData(HackerChannelId, nameof(HackerChannelId));
            DidRoleGiveawayBegin = InitializeDropboxData(DidRoleGiveawayBegin, nameof(DidRoleGiveawayBegin));
        }       

        public static async Task SaveAllDataAsync()
        {
            await AnonymousMessages.SaveAsync();
            await RatingChannels.SaveAsync();
            await AgreeingToPlayUsers.SaveAsync();
            await VotingLists.SaveAsync();
            await ParticipantsOfTheGiveaway.SaveAsync();
            await LastWinner.SaveAsync();
            await RainbowRoleIsRunning.SaveAsync();
            await HackerChannelIsRunning.SaveAsync();
            await RainbowRoleId.SaveAsync();
            await HackerChannelId.SaveAsync();
            await DidRoleGiveawayBegin.SaveAsync();
        }

        public static async Task ReadAllDataAsync()
        {
            await AnonymousMessages.ReadAsync();
            await RatingChannels.ReadAsync();
            await AgreeingToPlayUsers.ReadAsync();
            await VotingLists.ReadAsync();
            await ParticipantsOfTheGiveaway.ReadAsync();
            await LastWinner.ReadAsync();
            await RainbowRoleIsRunning.ReadAsync();
            await HackerChannelIsRunning.ReadAsync();
            await RainbowRoleId.ReadAsync();
            await HackerChannelId.ReadAsync();
            await DidRoleGiveawayBegin.ReadAsync();
        }

        public static void RemoveRatingList(ulong id) => RatingChannels.Value.Remove(id);

    }
}

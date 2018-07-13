using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data.CustomClasses;
using BotAnbotip.Bot.Data.CustomEnums;

namespace BotAnbotip.Bot.Data
{
    public class DataManager
    {
        public static DropboxData<Dictionary<ulong, ulong>> AnonymousMessages;    //MessageId -- UserId
        public static DropboxData<Dictionary<ulong, RatingList>> RatingChannels;    //ChannelId -- Rating List
        public static DropboxData<Dictionary<ulong, (DateTimeOffset, List<ulong>)>> AgreeingToPlayUsers;    //MessageId -- List of UserIds
        public static DropboxData<Dictionary<ulong, List<(string, int)>>> VotingLists;     //
        public static DropboxData<Dictionary<GiveawayType, List<ulong>>> ParticipantsOfTheGiveaway;     //GiveawayType -- List of UserIds
        public static DropboxData<Dictionary<GiveawayType, ulong>> LastWinner;   //GiveawayType -- UserId
        public static DropboxData<Dictionary<ulong, Dictionary<string, List<ulong>>>> Subscribers;    //User -- Games -- His subscribers
        public static DropboxData<Dictionary<ulong, UserProfile>> UserProfiles;

        public static DropboxData<bool> DidRoleGiveawayBegin;
        public static DropboxData<bool> HackerChannelIsRunning;
        public static DropboxData<ulong> HackerChannelId;
        

        public static bool[] DebugTriger = new bool[5];


        public static DropboxData<T> InitializeDropboxData<T>(DropboxData<T> obj, string fileName) => new DropboxData<T>(fileName);
        public static void InitializeAllVariables()
        {
            AnonymousMessages = InitializeDropboxData(AnonymousMessages, nameof(AnonymousMessages));
            RatingChannels = InitializeDropboxData(RatingChannels, nameof(RatingChannels));
            AgreeingToPlayUsers = InitializeDropboxData(AgreeingToPlayUsers, nameof(AgreeingToPlayUsers));
            VotingLists = InitializeDropboxData(VotingLists, nameof(VotingLists));
            ParticipantsOfTheGiveaway = InitializeDropboxData(ParticipantsOfTheGiveaway, nameof(ParticipantsOfTheGiveaway));
            LastWinner = InitializeDropboxData(LastWinner, nameof(LastWinner));
            Subscribers = InitializeDropboxData(Subscribers, nameof(Subscribers));
            UserProfiles = InitializeDropboxData(UserProfiles, nameof(UserProfiles));

            DidRoleGiveawayBegin = InitializeDropboxData(DidRoleGiveawayBegin, nameof(DidRoleGiveawayBegin));
            HackerChannelIsRunning = InitializeDropboxData(HackerChannelIsRunning, nameof(HackerChannelIsRunning));
            HackerChannelId = InitializeDropboxData(HackerChannelId, nameof(HackerChannelId));
        }       

        public static async Task SaveAllDataAsync()
        {
            try
            {
                await AnonymousMessages.SaveAsync();
                await RatingChannels.SaveAsync();
                await AgreeingToPlayUsers.SaveAsync();
                await VotingLists.SaveAsync();
                await ParticipantsOfTheGiveaway.SaveAsync();
                await LastWinner.SaveAsync();
                await Subscribers.SaveAsync();
                await UserProfiles.SaveAsync();

                await DidRoleGiveawayBegin.SaveAsync();
                await HackerChannelIsRunning.SaveAsync();
                await HackerChannelId.SaveAsync();
            }
            catch (Exception ex)
            {
                new ExceptionLogger().Log(ex, "Save data error");
            }
        }

        public static async Task ReadAllDataAsync()
        {
            try
            {
                InitializeAllVariables();
                await AnonymousMessages.ReadAsync();
                await RatingChannels.ReadAsync();
                await AgreeingToPlayUsers.ReadAsync();
                await VotingLists.ReadAsync();
                await ParticipantsOfTheGiveaway.ReadAsync();
                await LastWinner.ReadAsync();
                await Subscribers.ReadAsync();
                await UserProfiles.ReadAsync();

                await DidRoleGiveawayBegin.ReadAsync();
                await HackerChannelIsRunning.ReadAsync();
                await HackerChannelId.ReadAsync();
            }
            catch (Exception ex)
            {
                new ExceptionLogger().Log(ex, "Read data error");
            }
        }
    }
}

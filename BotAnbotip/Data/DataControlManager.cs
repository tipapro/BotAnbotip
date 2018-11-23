using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotAnbotip.Clients;
using BotAnbotip.Data.CustomClasses;
using BotAnbotip.Data.CustomEnums;

namespace BotAnbotip.Data
{
    public class DataControlManager
    {
        public static CloudData<Dictionary<ulong, ulong>> AnonymousMessages;    //MessageId -- UserId
        public static CloudData<Dictionary<ulong, RatingList>> RatingChannels;    //ChannelId -- Rating List
        public static CloudData<Dictionary<ulong, (DateTimeOffset, List<ulong>)>> AgreeingToPlayUsers;    //MessageId -- List of UserIds
        public static CloudData<Dictionary<ulong, List<(string, int)>>> VotingLists;     //
        public static CloudData<Dictionary<GiveawayType, List<ulong>>> ParticipantsOfTheGiveaway;     //GiveawayType -- List of UserIds
        public static CloudData<Dictionary<GiveawayType, ulong>> LastWinner;   //GiveawayType -- UserId
        public static CloudData<Dictionary<ulong, Dictionary<string, List<ulong>>>> Subscribers;    //User -- Games -- His subscribers
        public static CloudData<Dictionary<ulong, UserProfile>> UserProfiles;
        public static CloudData<List<(ulong Id, long Points, int Level)>> UserTopList;


        public static CloudData<bool> DidRoleGiveawayBegin;
        

        public static bool[] DebugTriger = new bool[5];


        public static void InitializeCloudData<T>(ref CloudData<T> obj, string fileName) => new CloudData<T>(fileName);

        public static void InitializeAll()
        {
            InitializeCloudData(ref AnonymousMessages, nameof(AnonymousMessages));
            InitializeCloudData(ref RatingChannels, nameof(RatingChannels));
            InitializeCloudData(ref AgreeingToPlayUsers, nameof(AgreeingToPlayUsers));
            InitializeCloudData(ref VotingLists, nameof(VotingLists));
            InitializeCloudData(ref ParticipantsOfTheGiveaway, nameof(ParticipantsOfTheGiveaway));
            InitializeCloudData(ref LastWinner, nameof(LastWinner));
            InitializeCloudData(ref Subscribers, nameof(Subscribers));
            InitializeCloudData(ref UserProfiles, nameof(UserProfiles));
            InitializeCloudData(ref UserTopList, nameof(UserTopList));

            InitializeCloudData(ref DidRoleGiveawayBegin, nameof(DidRoleGiveawayBegin));
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
                await UserTopList.SaveAsync();

                await DidRoleGiveawayBegin.SaveAsync();
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
                InitializeAll();
                await AnonymousMessages.ReadAsync();
                await RatingChannels.ReadAsync();
                await AgreeingToPlayUsers.ReadAsync();
                await VotingLists.ReadAsync();
                await ParticipantsOfTheGiveaway.ReadAsync();
                await LastWinner.ReadAsync();
                await Subscribers.ReadAsync();
                await UserProfiles.ReadAsync();
                await UserTopList.ReadAsync();

                await DidRoleGiveawayBegin.ReadAsync();
            }
            catch (Exception ex)
            {
                new ExceptionLogger().Log(ex, "Read data error");
            }
        }
    }
}

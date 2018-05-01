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
        public static DropboxData<Dictionary<ulong, (DateTimeOffset, List<ulong>)>> AgreeingToPlayUsers;    //MessageId -- List of UserIds
        public static DropboxData<Dictionary<ulong, List<(string, int)>>> VotingLists;     //
        public static DropboxData<Dictionary<GiveawayType, List<ulong>>> ParticipantsOfTheGiveaway;     //GiveawayType -- List of UserIds
        public static DropboxData<Dictionary<GiveawayType, ulong>> LastWinner;   //GiveawayType -- UserId
        public static DropboxData<bool> DidRoleGiveawayBegin;

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
            await DidRoleGiveawayBegin.SaveAsync();
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
                await DidRoleGiveawayBegin.ReadAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Read data error");
                Console.WriteLine(ex.Message);
            }
        }

        public static void RemoveRatingList(ulong id) => RatingChannels.Value.Remove(id);

    }
}

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data.CustomClasses;
using BotAnbotip.Bot.Data.CustomEnums;

namespace BotAnbotip.Bot.Data
{
    [JsonObject]
    public class DataManager
    {
        public static Dictionary<ulong, ulong> AnonymousMessages;    //MessageId -- UserId
        public static Dictionary<ulong, RatingList> RatingChannels;    //ChannelId -- Rating List
        public static Dictionary<ulong, Tuple<DateTimeOffset, List<ulong>>> AgreeingToPlayUsers;    //MessageId -- List of UserIds
        public static Dictionary<ulong, List<Tuple<string, int>>> VotingLists;     //
        public static Dictionary<GiveawayType, List<ulong>> ParticipantsOfTheGiveaway;     //GiveawayType -- List of UserIds
        public static Dictionary<GiveawayType, ulong> LastWinner;   //GiveawayType -- UserId
        public static bool RainbowRoleIsRunning;
        public static ulong RainbowRoleId;
        public static bool HackerChannelIsRunning;
        public static ulong HackerChannelId;
        public static bool DidRoleGiveawayBegin;

        [JsonIgnore]
        public static bool[] DebugTriger = new bool[5];
       

        public static void InitializeAllVariables()
        {
            AnonymousMessages = InitializeVariable(AnonymousMessages);
            RatingChannels = InitializeVariable(RatingChannels);
            AgreeingToPlayUsers = InitializeVariable(AgreeingToPlayUsers);
            VotingLists = InitializeVariable(VotingLists);
            ParticipantsOfTheGiveaway = InitializeVariable(ParticipantsOfTheGiveaway);
            LastWinner = InitializeVariable(LastWinner);

            RainbowRoleIsRunning = InitializeVariable(RainbowRoleIsRunning);
            HackerChannelIsRunning = InitializeVariable(HackerChannelIsRunning);
            RainbowRoleId = InitializeVariable(RainbowRoleId);
            HackerChannelId = InitializeVariable(HackerChannelId);
            DidRoleGiveawayBegin = InitializeVariable(DidRoleGiveawayBegin);
        }

        public static T InitializeVariable<T>(T obj)
        {
            var constructor = typeof(T).GetConstructor(Type.EmptyTypes);
            if (constructor == null) return default(T);
            else return (T)constructor.Invoke(new object[0]);
        }


        public static async Task SaveDataAsync<T>(T obj, string objName)
        {
            string json = JsonConvert.SerializeObject(obj);
            await DropboxIntegration.UploadAsync(PrivateData.FileNamePrefix + objName + ".json", json);
        }

        public static async Task<T> ReadDataAsync<T>(T obj, string objName)
        {            
            DropboxIntegration.Authorization(PrivateData.DropboxApiKey);

            string json = await DropboxIntegration.DownloadAsync(PrivateData.FileNamePrefix + objName + ".json");

            if (json != "")
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            else return InitializeVariable(obj);

        }

        public static async Task SaveAllDataAsync()
        {
            await SaveDataAsync(AnonymousMessages, nameof(AnonymousMessages));
            await SaveDataAsync(RatingChannels, nameof(RatingChannels));
            await SaveDataAsync(AgreeingToPlayUsers, nameof(AgreeingToPlayUsers));
            await SaveDataAsync(VotingLists, nameof(VotingLists));
            await SaveDataAsync(ParticipantsOfTheGiveaway, nameof(ParticipantsOfTheGiveaway));
            await SaveDataAsync(LastWinner, nameof(LastWinner));

            await SaveDataAsync(RainbowRoleIsRunning, nameof(RainbowRoleIsRunning));
            await SaveDataAsync(HackerChannelIsRunning, nameof(HackerChannelIsRunning));
            await SaveDataAsync(RainbowRoleId, nameof(RainbowRoleId));
            await SaveDataAsync(HackerChannelId, nameof(HackerChannelId));
            await SaveDataAsync(DidRoleGiveawayBegin, nameof(DidRoleGiveawayBegin));
        }

        public static async Task ReadAllDataAsync()
        {
            AnonymousMessages = await ReadDataAsync(AnonymousMessages, nameof(AnonymousMessages));
            RatingChannels = await ReadDataAsync(RatingChannels, nameof(RatingChannels));
            AgreeingToPlayUsers = await ReadDataAsync(AgreeingToPlayUsers, nameof(AgreeingToPlayUsers));
            VotingLists = await ReadDataAsync(VotingLists, nameof(VotingLists));
            ParticipantsOfTheGiveaway = await ReadDataAsync(ParticipantsOfTheGiveaway, nameof(ParticipantsOfTheGiveaway));
            LastWinner = await ReadDataAsync(LastWinner, nameof(LastWinner));
            RainbowRoleIsRunning = await ReadDataAsync(RainbowRoleIsRunning, nameof(RainbowRoleIsRunning));
            HackerChannelIsRunning = await ReadDataAsync(HackerChannelIsRunning, nameof(HackerChannelIsRunning));
            RainbowRoleId = await ReadDataAsync(RainbowRoleId, nameof(RainbowRoleId));
            HackerChannelId = await ReadDataAsync(HackerChannelId, nameof(HackerChannelId));
            DidRoleGiveawayBegin = await ReadDataAsync(DidRoleGiveawayBegin, nameof(DidRoleGiveawayBegin));
        }

        public static void RemoveRatingList(string name)
        {
            foreach(var ratingList in RatingChannels)
            {
                if (ratingList.Value.Name == name) RatingChannels.Remove(ratingList.Key);
            }
        }

        public static void RemoveRatingList(ulong id)
        {
            RatingChannels.Remove(id);
        }

    }
}

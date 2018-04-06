using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Dropbox.Api;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.Data
{
    [JsonObject]
    public class DataManager
    {
        [JsonIgnore]
        private const string FileName = "BotAnbotipData";
        [JsonProperty]
        public static Dictionary<ulong, ulong> anonymousMessagesAndUsersIds;    //MessageId -- UserId
        [JsonProperty]
        public static Dictionary<ulong, RatingList> ratingChannels;  //ChannelId -- UserId -- Object'sName -- IsLiked
        [JsonProperty]
        public static Dictionary<ulong, List<ulong>> agreeingToPlayUsers;    //MessageId -- List of UserIds
        [JsonProperty]
        public static bool RoleColorAutoChangingIsSwitchedOn;
        [JsonProperty]
        public static ulong RoleColorAutoChangingId;
        [JsonProperty]
        public static bool ChannelNameAutoChangingIsSwitchedOn;
        [JsonProperty]
        public static ulong ChannelNameAutoChangingId;

        internal static void Clear()
        {
            anonymousMessagesAndUsersIds = new Dictionary<ulong, ulong>();
            ratingChannels = new Dictionary<ulong, RatingList>();
            agreeingToPlayUsers = new Dictionary<ulong, List<ulong>>();
            RoleColorAutoChangingIsSwitchedOn = false; ;
            RoleColorAutoChangingId = 0;
            ChannelNameAutoChangingIsSwitchedOn = false;
            ChannelNameAutoChangingId = 0;
        }


        public DataManager() { }

        public static async Task SaveDataAsync()
        {
            string json = JsonConvert.SerializeObject(new DataManager());
            await DropboxIntegration.UploadAsync(FileName, json);
        }


        public static async void ReadData()
        {
            
            DropboxIntegration.Authorization(Environment.GetEnvironmentVariable("dropboxToken"));

            string json = await DropboxIntegration.DownloadAsync(FileName);

            if (json != "")
            {
                var dataManager = JsonConvert.DeserializeObject<DataManager>(json);
            }
            else
            {
                anonymousMessagesAndUsersIds = new Dictionary<ulong, ulong>();
                ratingChannels = new Dictionary<ulong, RatingList>();
                agreeingToPlayUsers = new Dictionary<ulong, List<ulong>>();
                RoleColorAutoChangingIsSwitchedOn = false;
                ChannelNameAutoChangingIsSwitchedOn = false;
                RoleColorAutoChangingId = 0;
                ChannelNameAutoChangingId = 0;
            }
        }

        public static void RemoveRatingList(string name)
        {
            foreach(var ratingList in ratingChannels)
            {
                if (ratingList.Value.Name == name) ratingChannels.Remove(ratingList.Key);
            }
        }

        public static void RemoveRatingList(ulong id)
        {
            ratingChannels.Remove(id);
        }

    }
}

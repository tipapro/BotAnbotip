//using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace BotAnbotip.Bot.Data
{
    [JsonObject]
    public class DataManager
    {
        [JsonProperty]
        public static Dictionary<ulong, ulong> anonymousMessagesAndUsersIds;    //MessageId -- UserId
        [JsonProperty]
        public static Dictionary<ulong, RatingList> ratingChannels;  //ChannelId -- UserId -- Game'sName -- IsLiked

        private DataManager() { }

        /*public DataManager(SerializationInfo info, StreamingContext context)
        {
            anonymousMessagesAndUsersIds = info.GetValue("anonymousMessagesAndUsersIds", typeof(Dictionary<ulong, ulong>)) as Dictionary<ulong, ulong>;
            ratingChannels = info.GetValue("ratingChannels", typeof(Dictionary<ulong, RatingList>)) as Dictionary<ulong, RatingList>;


            var connString = "Host=ec2-79-125-110-209.eu-west-1.compute.amazonaws.com;Username=blsckyftydferd;Password=937d599e3a3cb86415a791d6890282f9796c9e1b1fda2b93abdd7f0eab42f785;Database=d4l9d8h7on8sg8";

            using (var connection = new NpgsqlConnection(connString))
            {
                connection.Open();

                // Insert some data
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = "INSERT INTO data (some_field) VALUES (@p)";
                    cmd.Parameters.AddWithValue("p", "Hello world");
                    cmd.ExecuteNonQuery();
                }

                // Retrieve all rows
                using (var cmd = new NpgsqlCommand("SELECT some_field FROM data", connection))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        Console.WriteLine(reader.GetString(0));
            }
    }*/

        public static void SaveData()
        {
            using (FileStream dataFile = new FileStream("data.dat", FileMode.Truncate, FileAccess.Write))
            {
                var serializer = new JsonSerializer();
                using (StreamWriter sw = new StreamWriter(dataFile))
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, new DataManager());
                    }
            }    
        }


        public static void ReadData()
        {
            if (File.Exists("data.dat"))
            {
                using (FileStream dataFile = new FileStream("data.dat", FileMode.Open, FileAccess.Read))
                {
                    var serializer = new JsonSerializer();
                    using (StreamReader sw = new StreamReader(dataFile))
                    using (JsonReader reader = new JsonTextReader(sw))
                    {
                        serializer.Deserialize<DataManager>(reader);
                    }
                }
            }
            else
            {
                File.Create("data.dat").Dispose();
                anonymousMessagesAndUsersIds = new Dictionary<ulong, ulong>();
                ratingChannels = new Dictionary<ulong, RatingList>();
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

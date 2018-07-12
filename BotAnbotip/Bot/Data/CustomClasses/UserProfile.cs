using BotAnbotip.Bot.Clients;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.Data.CustomClasses
{
    [JsonObject]
    public class UserProfile : IComparable<UserProfile>
    {
        public ulong Id { get; }
        [JsonProperty]
        public int Level { get; private set; }
        [JsonProperty]
        public long Points { get; private set; }

        public UserProfile(ulong id)
        {
            Id = id;
        }

        public async Task AddPoints(long numberOfPoints)
        {
            Points += numberOfPoints;
            await CheckUserLevelRole();
        }

        private async Task CheckUserLevelRole()
        {
            if (Level == CustomClasses.Level.RoleList.Length - 1) return;
            if (CustomClasses.Level.Points[CustomClasses.Level.RoleList[Level + 1]] > Points) return;
            
            var oldRole = BotClientManager.MainBot.Guild.GetRole((ulong)CustomClasses.Level.RoleList[Level]);
            var user = BotClientManager.MainBot.Guild.GetUser(Id);

            while (CustomClasses.Level.Points[CustomClasses.Level.RoleList[Level + 1]] <= Points)
            {
                Level++;
            }
            
            var newRole = BotClientManager.MainBot.Guild.GetRole((ulong)CustomClasses.Level.RoleList[Level]);
            await user.RemoveRoleAsync(oldRole);
            await user.AddRoleAsync(newRole);
        }

        public int CompareTo(UserProfile other)
        {
            var result = other.Points.CompareTo(Points);
            if (result == 0) result = Id.CompareTo(other.Id);
            return result;
        }
    }
}

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

        public async Task AddPoints(long amountOfPoints, bool inPercent = false)
        {
            Points += inPercent ? (long)Math.Round(Points * amountOfPoints * 0.01) : amountOfPoints;
            await CheckRiseOfUserLevelRole();
        }

        public async Task RemovePoints(long amountOfPoints)
        {
            Points -= amountOfPoints;
            await CheckFallingOfUserLevelRole();
        }

        private async Task CheckRiseOfUserLevelRole()
        {
            if (Level == LevelInfo.RoleList.Length - 1) return;
            if (LevelInfo.Points[LevelInfo.RoleList[Level + 1]] > Points) return;
            
            var oldRole = BotClientManager.MainBot.Guild.GetRole((ulong)LevelInfo.RoleList[Level]);
            var user = BotClientManager.MainBot.Guild.GetUser(Id);

            while ((Level < LevelInfo.RoleList.Length - 1) && (LevelInfo.Points[LevelInfo.RoleList[Level + 1]] <= Points))
            {
                Level++;
            }
            
            var newRole = BotClientManager.MainBot.Guild.GetRole((ulong)LevelInfo.RoleList[Level]);
            await user.RemoveRoleAsync(oldRole);
            await user.AddRoleAsync(newRole);
        }

        private async Task CheckFallingOfUserLevelRole()
        {
            if (Level == 0) return;
            if (LevelInfo.Points[LevelInfo.RoleList[Level]] < Points) return;

            var oldRole = BotClientManager.MainBot.Guild.GetRole((ulong)LevelInfo.RoleList[Level]);
            var user = BotClientManager.MainBot.Guild.GetUser(Id);

            while ((Level > 0) && (LevelInfo.Points[LevelInfo.RoleList[Level]] > Points))
            {
                Level--;
            }

            var newRole = BotClientManager.MainBot.Guild.GetRole((ulong)LevelInfo.RoleList[Level]);
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

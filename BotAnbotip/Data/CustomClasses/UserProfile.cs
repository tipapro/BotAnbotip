using BotAnbotip.Clients;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Data.CustomClasses
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

        public async Task UpdateLevel()
        {
            await CheckRiseOfUserLevelRole();
            await CheckFallingOfUserLevelRole();
        }

        private async Task CheckRiseOfUserLevelRole()
        {
            if (Level == LevelInfo.RoleList.Length - 1) return;
            if (LevelInfo.Points[LevelInfo.RoleList[Level + 1]] > Points) return;
            
            var oldRole = (ulong)LevelInfo.RoleList[Level];
            while ((Level < LevelInfo.RoleList.Length - 1) && (LevelInfo.Points[LevelInfo.RoleList[Level + 1]] <= Points))
            {
                Level++;
            }
            try
            {
                var user = ClientControlManager.MainBot.Guild.GetUser(Id);
                var newRole = ClientControlManager.MainBot.Guild.GetRole((ulong)LevelInfo.RoleList[Level]);
                await user.RemoveRoleAsync(ClientControlManager.MainBot.Guild.GetRole(oldRole));
                await user.AddRoleAsync(newRole);
            }
            finally { }
        }

        private async Task CheckFallingOfUserLevelRole()
        {
            if (Level == 0) return;
            if (LevelInfo.Points[LevelInfo.RoleList[Level]] < Points) return;

            var oldRole = (ulong)LevelInfo.RoleList[Level];
            while ((Level > 0) && (LevelInfo.Points[LevelInfo.RoleList[Level]] > Points))
            {
                Level--;
            }
            try
            {
                var user = ClientControlManager.MainBot.Guild.GetUser(Id);
                var newRole = ClientControlManager.MainBot.Guild.GetRole((ulong)LevelInfo.RoleList[Level]);
                await user.RemoveRoleAsync(ClientControlManager.MainBot.Guild.GetRole(oldRole));
                await user.AddRoleAsync(newRole);
            }
            finally { }
        }

        public int CompareTo(UserProfile other)
        {
            var result = other.Points.CompareTo(Points);
            if (result == 0) result = Id.CompareTo(other.Id);
            return result;
        }
    }
}

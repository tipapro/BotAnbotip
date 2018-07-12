using BotAnbotip.Bot.Clients;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.Data.CustomClasses
{
    public class UserProfile : IComparable<UserProfile>
    {
        public ulong Id { get; }
        public int Level { get; private set; }
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
            if (Level == LevelPoints.RolelList.Length - 1) return;
            if (LevelPoints.Levels[LevelPoints.RolelList[Level + 1]] > Points) return;
            
            var oldRole = BotClientManager.MainBot.Guild.GetRole((ulong)LevelPoints.RolelList[Level]);
            var user = BotClientManager.MainBot.Guild.GetUser(Id);

            while (LevelPoints.Levels[LevelPoints.RolelList[Level + 1]] <= Points)
            {
                Level++;
            }
            
            var newRole = BotClientManager.MainBot.Guild.GetRole((ulong)LevelPoints.RolelList[Level]);
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

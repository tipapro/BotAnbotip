using BotAnbotip.Bot.Data.Group;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotAnbotip.Bot.Data.CustomClasses
{
    class LevelInfo
    {
        public static Dictionary<LevelRoleIds, long> Points = new Dictionary<LevelRoleIds, long>
        {
            { LevelRoleIds.Copper1, 0 },
            { LevelRoleIds.Copper2, 200 },
            { LevelRoleIds.Copper3, 550 },
            { LevelRoleIds.Bronze1, 1000 },
            { LevelRoleIds.Bronze2, 2250 },
            { LevelRoleIds.Bronze3, 3500 },
            { LevelRoleIds.Silver1, 5000 },
            { LevelRoleIds.Silver2, 6500 },
            { LevelRoleIds.Silver3, 8000 },
            { LevelRoleIds.Gold1, 10000 },
            { LevelRoleIds.Gold2, 22500 },
            { LevelRoleIds.Gold3, 35000 },
            { LevelRoleIds.Platinum1, 50000 },
            { LevelRoleIds.Platinum2, 65000 },
            { LevelRoleIds.Platinum3, 80000 },
            { LevelRoleIds.Sapphire1, 100000 },
            { LevelRoleIds.Sapphire2, 225000 },
            { LevelRoleIds.Sapphire3, 350000 },
            { LevelRoleIds.Emerald1, 500000 },
            { LevelRoleIds.Emerald2, 650000 },
            { LevelRoleIds.Emerald3, 800000 },
            { LevelRoleIds.Ruby1, 1000000 },
            { LevelRoleIds.Ruby2, 2250000 },
            { LevelRoleIds.Ruby3, 3500000 },
            { LevelRoleIds.Diamond1, 5000000 },
            { LevelRoleIds.Diamond2, 6500000 },
            { LevelRoleIds.Diamond3, 8000000 },
            { LevelRoleIds.Magma1, 10000000 },
            { LevelRoleIds.Magma2, 22500000 },
            { LevelRoleIds.Magma3, 35000000 },
            { LevelRoleIds.Dark_matter1, 50000000 },
            { LevelRoleIds.Dark_matter2, 65000000 },
            { LevelRoleIds.Dark_matter3, 80000000 },
            { LevelRoleIds.Singularity, 100000000 }
        };

        public static LevelRoleIds[] RoleList = 
        {
            LevelRoleIds.Copper1, LevelRoleIds.Copper2, LevelRoleIds.Copper3,
            LevelRoleIds.Bronze1, LevelRoleIds.Bronze2, LevelRoleIds.Bronze3,
            LevelRoleIds.Silver1, LevelRoleIds.Silver2, LevelRoleIds.Silver3,
            LevelRoleIds.Gold1, LevelRoleIds.Gold2, LevelRoleIds.Gold3,
            LevelRoleIds.Platinum1, LevelRoleIds.Platinum2, LevelRoleIds.Platinum3,
            LevelRoleIds.Sapphire1, LevelRoleIds.Sapphire2, LevelRoleIds.Sapphire3,
            LevelRoleIds.Emerald1, LevelRoleIds.Emerald2, LevelRoleIds.Emerald3,
            LevelRoleIds.Ruby1, LevelRoleIds.Ruby2, LevelRoleIds.Ruby3,
            LevelRoleIds.Diamond1, LevelRoleIds.Diamond2, LevelRoleIds.Diamond3,
            LevelRoleIds.Magma1, LevelRoleIds.Magma2, LevelRoleIds.Magma3,
            LevelRoleIds.Dark_matter1, LevelRoleIds.Dark_matter2, LevelRoleIds.Dark_matter3,
            LevelRoleIds.Singularity
        };
    }
}

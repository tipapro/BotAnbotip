using BotAnbotip.Data.Group;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotAnbotip.Data.CustomClasses
{
    class LevelInfo
    {
        public static Dictionary<LevelRoleIds, long> Points = new Dictionary<LevelRoleIds, long>
        {
            { LevelRoleIds.Copper1, 0 },
            { LevelRoleIds.Copper2, 248 },
            { LevelRoleIds.Copper3, 567 },
            { LevelRoleIds.Bronze1, 1135 },
            { LevelRoleIds.Bronze2, 2178 },
            { LevelRoleIds.Bronze3, 3589 },
            { LevelRoleIds.Silver1, 5489 },
            { LevelRoleIds.Silver2, 8603 },
            { LevelRoleIds.Silver3, 11345 },
            { LevelRoleIds.Gold1, 17001 },
            { LevelRoleIds.Gold2, 24521 },
            { LevelRoleIds.Gold3, 34876 },
            { LevelRoleIds.Platinum1, 49053 },
            { LevelRoleIds.Platinum2, 65798 },
            { LevelRoleIds.Platinum3, 81322 },
            { LevelRoleIds.Sapphire1, 105463 },
            { LevelRoleIds.Sapphire2, 133123 },
            { LevelRoleIds.Sapphire3, 179980},
            { LevelRoleIds.Emerald1, 242124 },
            { LevelRoleIds.Emerald2, 328954 },
            { LevelRoleIds.Emerald3, 437989 },
            { LevelRoleIds.Ruby1, 658246 },
            { LevelRoleIds.Ruby2, 917654 },
            { LevelRoleIds.Ruby3, 1198793 },
            { LevelRoleIds.Diamond1, 1895275 },
            { LevelRoleIds.Diamond2, 3222674 },
            { LevelRoleIds.Diamond3, 5194876 },
            { LevelRoleIds.Magma1, 8003467 },
            { LevelRoleIds.Magma2, 11537986 },
            { LevelRoleIds.Magma3, 15909458 },
            { LevelRoleIds.Dark_matter1, 21741347 },
            { LevelRoleIds.Dark_matter2, 30644293 },
            { LevelRoleIds.Dark_matter3, 41107630 },
            { LevelRoleIds.Singularity, 55555555 }
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

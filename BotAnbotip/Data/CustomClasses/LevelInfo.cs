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
            { LevelRoleIds.Copper2, 155 },
            { LevelRoleIds.Copper3, 278 },
            { LevelRoleIds.Bronze1, 418 },
            { LevelRoleIds.Bronze2, 593 },
            { LevelRoleIds.Bronze3, 768 },
            { LevelRoleIds.Silver1, 990 },
            { LevelRoleIds.Silver2, 2342 },
            { LevelRoleIds.Silver3, 4109 },
            { LevelRoleIds.Gold1, 6452 },
            { LevelRoleIds.Gold2, 13989 },
            { LevelRoleIds.Gold3, 23393 },
            { LevelRoleIds.Platinum1, 34384 },
            { LevelRoleIds.Platinum2, 46485 },
            { LevelRoleIds.Platinum3, 60322 },
            { LevelRoleIds.Sapphire1, 88463 },
            { LevelRoleIds.Sapphire2, 133123 },
            { LevelRoleIds.Sapphire3, 199980},
            { LevelRoleIds.Emerald1, 322124 },
            { LevelRoleIds.Emerald2, 438954 },
            { LevelRoleIds.Emerald3, 597989 },
            { LevelRoleIds.Ruby1, 768246 },
            { LevelRoleIds.Ruby2, 987654 },
            { LevelRoleIds.Ruby3, 1298793 },
            { LevelRoleIds.Diamond1, 1995275 },
            { LevelRoleIds.Diamond2, 3522674 },
            { LevelRoleIds.Diamond3, 5194876 },
            { LevelRoleIds.Magma1, 8303467 },
            { LevelRoleIds.Magma2, 11637986 },
            { LevelRoleIds.Magma3, 17909458 },
            { LevelRoleIds.Dark_matter1, 24741347 },
            { LevelRoleIds.Dark_matter2, 31644293 },
            { LevelRoleIds.Dark_matter3, 42107630 },
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

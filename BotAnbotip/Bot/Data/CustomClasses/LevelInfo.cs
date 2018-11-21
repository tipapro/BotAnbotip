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
            { LevelRoleIds.Copper2, 155 },
            { LevelRoleIds.Copper3, 278 },
            { LevelRoleIds.Bronze1, 410 },
            { LevelRoleIds.Bronze2, 590 },
            { LevelRoleIds.Bronze3, 760 },
            { LevelRoleIds.Silver1, 1000 },
            { LevelRoleIds.Silver2, 2400 },
            { LevelRoleIds.Silver3, 4600 },
            { LevelRoleIds.Gold1, 7000 },
            { LevelRoleIds.Gold2, 16000 },
            { LevelRoleIds.Gold3, 27000 },
            { LevelRoleIds.Platinum1, 39000 },
            { LevelRoleIds.Platinum2, 55000 },
            { LevelRoleIds.Platinum3, 61000 },
            { LevelRoleIds.Sapphire1, 89000 },
            { LevelRoleIds.Sapphire2, 140000 },
            { LevelRoleIds.Sapphire3, 201000 },
            { LevelRoleIds.Emerald1, 324000 },
            { LevelRoleIds.Emerald2, 443000 },
            { LevelRoleIds.Emerald3, 600000 },
            { LevelRoleIds.Ruby1, 768000 },
            { LevelRoleIds.Ruby2, 990000 },
            { LevelRoleIds.Ruby3, 1300000 },
            { LevelRoleIds.Diamond1, 2100000 },
            { LevelRoleIds.Diamond2, 3600000 },
            { LevelRoleIds.Diamond3, 5200000 },
            { LevelRoleIds.Magma1, 8400000 },
            { LevelRoleIds.Magma2, 12000000 },
            { LevelRoleIds.Magma3, 19000000 },
            { LevelRoleIds.Dark_matter1, 25000000 },
            { LevelRoleIds.Dark_matter2, 31000000 },
            { LevelRoleIds.Dark_matter3, 43000000 },
            { LevelRoleIds.Singularity, 58000000 }
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

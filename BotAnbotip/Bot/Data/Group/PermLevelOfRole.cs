using System;
using System.Collections.Generic;
using System.Text;

namespace BotAnbotip.Bot.Data.Group
{
    enum PermLevelOfRole : byte
    {              
        everyone = 0,
        Member = 1,
        DELETED_Active_Member = 2,
        Moderator = 3,
        Admin = 4,
        Co_founder = 5,
        Founder = 6
    }
}

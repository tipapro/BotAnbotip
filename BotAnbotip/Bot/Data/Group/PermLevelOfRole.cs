using System;
using System.Collections.Generic;
using System.Text;

namespace BotAnbotip.Bot.Data.Group
{
    enum PermLevelOfRole : byte
    {              
        everyone = 0,
        Участник = 1,
        Активный_Участник = 2,
        Модератор = 3,
        Администратор = 4,
        Заместитель = 5,
        Основатель = 6
    }
}

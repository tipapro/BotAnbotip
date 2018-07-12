using BotAnbotip.Bot.Data.Group;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotAnbotip.Bot.Data.CustomClasses
{
    class Level
    {
        public static Dictionary<LevelRoleIds, long> Points = new Dictionary<LevelRoleIds, long>
        {
            { LevelRoleIds.Медь1, 0 },
            { LevelRoleIds.Медь2, 200 },
            { LevelRoleIds.Медь3, 550 },
            { LevelRoleIds.Бронза1, 1000 },
            { LevelRoleIds.Бронза2, 2250 },
            { LevelRoleIds.Бронза3, 3500 },
            { LevelRoleIds.Серебро1, 5000 },
            { LevelRoleIds.Серебро2, 6500 },
            { LevelRoleIds.Серебро3, 8000 },
            { LevelRoleIds.Золото1, 10000 },
            { LevelRoleIds.Золото2, 22500 },
            { LevelRoleIds.Золото3, 35000 },
            { LevelRoleIds.Платина1, 50000 },
            { LevelRoleIds.Платина2, 65000 },
            { LevelRoleIds.Платина3, 80000 },
            { LevelRoleIds.Сапфир1, 100000 },
            { LevelRoleIds.Сапфир2, 225000 },
            { LevelRoleIds.Сапфир3, 350000 },
            { LevelRoleIds.Изумруд1, 500000 },
            { LevelRoleIds.Изумруд2, 650000 },
            { LevelRoleIds.Изумруд3, 800000 },
            { LevelRoleIds.Рубин1, 1000000 },
            { LevelRoleIds.Рубин2, 2250000 },
            { LevelRoleIds.Рубин3, 3500000 },
            { LevelRoleIds.Алмаз1, 5000000 },
            { LevelRoleIds.Алмаз2, 6500000 },
            { LevelRoleIds.Алмаз3, 8000000 },
            { LevelRoleIds.Магма1, 10000000 },
            { LevelRoleIds.Магма2, 22500000 },
            { LevelRoleIds.Магма3, 35000000 },
            { LevelRoleIds.Тёмная_материя1, 50000000 },
            { LevelRoleIds.Тёмная_материя2, 65000000 },
            { LevelRoleIds.Тёмная_материя3, 80000000 },
            { LevelRoleIds.Сингулярность, 100000000 }
        };

        public static LevelRoleIds[] RoleList = 
        {
            LevelRoleIds.Медь1, LevelRoleIds.Медь2, LevelRoleIds.Медь3,
            LevelRoleIds.Бронза1, LevelRoleIds.Бронза2, LevelRoleIds.Бронза3,
            LevelRoleIds.Серебро1, LevelRoleIds.Серебро2, LevelRoleIds.Серебро3,
            LevelRoleIds.Золото1, LevelRoleIds.Золото2, LevelRoleIds.Золото3,
            LevelRoleIds.Платина1, LevelRoleIds.Платина2, LevelRoleIds.Платина3,
            LevelRoleIds.Сапфир1, LevelRoleIds.Сапфир2, LevelRoleIds.Сапфир3,
            LevelRoleIds.Изумруд1, LevelRoleIds.Изумруд2, LevelRoleIds.Изумруд3,
            LevelRoleIds.Рубин1, LevelRoleIds.Рубин2, LevelRoleIds.Рубин3,
            LevelRoleIds.Алмаз1, LevelRoleIds.Алмаз2, LevelRoleIds.Алмаз3,
            LevelRoleIds.Магма1, LevelRoleIds.Магма2, LevelRoleIds.Магма3,
            LevelRoleIds.Тёмная_материя1, LevelRoleIds.Тёмная_материя2, LevelRoleIds.Тёмная_материя3,
            LevelRoleIds.Сингулярность
        };
    }
}

﻿using BotAnbotip.Data.CustomEnums;
using System;
using System.Collections.Generic;

namespace BotAnbotip.Data.CustomClasses
{
    class MessageTitles
    {
        public static Dictionary<TitleType, string> Titles = new Dictionary<TitleType, string>
        {
            { TitleType.Announcement, ":loudspeaker:Объявление:loudspeaker:" },
            { TitleType.Anonymous, ":spy:Анонимное сообщение:spy:" },
            { TitleType.VipGiveaway, ":gift:Еженедельный розыгрыш VIP роли:gift:" },
            { TitleType.News, ":newspaper:Новость:newspaper:" },
            { TitleType.Voting, ":bar_chart:Голосование:bar_chart:" },
            { TitleType.WantPlay, ":video_game:Приглашение в игру:video_game:" },
            { TitleType.SubscriptionManager, ":envelope_with_arrow:Менеджер подписок:envelope_with_arrow:" },
            { TitleType.UserLevel, ":small_blue_diamond:Уровень пользователя:small_blue_diamond:" },
            { TitleType.ManageRole, ":tools:Доступ к тематическим чатам:tools:"},
            { TitleType.Greeting, ":cyclone:Приветствуем Вас на нашем сервере!:cyclone:"},
            { TitleType.UsersTop, ":top:Топ 10:top:"}
        };

        public static TitleType GetType(string title)
        {
            foreach(var pair in Titles) if (pair.Value == title) return pair.Key;
            throw new Exception("Ошибка при определнии типа заголовка");
        }
    }
}

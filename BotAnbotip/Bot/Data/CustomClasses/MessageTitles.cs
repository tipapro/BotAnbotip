using BotAnbotip.Bot.Data.CustomEnums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BotAnbotip.Bot.Data.CustomClasses
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
            { TitleType.SubscriptionManager, ":envelope_with_arrow:Менеджер подписок:envelope_with_arrow:" }
        };

        public static TitleType GetType(string title)
        {
            foreach(var pair in Titles) if (pair.Value == title) return pair.Key;
            throw new Exception("Ошибка при определнии типа заголовка");
        }
    }
}

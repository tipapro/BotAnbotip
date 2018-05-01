using System;
using System.Collections.Generic;
using System.Text;
using BotAnbotip.Bot.Data.CustomEnums;

namespace BotAnbotip.Bot.Client
{
    class AntiSpam
    {
        private readonly SpamType _spamType;
        private Dictionary<ulong, (DateTimeOffset, long)> _spamCounter;

        public AntiSpam(SpamType type)
        {
            _spamType = type;
            _spamCounter = new Dictionary<ulong, (DateTimeOffset, long)>();
        }

        public bool Check(ulong id)
        {
            if (!_spamCounter.ContainsKey(id))
            {
                _spamCounter.Add(id, (DateTime.Now, 0));
                return false;
            }
            var last = (_spamCounter[id].Item1 - DateTime.Now).Duration();
            if (last > new TimeSpan(0, 10, 0))
            {
                _spamCounter[id] = (DateTime.Now, _spamCounter[id].Item2 + 1);
                return false;
            }
            //double score = last / new TimeSpan(0, 0, 1);
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using BotAnbotip.Bot.Data.CustomEnums;
using BotAnbotip.Bot.OtherModules;

namespace BotAnbotip.Bot.Handlers
{    
    class AntiSpam
    {
        private const long CriticalScore = 1000;
        private const long CostOfOneAction = 100;
        private readonly SpamType _spamType;
        private Dictionary<ulong, (DateTimeOffset, ulong)> _spamCounter;
        //private Dictionary<ulong, (string, int)> _monotonousMessages;   //!!!!!!!!!!!!!!!!!!!!!!!!!!

        public AntiSpam(SpamType type)
        {
            _spamType = type;
            _spamCounter = new Dictionary<ulong, (DateTimeOffset, ulong)>();
        }

        public bool Check(ulong userId, string message = null)
        {
            if (!_spamCounter.ContainsKey(userId))
            {
                _spamCounter.Add(userId, (DateTime.Now, 0));
                return false;
            }
            var fromLast = (_spamCounter[userId].Item1 - DateTime.Now).Duration();
            if (fromLast > new TimeSpan(0, 10, 0))
            {
                _spamCounter[userId] = (DateTime.Now, CostOfOneAction);
                return false;
            }
            double secondsPassed = fromLast.TotalSeconds;
            if (_spamCounter[userId].Item2 < secondsPassed)
            {
                _spamCounter[userId] = (DateTime.Now, CostOfOneAction);
                return false;
            }
            else
            {
                //!!!!!
                //_spamCounter[userId] = (DateTime.Now, _spamCounter[userId].Item2 - (ulong)Math.Round(secondsPassed) + CostOfOneAction);
            }
            if (_spamCounter[userId].Item2 > CriticalScore)
            {
                PunishmentModule.Punish(userId, PunishmentReason.Spam).GetAwaiter().GetResult();
                return true;
            }
            return false;
        }
    }
}

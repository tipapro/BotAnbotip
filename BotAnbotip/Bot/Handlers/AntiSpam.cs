using System;
using System.Collections.Generic;
using System.Text;
using BotAnbotip.Bot.Data.CustomEnums;
using BotAnbotip.Bot.OtherModules;

namespace BotAnbotip.Bot.Handlers
{    
    class AntiSpam
    {
        private readonly static TimeSpan CriticalTimeSpan = new TimeSpan(0, 3, 0);
        private static readonly double CriticalScore = CriticalTimeSpan.TotalSeconds;
        private const long CostOfOneAction = 10;
        private const long CostOfOneSecond = 5;
        private readonly SpamType _spamType;
        private Dictionary<ulong, (DateTimeOffset, double)> _spamCounter;
        //private Dictionary<ulong, (string, int)> _monotonousMessages;   //!!!!!!!!!!!!!!!!!!!!!!!!!!

        public AntiSpam(SpamType type)
        {
            _spamType = type;
            _spamCounter = new Dictionary<ulong, (DateTimeOffset, double)>();
        }

        public bool Check(ulong userId, string message = null)
        {
            DateTime currentTime = DateTime.Now;
            if (!_spamCounter.ContainsKey(userId))
            {
                _spamCounter.Add(userId, (currentTime, 0));
                return false;
            }
            var timePassed = (_spamCounter[userId].Item1 - currentTime).Duration();            
            if (timePassed > new TimeSpan(0, 3, 0))
            {
                _spamCounter[userId] = (currentTime, CostOfOneAction);
                return false;
            }
            else
            {
                double secondsPassed = timePassed.TotalSeconds;
                _spamCounter[userId] = (currentTime, _spamCounter[userId].Item2 + 
                    CostOfOneAction / secondsPassed - Math.Log(1 + CostOfOneSecond * secondsPassed * secondsPassed));
                if (_spamCounter[userId].Item2 < 0) _spamCounter[userId] = (currentTime, 0);
                Console.WriteLine("Антиспам: " + _spamCounter[userId].Item2 + "||" + Math.Log(1 + CostOfOneSecond * secondsPassed * secondsPassed));
            }
            if (_spamCounter[userId].Item2 > CriticalScore)
            {
                Console.WriteLine("Detected");
                PunishmentModule.Punish(userId, PunishmentReason.Spam).GetAwaiter().GetResult();
                return true;
            }
            return false;
        }
    }
}

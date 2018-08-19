using System;
using System.Collections.Generic;
using System.Text;

namespace BotAnbotip.Bot.OtherModules
{
    static class OtherMethods
    {
        private const char FilledChar = '▰';
        private const char EmptyChar = '▱';
        private const int Length = 20;


        public static string GenerateTextProgressBar(float filled, float total)
        {
            int amountOfFilledChar = (int)Math.Round(filled * Length / total);
            int amountOfEmptyChar = Length - amountOfFilledChar;
            return new string(FilledChar, amountOfFilledChar) + new string(EmptyChar, amountOfEmptyChar);
        }
    }
}

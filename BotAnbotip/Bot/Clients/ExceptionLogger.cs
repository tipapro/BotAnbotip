using System;
using System.Collections.Generic;
using System.Text;

namespace BotAnbotip.Bot.Clients
{
    class ExceptionLogger
    {
        private int counter;
        public void Log(Exception ex, string text = "")
        {
            if (ex != null)
            {
                if ((text != "") && (counter == 0)) text += ": ";
                
                Console.WriteLine(DateTime.Now + "  " + ex.Source + ": " + text + "Степень вложенности - " + counter++ + ": " + ex.Message);
                Log(ex.InnerException, text);
            }
            counter = 0;
        }
    }
}

using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.Data.Group;
using Discord;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.CyclicActions
{
    class DailyMessagesCyclicAction : CyclicActionBase
    {
        public DailyMessagesCyclicAction(BotClientBase botClient, string errorMessage, string startMessage, string stopMessage) : 
            base(botClient, errorMessage, startMessage, stopMessage)
        {
        }

        protected override async Task Cycle(CancellationToken token)
        {
            await Task.CompletedTask;
            /*while (IsStarted)
            {
                JObject obj;
                string url;
                var embedBuilder = new EmbedBuilder()
                    .WithColor(Color.DarkGrey);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                    "https://" + "www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=en-US");
                while (true)
                {
                    HttpWebResponse resp = (HttpWebResponse)await request.GetResponseAsync();
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    {
                        await Task.Delay(100, token);
                        try { obj = JObject.Parse(sr.ReadToEnd().Trim()); }
                        catch (Exception) { continue; }
                        url = "https://" + "www.bing.com" + (string)obj["images"][0]["url"];
                        break;
                    }
                }
                embedBuilder.WithImageUrl(url);
                await ((ITextChannel)BotClientManager.MainBot.Guild.GetChannel((ulong)ChannelIds.test)).SendMessageAsync("", false, embedBuilder.Build());
                await Task.Delay(new TimeSpan(0, 10, 0), token);
            }*/
        }
    }
}

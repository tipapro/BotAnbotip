using BotAnbotip.Bot.Clients;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.CyclicActions
{
    class CyclicActionBase
    {
        private CancellationTokenSource _cts;
        private Func<CancellationToken, Task> _cycleMethod;

        public string ErrorMessage;
        public string StartMessage;
        public string StopMessage;

        public bool IsStarted { get; private set; }

        public BotClientBase BotClient { get; }

        public CyclicActionBase(BotClientBase botClient, string errorMessage, string startMessage, string stopMessage)
        {
            BotClient = botClient;
            ErrorMessage = errorMessage;
            StartMessage = startMessage;
            StopMessage = stopMessage;
            _cycleMethod = Cycle;
        }

        protected virtual Task Cycle(CancellationToken token) => Task.CompletedTask;

        public async void Run()
        {
            if (IsStarted)
            {
                await BotClient.Log(new LogMessage(LogSeverity.Info, "", ErrorMessage + ": Действие уже запущено"));
                return;
            }
            while (!BotClient.IsLoaded) await Task.Delay(1000);
            _cts = new CancellationTokenSource();
            IsStarted = true;
            RunCycle();
        }

        private async void RunCycle()
        {
            try
            {
                await BotClient.Log(new LogMessage(LogSeverity.Info, "", StartMessage));
                await _cycleMethod.Invoke(_cts.Token);
            }
            catch (Exception ex)
            {
                IsStarted = false;
                if ((ex is OperationCanceledException) && (_cts != null) && (((OperationCanceledException)ex).CancellationToken == _cts.Token))
                    new ExceptionLogger().Log(ex, StopMessage);
                else
                {
                    new ExceptionLogger().Log(ex, ErrorMessage);
                    Run();
                }

            }
        }

        public void Stop()
        {
            if (IsStarted && (_cts != null)) _cts.Cancel();
        }
    }
}

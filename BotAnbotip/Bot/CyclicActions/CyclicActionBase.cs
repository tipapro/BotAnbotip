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
        private bool _isStarted;
        private BotClientBase _botClient;
        private CancellationTokenSource _cts;
        protected Func<CancellationToken, Task> _cycleMethod;

        public string ErrorMessage;
        public string StartMessage;
        public string StopMessage;

        public bool IsStarted => _isStarted;
        public BotClientBase BotClient => _botClient;
        public Func<CancellationToken, Task> CycleMethod => _cycleMethod;

        public CyclicActionBase(BotClientBase botClient, string errorMessage, string startMessage, string stopMessage)
        {
            _botClient = botClient;
            ErrorMessage = errorMessage;
            StartMessage = startMessage;
            StopMessage = stopMessage;
        }

        public async void Run()
        {
            if (IsStarted) return;
            while (!BotClient.IsLoaded) await Task.Delay(1000);
            _cts = new CancellationTokenSource();
            _isStarted = true;
            RunCycle();
        }

        private async void RunCycle()
        {
            try
            {
                await BotClient.Log(new LogMessage(LogSeverity.Info, "", "Бот авторизован"));
                await CycleMethod.Invoke(_cts.Token);
            }
            catch (Exception ex)
            {
                _isStarted = false;
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

using BotAnbotip.Clients;
using Discord;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BotAnbotip.Services
{
    class ServiceBase
    {
        protected readonly ILogger<ServiceBase> _logger;
        private CancellationTokenSource _cts;

        public string ServiceName { get; }
        public bool IsStarted { get; private set; }
        public ClientBase BotClient { get; }

        public ServiceBase(ClientBase botClient, string serviceName, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ServiceBase>();
            ServiceName = serviceName;
            BotClient = botClient;
        }

        protected virtual Task Cycle(CancellationToken token) => Task.CompletedTask;

        public void Run()
        {
            if (IsStarted)
            {
                _logger.LogWarning("Service \"{ServiceName}\" has already been launched", ServiceName);
                return;
            }
            IsStarted = true;
            _cts = new CancellationTokenSource();
            RunCycle();
        }

        private async void RunCycle()
        {
            try
            {
                _logger.LogInformation("Service \"{ServiceName}\" is launched", ServiceName);
                await Task.Run(() => Cycle(_cts.Token));
            }
            catch (Exception ex)
            {
                IsStarted = false;
                if ((ex as OperationCanceledException)?.CancellationToken == _cts.Token)
                {
                    _logger.LogInformation(ex as OperationCanceledException,
                        "Service \"{ServiceName}\" was stopped by CancellationToken: {_cts.Token}", ServiceName, _cts.Token);
                }
                else
                {
                    _logger.LogError(ex, "Service \"{ServiceName}\" was crashed", ServiceName);
                    Run();  //!!!!!!!!Event
                }
            }
        }

        public void Stop()
        {
            if (IsStarted && (_cts != null)) _cts.Cancel();
        }
    }
}

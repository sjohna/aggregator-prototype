using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace aggregator_server
{
    public class Poller
    {
        private static readonly ILog configLog = LogManager.GetLogger($"Config.{typeof(Poller)}");
        private static readonly ILog log = LogManager.GetLogger(typeof(Poller));

        public int PollIntervalMS { get; private set; }

        private CancellationTokenSource m_cancelTokenSource;

        private CancellationToken CancelToken => m_cancelTokenSource.Token;

        public Poller(int pollIntervalMs)
        {
            this.PollIntervalMS = pollIntervalMs;

            m_cancelTokenSource = new CancellationTokenSource();
        }

        public async Task DoPollingLoop()
        {
            configLog.Info("Polling loop started.");

            Task intervalTask = Task.CompletedTask;   // do first poll immediately

            try
            {
                while (true)
                {
                    await intervalTask;
                    intervalTask = Task.Delay(PollIntervalMS, CancelToken);

                    log.Info("Checking for updates.");
                }
            }
            catch (OperationCanceledException)
            {
                log.Info("Polling cancelled.");
            }

            configLog.Info("Polling loop ended.");
        }

        public void CancelPolling()
        {
            m_cancelTokenSource.Cancel();
        }
    }
}

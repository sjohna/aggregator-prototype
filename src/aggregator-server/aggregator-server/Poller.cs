using log4net;
using NodaTime;
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

        private IPollConfigurationRepository repository;

        public Poller(int pollIntervalMs, IPollConfigurationRepository repository)
        {
            this.PollIntervalMS = pollIntervalMs;
            this.repository = repository;

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

                    Instant pollTime = NodaTime.SystemClock.Instance.GetCurrentInstant();

                    foreach (var pollConfiguration in repository.GetConfigurations())
                    {
                        bool doPoll = false;

                        if (pollConfiguration.LastPollInformation == null)
                        {
                            log.Info($"Configuration check: Configuration {pollConfiguration.ID} ({pollConfiguration.URL}) has never been polled.");
                            doPoll = true;
                        }
                        else
                        {
                            var timeSinceLastPoll = pollTime.Minus(pollConfiguration.LastPollInformation.PolledTime);
                            if (timeSinceLastPoll >= Duration.FromMinutes(pollConfiguration.PollIntervalMinutes))
                            {
                                log.Info($"Configuration check: Configuration {pollConfiguration.ID} ({pollConfiguration.URL}), last polled at {pollConfiguration.LastPollInformation.PolledTime}, will be polled again.");
                                doPoll = true;
                            }
                        }

                        if (doPoll)
                        {
                            log.Info($"Polling configuration {pollConfiguration.ID} ({pollConfiguration.URL}).");

                            var LastPollInformation = new Models.PollingInformation() { Successful = true, PolledTime = pollTime };

                            repository.SetConfigurationLastPollInformation(pollConfiguration.ID, LastPollInformation);
                        }
                    }
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

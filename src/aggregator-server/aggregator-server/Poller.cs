using aggregator_server.Models;
using log4net;
using NodaTime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace aggregator_server
{
    public class Poller
    {
        private static readonly ILog configLog = LogManager.GetLogger($"Config.{typeof(Poller)}");
        private static readonly ILog log = LogManager.GetLogger(typeof(Poller));

        public int PollIntervalMS { get; private set; }

        private CancellationTokenSource m_cancelTokenSource;

        private CancellationToken CancelToken => m_cancelTokenSource.Token;

        private IPollConfigurationRepository configurationRepository;
        private IDocumentRepository documentRepository;

        public Poller(int pollIntervalMs, IPollConfigurationRepository configurationRepository, IDocumentRepository documentRepository)
        {
            this.PollIntervalMS = pollIntervalMs;
            this.configurationRepository = configurationRepository;
            this.documentRepository = documentRepository;

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
                    // TODO: better logging here, maybe? Should I explicitly log when a configuration is skipped due to being inactive? Maybe at the Debug level...
                    await intervalTask;
                    intervalTask = Task.Delay(PollIntervalMS, CancelToken);

                    log.Debug("Checking for updates.");

                    Instant pollTime = NodaTime.SystemClock.Instance.GetCurrentInstant();

                    foreach (var pollConfiguration in configurationRepository.GetConfigurations())
                    {
                        bool doPoll = false;

                        if (!pollConfiguration.Active)
                        {
                            doPoll = false;
                        }
                        else if (pollConfiguration.LastPollInformation == null)
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

                            // actually do poll here
                            PollFeed(pollConfiguration.URL);

                            var LastPollInformation = new Models.PollingInformation() { Successful = true, PolledTime = pollTime };

                            configurationRepository.SetConfigurationLastPollInformation(pollConfiguration.ID, LastPollInformation);
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

        void PollFeed(string feedUri)
        {
            // TODO: figure out automated test strategy for polling logic

            var pollTime = DateTime.Now;

            log.Info($"At {pollTime}, polling {feedUri}");

            using (var client = new WebClient())
            {
                string text = client.DownloadString(feedUri);
                byte[] bytes = Encoding.UTF8.GetBytes(text);

                using (var inputStream = new MemoryStream(bytes))
                using (var reader = XmlReader.Create(inputStream))
                {
                    var feed = SyndicationFeed.Load(reader);
                    log.Info($"Poll response: {feed.Items.Count()} items, {bytes.Length} bytes");

                    foreach (var post in feed.Items)
                    {
                        var sourceID = post.Id;

                        var docs = documentRepository.FindBySourceID(sourceID);

                        if (docs.Count() == 0)
                        {
                            var doc = new Document()
                            {
                                Content = (post.Content as TextSyndicationContent)?.Text,   // TODO: validation here
                                PublishTime = Instant.FromDateTimeOffset(post.PublishDate), // TODO: test this...
                                SourceID = sourceID,
                                SourceLink = post.Links.FirstOrDefault().Uri.ToString(),   // TODO: validate this, and maybe change the type
                                Title = WebUtility.HtmlDecode(post.Title.Text), // TODO: do this in one place so that it isn't duplicated with the update...
                                UpdateTime = Instant.FromDateTimeOffset(post.LastUpdatedTime)
                            };

                            documentRepository.AddDocument(doc);

                            log.Info("Added new document:");
                            LogDocument(doc);
                        }
                        else if (docs.Count() == 1)
                        {
                            // TODO: Race condition...
                            var matchingDoc = docs.First();

                            var updateTime = Instant.FromDateTimeOffset(post.LastUpdatedTime);

                            if (matchingDoc.UpdateTime < updateTime)
                            {
                                matchingDoc.Content = (post.Content as TextSyndicationContent)?.Text;   // TODO: validation here
                                matchingDoc.PublishTime = Instant.FromDateTimeOffset(post.PublishDate); // TODO: test this...
                                matchingDoc.SourceID = sourceID;
                                matchingDoc.SourceLink = post.Links.FirstOrDefault().Uri.ToString();   // TODO: validate this, and maybe change the type
                                matchingDoc.Title = WebUtility.HtmlDecode(post.Title.Text);
                                matchingDoc.UpdateTime = Instant.FromDateTimeOffset(post.LastUpdatedTime);

                                documentRepository.UpdateDocument(matchingDoc);

                                log.Info($"Updated document {matchingDoc.ID}:");
                                LogDocument(matchingDoc);
                            }
                            else
                            {
                                log.Debug($"Document {matchingDoc.ID} ({matchingDoc.SourceID}) present in feed, but not updated.");
                            }
                        }
                    }
                }
            }

            Console.WriteLine();
        }

        void LogDocument(Document doc)
        {
            log.Info($"Title: {doc.Title}");
            log.Info($"Source ID: {doc.SourceID}");
            log.Info($"Source Link: {doc.SourceLink}");
            log.Info($"Publish Time: {doc.PublishTime}");
            log.Info($"Update Time: {doc.UpdateTime}");
            log.Info($"Content length: {doc.Content.Length}");
        }

        public void CancelPolling()
        {
            m_cancelTokenSource.Cancel();
        }
    }
}

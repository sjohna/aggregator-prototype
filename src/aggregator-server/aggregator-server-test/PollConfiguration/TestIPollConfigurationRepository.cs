using aggregator_server;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using aggregator_server.Models;
using NodaTime;
using System;
using aggregator_server.Exceptions;

namespace aggregator_server_test
{
    public abstract class TestIPollConfigurationRepository
    {
        protected IPollConfigurationRepository repository;

        private PollConfiguration AddConfiguration(string url, int pollIntervalMinutes, bool active)
        {
            var createEvent = new CreatePollConfigurationEvent(Guid.NewGuid(), pollIntervalMinutes, url, active);
            repository.ApplyEvent(createEvent);

            return repository.GetConfiguration(createEvent.AffectedEntityID);
        }

        [Test]
        public void InitialRepositoryState()
        {
            Assert.AreEqual(0, repository.GetConfigurations().Count());
        }

        [Test]
        public void GetConfigurationInEmptyRepository()
        {
            Assert.Throws<RepositoryItemNotFoundException>(() => repository.GetConfiguration(Guid.NewGuid()));
        }

        [Test]
        public void GetAbsentConfigurationInNonEmptyRepository()
        {
            var createEvent = new CreatePollConfigurationEvent(Guid.NewGuid(), 7, "test1", true);
            repository.ApplyEvent(createEvent);

            Assert.Throws<RepositoryItemNotFoundException>(() => repository.GetConfiguration(Guid.NewGuid()));
        }

        [Test]
        public void SetConfigurationLastPollInformation()
        {
            var configuration = AddConfiguration("test1", 7, true);

            var polledTime = Instant.FromUnixTimeSeconds(1000000000);
            var successful = true;

            var updatedConfiguration = repository.SetConfigurationLastPollInformation(configuration.ID,
                new PollingInformation()
                {
                    PolledTime = polledTime,
                    Successful = successful
                });

            Assert.AreEqual(configuration.ID, updatedConfiguration.ID);
            Assert.AreEqual(configuration.URL, updatedConfiguration.URL);
            Assert.AreEqual(configuration.PollIntervalMinutes, updatedConfiguration.PollIntervalMinutes);
            Assert.AreEqual(polledTime, updatedConfiguration.LastPollInformation.PolledTime);
            Assert.AreEqual(successful, updatedConfiguration.LastPollInformation.Successful);

            var gottenConfiguration = repository.GetConfiguration(configuration.ID);    // test that updated LastPollConfiguration is present when getting the updated configuration

            Assert.AreEqual(configuration.ID, gottenConfiguration.ID);
            Assert.AreEqual(configuration.URL, gottenConfiguration.URL);
            Assert.AreEqual(configuration.PollIntervalMinutes, gottenConfiguration.PollIntervalMinutes);
            Assert.AreEqual(polledTime, gottenConfiguration.LastPollInformation.PolledTime);
            Assert.AreEqual(successful, gottenConfiguration.LastPollInformation.Successful);
        }

        [Test]
        public void SetAbsentConfigurationLastPollInformationInNonEmptyRepository()
        {
            var configuration = AddConfiguration("test1", 7, true);

            var polledTime = Instant.FromUnixTimeSeconds(1000000000);
            var successful = true;

            var pollInfo = new PollingInformation()
            {
                PolledTime = polledTime,
                Successful = successful
            };

            Assert.Throws<RepositoryItemNotFoundException>(() => repository.SetConfigurationLastPollInformation(Guid.NewGuid(), pollInfo));
        }

        [Test]
        public void AddConfigurationWithDuplicateURL()
        {
            AddConfiguration("test", 7, true);
            Assert.Throws<RepositoryConflictException>(() => AddConfiguration("test", 2, false));
        }

        [Test]
        public void UpdateConfigurationDoesNotAffectLastPollInformation()
        {
            var configuration = AddConfiguration("test", 7, true);

            var polledTime = Instant.FromUnixTimeSeconds(1000000000);
            var successful = true;

            var pollInfo = new PollingInformation()
            {
                PolledTime = polledTime,
                Successful = successful
            };

            repository.SetConfigurationLastPollInformation(configuration.ID, pollInfo);
            repository.UpdateConfiguration(configuration);

            var gottenConfiguration = repository.GetConfiguration(configuration.ID);

            Assert.AreEqual(polledTime, gottenConfiguration.LastPollInformation.PolledTime);
            Assert.AreEqual(successful, gottenConfiguration.LastPollInformation.Successful);
        }
    }
}

using aggregator_server;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using aggregator_server.Models;
using NodaTime;
using System;

namespace aggregator_server_test
{
    public abstract class TestIPollConfigurationRepository
    {
        protected IPollConfigurationRepository repository;

        [Test]
        public void InitialRepositoryState()
        {
            Assert.AreEqual(0, repository.GetConfigurations().Count());
        }

        [Test]
        public void AddOnePollConfiguration()
        {
            repository.AddConfiguration("test", 7);

            Assert.AreEqual(1, repository.GetConfigurations().Count());

            var configuration = repository.GetConfigurations().First();

            Assert.AreEqual("test", configuration.URL);
            Assert.AreEqual(7, configuration.PollIntervalMinutes);
            Assert.IsNull(configuration.LastPollInformation);
        }

        [Test]
        public void TwoAddedConfigurationsHaveDistinctIDs()
        {
            repository.AddConfiguration("test1", 7);
            repository.AddConfiguration("test2", 7);

            var configurations = repository.GetConfigurations().ToList();

            Assert.AreNotEqual(configurations[0].ID, configurations[1].ID);
        }

        [Test]
        public void GetPresentConfigurationByID()
        {
            var addedConfiguration = repository.AddConfiguration("test1", 7);
            var gottenConfiguration = repository.GetConfigurationByID(addedConfiguration.ID);

            Assert.AreEqual(addedConfiguration.ID, gottenConfiguration.ID);
            Assert.AreEqual(addedConfiguration.URL, gottenConfiguration.URL);
            Assert.AreEqual(addedConfiguration.PollIntervalMinutes, gottenConfiguration.PollIntervalMinutes);
        }

        [Test]
        public void GetConfigurationByIDInEmptyRepository()
        {
            Assert.Throws<Exception>(() => repository.GetConfigurationByID(1));
        }

        [Test]
        public void GetAbsentConfigurationByIDInNonEmptyRepository()
        {
            var addedConfiguration = repository.AddConfiguration("test1", 7);

            Assert.Throws<Exception>(() => repository.GetConfigurationByID(addedConfiguration.ID + 1));
        }

        [Test]
        public void SetConfigurationLastPollInformation()
        {
            var configuration = repository.AddConfiguration("test1", 7);

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
        }

        [Test]
        public void AddConfigurationWithDuplicateURL()
        {
            repository.AddConfiguration("test", 7);
            Assert.Throws<RepositoryException>(() => repository.AddConfiguration("test", 2));
        }
    }
}

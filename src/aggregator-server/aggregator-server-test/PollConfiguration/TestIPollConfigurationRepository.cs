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

        [Test]
        public void InitialRepositoryState()
        {
            Assert.AreEqual(0, repository.GetConfigurations().Count());
        }

        [Test]
        public void AddOnePollConfiguration()
        {
            repository.AddConfiguration("test", 7, true);

            Assert.AreEqual(1, repository.GetConfigurations().Count());

            var configuration = repository.GetConfigurations().First();

            Assert.AreEqual("test", configuration.URL);
            Assert.AreEqual(7, configuration.PollIntervalMinutes);
            Assert.IsTrue(configuration.Active);
            Assert.IsNull(configuration.LastPollInformation);
        }

        [Test]
        public void AddOneInactiveConfiguration()
        {
            repository.AddConfiguration("test", 7, false);

            Assert.AreEqual(1, repository.GetConfigurations().Count());

            var configuration = repository.GetConfigurations().First();

            Assert.AreEqual("test", configuration.URL);
            Assert.AreEqual(7, configuration.PollIntervalMinutes);
            Assert.IsFalse(configuration.Active);
            Assert.IsNull(configuration.LastPollInformation);
        }

        [Test]
        public void TwoAddedConfigurationsHaveDistinctIDs()
        {
            repository.AddConfiguration("test1", 7, true);
            repository.AddConfiguration("test2", 7, true);

            var configurations = repository.GetConfigurations().ToList();

            Assert.AreNotEqual(configurations[0].ID, configurations[1].ID);
        }

        [Test]
        public void GetPresentConfiguration()
        {
            var addedConfiguration = repository.AddConfiguration("test1", 7, true);
            var gottenConfiguration = repository.GetConfiguration(addedConfiguration.ID);

            addedConfiguration.AssertEqualTo(gottenConfiguration);
        }

        [Test]
        public void GetConfigurationInEmptyRepository()
        {
            Assert.Throws<RepositoryItemNotFoundException>(() => repository.GetConfiguration(1));
        }

        [Test]
        public void GetAbsentConfigurationInNonEmptyRepository()
        {
            var addedConfiguration = repository.AddConfiguration("test1", 7, true);

            Assert.Throws<RepositoryItemNotFoundException>(() => repository.GetConfiguration(addedConfiguration.ID + 1));
        }

        [Test]
        public void SetConfigurationLastPollInformation()
        {
            var configuration = repository.AddConfiguration("test1", 7, true);

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

            var gottenConfiguration = repository.GetConfiguration(configuration.ID);    // test that updated LAstPollConfiguration is present when getting the updated configuration

            Assert.AreEqual(configuration.ID, gottenConfiguration.ID);
            Assert.AreEqual(configuration.URL, gottenConfiguration.URL);
            Assert.AreEqual(configuration.PollIntervalMinutes, gottenConfiguration.PollIntervalMinutes);
            Assert.AreEqual(polledTime, gottenConfiguration.LastPollInformation.PolledTime);
            Assert.AreEqual(successful, gottenConfiguration.LastPollInformation.Successful);
        }

        [Test]
        public void SetAbsentConfigurationLastPollInformationInNonEmptyRepository()
        {
            var configuration = repository.AddConfiguration("test1", 7, true);

            var polledTime = Instant.FromUnixTimeSeconds(1000000000);
            var successful = true;

            var pollInfo = new PollingInformation()
            {
                PolledTime = polledTime,
                Successful = successful
            };

            Assert.Throws<RepositoryItemNotFoundException>(() => repository.SetConfigurationLastPollInformation(configuration.ID + 1, pollInfo));
        }

        [Test]
        public void AddConfigurationWithDuplicateURL()
        {
            repository.AddConfiguration("test", 7, true);
            Assert.Throws<RepositoryConflictException>(() => repository.AddConfiguration("test", 2, false));
        }

        [Test]
        public void DeleteOnlyConfiguration()
        {
            var configuration = repository.AddConfiguration("test", 7, true);
            repository.DeleteConfiguration(configuration.ID);
            Assert.AreEqual(0, repository.GetConfigurations().Count());
        }

        [Test]
        public void DeleteOneOfTwoConfigurations()
        {
            var config1 = repository.AddConfiguration("test", 7, true);
            var config2 = repository.AddConfiguration("test2", 9, true);

            repository.DeleteConfiguration(config1.ID);

            Assert.AreEqual(1, repository.GetConfigurations().Count());

            var configInRepository = repository.GetConfigurations().First();

            config2.AssertEqualTo(configInRepository);
        }

        [Test]
        public void DeleteInEmptyRepository()
        {
            Assert.Throws<RepositoryItemNotFoundException>(() => repository.DeleteConfiguration(1));
        }

        [Test]
        public void DeleteInvalidConfigurationInNonEmptyRepository()
        {
            var configuration = repository.AddConfiguration("test", 7, true);

            Assert.Throws<RepositoryItemNotFoundException>(() => repository.DeleteConfiguration(configuration.ID - 1));

            Assert.AreEqual(1, repository.GetConfigurations().Count());

            var configurationInRepository = repository.GetConfigurations().First();

            configuration.AssertEqualTo(configurationInRepository);
        }

        [Test]
        public void UpdatePollConfigurationParameters()
        {
            var configuration = repository.AddConfiguration("test", 7, true);

            configuration.Active = false;
            configuration.PollIntervalMinutes = 10;

            repository.UpdateConfiguration(configuration);

            var gottenConfiguration = repository.GetConfiguration(configuration.ID);

            Assert.IsFalse(gottenConfiguration.Active);
            Assert.AreEqual(10, gottenConfiguration.PollIntervalMinutes);
            Assert.AreEqual("test", gottenConfiguration.URL);
            Assert.IsNull(gottenConfiguration.LastPollInformation);
        }

        [Test]
        public void UpdateDeletedPollConfiguration()
        {
            var configuration = repository.AddConfiguration("test", 7, true);
            repository.DeleteConfiguration(configuration.ID);

            configuration.PollIntervalMinutes = 10;

            Assert.Throws<RepositoryItemNotFoundException>(() => repository.UpdateConfiguration(configuration));
        }

        [Test]
        public void UpdatePollConfigurationWithNoChanges()
        {
            var configuration = repository.AddConfiguration("test", 7, true);

            repository.UpdateConfiguration(configuration);

            var gottenConfiguration = repository.GetConfiguration(configuration.ID);

            configuration.AssertEqualTo(gottenConfiguration);
        }

        [Test]
        public void UpdateConfigurationDoesNotAffectLastPollInformation()
        {
            var configuration = repository.AddConfiguration("test", 7, true);

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

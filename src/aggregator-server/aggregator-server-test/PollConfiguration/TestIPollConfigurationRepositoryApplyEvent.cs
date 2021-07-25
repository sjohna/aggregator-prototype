using aggregator_server;
using aggregator_server.Exceptions;
using aggregator_server.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aggregator_server_test
{
    public abstract class TestIPollConfigurationRepositoryApplyEvent
    {
        protected IPollConfigurationRepository repository;

        [Test]
        public void ApplyOneCreateEvent()
        {
            var e = new CreatePollConfigurationEvent(Guid.NewGuid(), 7, "test", true);
            repository.ApplyEvent(e);

            Assert.AreEqual(1, repository.GetConfigurations().Count());

            var configuration = repository.GetConfigurations().First();

            Assert.AreEqual("test", configuration.URL);
            Assert.AreEqual(7, configuration.PollIntervalMinutes);
            Assert.IsTrue(configuration.Active);
            Assert.IsNull(configuration.LastPollInformation);
        }

        [Test]
        public void ApplyTwoCreateEvents()
        {
            var e1 = new CreatePollConfigurationEvent(Guid.NewGuid(), 7, "test", true);
            var e2 = new CreatePollConfigurationEvent(Guid.NewGuid(), 10, "test2", false);
            repository.ApplyEvent(e1);
            repository.ApplyEvent(e2);

            Assert.AreEqual(2, repository.GetConfigurations().Count());

            var configuration1 = repository.GetConfiguration(e1.AffectedEntityID);

            Assert.AreEqual("test", configuration1.URL);
            Assert.AreEqual(7, configuration1.PollIntervalMinutes);
            Assert.AreEqual(true, configuration1.Active);
            Assert.IsNull(configuration1.LastPollInformation);

            var configuration2 = repository.GetConfiguration(e2.AffectedEntityID);

            Assert.AreEqual("test2", configuration2.URL);
            Assert.AreEqual(10, configuration2.PollIntervalMinutes);
            Assert.AreEqual(false, configuration2.Active);
            Assert.IsNull(configuration2.LastPollInformation);
        }

        [Test]
        public void ApplyCreateEventForConfigurationWithDuplicateURL()
        {
            var e1 = new CreatePollConfigurationEvent(Guid.NewGuid(), 7, "test", true);
            var e2 = new CreatePollConfigurationEvent(Guid.NewGuid(), 10, "test", false);
            repository.ApplyEvent(e1);

            Assert.Throws<RepositoryConflictException>(() => repository.ApplyEvent(e2));

            Assert.AreEqual(1, repository.GetConfigurations().Count());

            var configuration = repository.GetConfiguration(e1.AffectedEntityID);

            Assert.AreEqual("test", configuration.URL);
            Assert.AreEqual(7, configuration.PollIntervalMinutes);
            Assert.AreEqual(true, configuration.Active);
            Assert.IsNull(configuration.LastPollInformation);
        }

        [Test]
        public void ApplyDeleteEventForOnlyConfiguration()
        {
            var configID = Guid.NewGuid();

            var createEvent = new CreatePollConfigurationEvent(configID, 7, "test", true);
            repository.ApplyEvent(createEvent);

            var deleteEvent = new DeletePollConfigurationEvent(configID);
            repository.ApplyEvent(deleteEvent);

            Assert.AreEqual(0, repository.GetConfigurations().Count());
        }

        [Test]
        public void ApplyDeleteEventForOneOfTwoConfigurations()
        {
            var e1 = new CreatePollConfigurationEvent(Guid.NewGuid(), 7, "test", true);
            var e2 = new CreatePollConfigurationEvent(Guid.NewGuid(), 10, "test2", false);
            repository.ApplyEvent(e1);
            repository.ApplyEvent(e2);

            var deleteEvent = new DeletePollConfigurationEvent(e1.AffectedEntityID);
            repository.ApplyEvent(deleteEvent);

            Assert.AreEqual(1, repository.GetConfigurations().Count());

            var configuration = repository.GetConfiguration(e2.AffectedEntityID);

            Assert.AreEqual("test2", configuration.URL);
            Assert.AreEqual(10, configuration.PollIntervalMinutes);
            Assert.AreEqual(false, configuration.Active);
            Assert.IsNull(configuration.LastPollInformation);
        }

        [Test]
        public void ApplyDeleteEventInEmptyRepository()
        {
            Assert.Throws<RepositoryItemNotFoundException>(() => repository.ApplyEvent(new DeletePollConfigurationEvent(Guid.NewGuid())));
        }

        [Test]
        public void ApplyDeleteEventForInvalidConfigurationInNonEmptyRepository()
        {
            var configID = Guid.NewGuid();

            var createEvent = new CreatePollConfigurationEvent(configID, 7, "test", true);
            repository.ApplyEvent(createEvent);

            var deleteEvent = new DeletePollConfigurationEvent(Guid.NewGuid());
            Assert.Throws<RepositoryItemNotFoundException>(() => repository.ApplyEvent(deleteEvent));

            Assert.AreEqual(1, repository.GetConfigurations().Count());

            var configuration = repository.GetConfiguration(configID);

            Assert.AreEqual("test", configuration.URL);
            Assert.AreEqual(7, configuration.PollIntervalMinutes);
            Assert.AreEqual(true, configuration.Active);
            Assert.IsNull(configuration.LastPollInformation);
        }

        [Test]
        public void ApplyUpdateEventForPresentConfiguration()
        {
            var createEvent = new CreatePollConfigurationEvent(Guid.NewGuid(), 7, "test", true);
            repository.ApplyEvent(createEvent);

            var updateEvent = new UpdatePollConfigurationEvent(createEvent.AffectedEntityID);
            updateEvent.Active = false;
            updateEvent.PollIntervalMinutes = 10;

            repository.ApplyEvent(updateEvent);

            var configuration = repository.GetConfiguration(createEvent.AffectedEntityID);

            Assert.AreEqual("test", configuration.URL);
            Assert.AreEqual(10, configuration.PollIntervalMinutes);
            Assert.AreEqual(false, configuration.Active);
            Assert.IsNull(configuration.LastPollInformation);
        }

        [Test]
        public void ApplyUpdateEventInEmptyRepository()
        {
            var updateEvent = new UpdatePollConfigurationEvent(Guid.NewGuid());
            updateEvent.Active = false;
            updateEvent.PollIntervalMinutes = 10;

            Assert.Throws<RepositoryItemNotFoundException>(() => repository.ApplyEvent(updateEvent));
        }
    }
}

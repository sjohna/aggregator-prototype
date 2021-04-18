using aggregator_server;
using aggregator_server.Exceptions;
using aggregator_server.Models;
using aggregator_server.Repositories;
using LiteDB;
using NodaTime;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aggregator_server_test
{
    [TestFixture]
    class TestLiteDBActionEventRepository
    {
        private LiteDatabase database;
        private LiteDBActionEventRepository repository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            LiteDBFunctions.DoLiteDBGlobalSetUp();
        }

        [SetUp]
        public void SetUp()
        {
            database = new LiteDatabase(":memory:");
            repository = new LiteDBActionEventRepository(database);
        }

        [TearDown]
        public void TearDown()
        {
            database?.Dispose();
            repository?.Dispose();
        }

        [Test]
        public void AddOneActionWithNoEvents()
        {
            var action = new AggregatorAction(Guid.NewGuid(), "Action description", AggregatorAction.ActionOrigin.User, Instant.FromUnixTimeMilliseconds(1000000));

            repository.InsertAction(action);

            var actionInRepository = repository.GetActions().First();

            Assert.AreEqual(action.ActionTime, actionInRepository.ActionTime);
            Assert.AreEqual(action.Description, actionInRepository.Description);
            Assert.AreEqual(action.ID, actionInRepository.ID);
            Assert.AreEqual(action.Origin, actionInRepository.Origin);
            Assert.IsNotNull(action.Events);
            Assert.AreEqual(0, action.Events.Count);
        }

        [Test]
        public void AddActionWithDuplicateIDThrowsException()
        {
            var action = new AggregatorAction(Guid.NewGuid(), "Action description", AggregatorAction.ActionOrigin.User, Instant.FromUnixTimeMilliseconds(1000000));

            repository.InsertAction(action);

            var actionWithSameID = new AggregatorAction(action.ID, "Different description", AggregatorAction.ActionOrigin.Admin, Instant.FromUnixTimeMilliseconds(5000000));

            Assert.Throws<RepositoryConflictException>(() => repository.InsertAction(actionWithSameID));

            var actionInRepository = repository.GetActions().First();

            Assert.AreEqual(action.ActionTime, actionInRepository.ActionTime);
            Assert.AreEqual(action.Description, actionInRepository.Description);
            Assert.AreEqual(action.ID, actionInRepository.ID);
            Assert.AreEqual(action.Origin, actionInRepository.Origin);
            Assert.IsNotNull(action.Events);
            Assert.AreEqual(0, action.Events.Count);
        }

        [Test]
        public void AddActionWithDefaultIDThrowsException()
        {
            var action = new AggregatorAction(new Guid(), "Action description", AggregatorAction.ActionOrigin.User, Instant.FromUnixTimeMilliseconds(1000000));

            Assert.Throws<RepositoryConflictException>(() => repository.InsertAction(action));

            Assert.AreEqual(0, repository.GetActions().Count());
        }

        [Test]
        public void GetExistingActionByID()
        {
            var action = new AggregatorAction(Guid.NewGuid(), "Action description", AggregatorAction.ActionOrigin.User, Instant.FromUnixTimeMilliseconds(1000000));

            repository.InsertAction(action);

            var actionInRepository = repository.GetAction(action.ID);

            Assert.AreEqual(action.ActionTime, actionInRepository.ActionTime);
            Assert.AreEqual(action.Description, actionInRepository.Description);
            Assert.AreEqual(action.ID, actionInRepository.ID);
            Assert.AreEqual(action.Origin, actionInRepository.Origin);
            Assert.IsNotNull(action.Events);
            Assert.AreEqual(0, action.Events.Count);
        }

        [Test]
        public void GetInvalidAction()
        {
            var action = new AggregatorAction(Guid.NewGuid(), "Action description", AggregatorAction.ActionOrigin.User, Instant.FromUnixTimeMilliseconds(1000000));

            repository.InsertAction(action);

            Assert.Throws<RepositoryItemNotFoundException>(() => repository.GetAction(Guid.NewGuid()));
        }

        [Test]
        public void GetActionInEmptyRepository()
        {
            Assert.Throws<RepositoryItemNotFoundException>(() => repository.GetAction(Guid.NewGuid()));
        }

        [Test]
        public void AddActionWithOneEvent_EventIsPresentInRetrievedAction()
        {
            var action = new AggregatorAction(Guid.NewGuid(), "Action description", AggregatorAction.ActionOrigin.User, Instant.FromUnixTimeMilliseconds(1000000));
            var entityEvent = new CreatePollConfigurationEvent(Guid.NewGuid(), 10, "test", true);
            action.AddEvent(entityEvent);

            repository.InsertAction(action);

            var actionInRepository = repository.GetActions().First();

            Assert.AreEqual(action.ActionTime, actionInRepository.ActionTime);
            Assert.AreEqual(action.Description, actionInRepository.Description);
            Assert.AreEqual(action.ID, actionInRepository.ID);
            Assert.AreEqual(action.Origin, actionInRepository.Origin);
            Assert.IsNotNull(action.Events);
            Assert.AreEqual(1, action.Events.Count);

            var eventInAction = actionInRepository.Events.First() as CreatePollConfigurationEvent;

            Assert.AreEqual(entityEvent.ID, eventInAction.ID);
            Assert.AreEqual(entityEvent.AffectedEntityID, eventInAction.AffectedEntityID);
            Assert.AreEqual(entityEvent.AffectedEntityType, eventInAction.AffectedEntityType);

            var createdConfiguration = eventInAction.CreateEntity();

            Assert.AreEqual(10, createdConfiguration.PollIntervalMinutes);
            Assert.AreEqual("test", createdConfiguration.URL);
            Assert.AreEqual(true, createdConfiguration.Active);
        }

        [Test]
        public void AddActionWithOneEvent_EventIsPresentInRepository()
        {
            var action = new AggregatorAction(Guid.NewGuid(), "Action description", AggregatorAction.ActionOrigin.User, Instant.FromUnixTimeMilliseconds(1000000));
            var entityEvent = new CreatePollConfigurationEvent(Guid.NewGuid(), 10, "test", true);
            action.AddEvent(entityEvent);

            repository.InsertAction(action);

            var eventInRepository = repository.GetEvents().First() as CreatePollConfigurationEvent;

            Assert.AreEqual(entityEvent.ID, eventInRepository.ID);
            Assert.AreEqual(entityEvent.AffectedEntityID, eventInRepository.AffectedEntityID);
            Assert.AreEqual(entityEvent.AffectedEntityType, eventInRepository.AffectedEntityType);

            var createdConfiguration = eventInRepository.CreateEntity();

            Assert.AreEqual(10, createdConfiguration.PollIntervalMinutes);
            Assert.AreEqual("test", createdConfiguration.URL);
            Assert.AreEqual(true, createdConfiguration.Active);
        }

        [Test]
        public void GetEventById()
        {
            var action = new AggregatorAction(Guid.NewGuid(), "Action description", AggregatorAction.ActionOrigin.User, Instant.FromUnixTimeMilliseconds(1000000));
            var entityEvent = new CreatePollConfigurationEvent(Guid.NewGuid(), 10, "test", true);
            action.AddEvent(entityEvent);

            repository.InsertAction(action);

            var eventInRepository = repository.GetEvent(entityEvent.ID) as CreatePollConfigurationEvent;

            Assert.AreEqual(entityEvent.ID, eventInRepository.ID);
            Assert.AreEqual(entityEvent.AffectedEntityID, eventInRepository.AffectedEntityID);
            Assert.AreEqual(entityEvent.AffectedEntityType, eventInRepository.AffectedEntityType);

            var createdConfiguration = eventInRepository.CreateEntity();

            Assert.AreEqual(10, createdConfiguration.PollIntervalMinutes);
            Assert.AreEqual("test", createdConfiguration.URL);
            Assert.AreEqual(true, createdConfiguration.Active);
        }

        [Test]
        public void GetInvalidEvent()
        {
            var action = new AggregatorAction(Guid.NewGuid(), "Action description", AggregatorAction.ActionOrigin.User, Instant.FromUnixTimeMilliseconds(1000000));
            var entityEvent = new CreatePollConfigurationEvent(Guid.NewGuid(), 10, "test", true);
            action.AddEvent(entityEvent);

            repository.InsertAction(action);

            Assert.Throws<RepositoryItemNotFoundException>(() => repository.GetEvent(Guid.NewGuid()));
        }

        [Test]
        public void GetEventInEmptyRepository()
        {
            Assert.Throws<RepositoryItemNotFoundException>(() => repository.GetEvent(Guid.NewGuid()));
        }

        [Test]
        public void AddActionWithDuplicateEventThrowsException()
        {
            var action = new AggregatorAction(Guid.NewGuid(), "Action description", AggregatorAction.ActionOrigin.User, Instant.FromUnixTimeMilliseconds(1000000));
            var entityEvent = new CreatePollConfigurationEvent(Guid.NewGuid(), 10, "test", true);

            action.AddEvent(entityEvent);
            action.AddEvent(entityEvent);

            Assert.Throws<RepositoryConflictException>(() => repository.InsertAction(action));

            Assert.AreEqual(0, repository.GetEvents().Count());
            Assert.AreEqual(0, repository.GetActions().Count());
        }

        [Test]
        public void GetEventByAffectedEntityId()
        {
            var action = new AggregatorAction(Guid.NewGuid(), "Action description", AggregatorAction.ActionOrigin.User, Instant.FromUnixTimeMilliseconds(1000000));
            var entityEvent = new CreatePollConfigurationEvent(Guid.NewGuid(), 10, "test", true);
            action.AddEvent(entityEvent);

            repository.InsertAction(action);

            var eventInRepository = repository.GetEventsByAffectedEntityId(entityEvent.AffectedEntityID).First() as CreatePollConfigurationEvent;

            Assert.AreEqual(entityEvent.ID, eventInRepository.ID);
            Assert.AreEqual(entityEvent.AffectedEntityID, eventInRepository.AffectedEntityID);
            Assert.AreEqual(entityEvent.AffectedEntityType, eventInRepository.AffectedEntityType);

            var createdConfiguration = eventInRepository.CreateEntity();

            Assert.AreEqual(10, createdConfiguration.PollIntervalMinutes);
            Assert.AreEqual("test", createdConfiguration.URL);
            Assert.AreEqual(true, createdConfiguration.Active);
        }

        [Test]
        public void GetEventByAffectedEntityIdInEmptyRepository()
        {
            var eventsInRepository = repository.GetEventsByAffectedEntityId(Guid.NewGuid());

            Assert.AreEqual(0, eventsInRepository.Count());
        }
    }
}

using aggregator_server.Models;
using LiteDB;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace aggregator_server_test
{
    [TestFixture]
    class TestLiteDBSerializeEvents
    {
        [Test]
        public void TestStoreAndLoadOneCreateEvent()
        {
            var dbStream = new MemoryStream();
            var db = new LiteDatabase(dbStream);

            var collection = db.GetCollection<CreatePollConfigurationEvent>("CreatePollConfigurationEvents");

            var configID = Guid.NewGuid();

            collection.Insert(new CreatePollConfigurationEvent(configID, 10, "test", true));

            Assert.AreEqual(1, collection.Count());

            var createEvent = collection.FindAll().First();

            var createdConfiguration = createEvent.CreateEntity();

            Assert.AreEqual(10, createdConfiguration.PollIntervalMinutes);
            Assert.AreEqual("test", createdConfiguration.URL);
            Assert.AreEqual(true, createdConfiguration.Active);
        }

        [Test]
        public void TestStoreAndLoadOneCreateEventInBaseClassCollection()
        {
            var dbStream = new MemoryStream();
            var db = new LiteDatabase(dbStream);

            var collection = db.GetCollection<EntityEvent>("Events");

            var configID = Guid.NewGuid();

            collection.Insert(new CreatePollConfigurationEvent(configID, 10, "test", true));

            Assert.AreEqual(1, collection.Count());

            var baseEvent = collection.FindAll().First();

            Assert.IsTrue(baseEvent is CreatePollConfigurationEvent);

            var createEvent = baseEvent as CreatePollConfigurationEvent;

            var createdConfiguration = createEvent.CreateEntity();

            Assert.AreEqual(10, createdConfiguration.PollIntervalMinutes);
            Assert.AreEqual("test", createdConfiguration.URL);
            Assert.AreEqual(true, createdConfiguration.Active);
        }

        [Test]
        public void TestStoreAndLoadOneUpdateEventInBaseClassCollection()
        {
            var dbStream = new MemoryStream();
            var db = new LiteDatabase(dbStream);

            var collection = db.GetCollection<EntityEvent>("Events");

            var configID = Guid.NewGuid();
            var config = new PollConfiguration(configID, 10, "test", true);

            var updateEvent = new UpdatePollConfigurationEvent(configID);
            updateEvent.PollIntervalMinutes = 15;
            updateEvent.Active = false;

            collection.Insert(updateEvent);

            Assert.AreEqual(1, collection.Count());

            var baseEvent = collection.FindAll().First();

            Assert.IsTrue(baseEvent is UpdatePollConfigurationEvent);

            var createEvent = baseEvent as UpdatePollConfigurationEvent;

            updateEvent.UpdateEntity(config);

            Assert.AreEqual(15, config.PollIntervalMinutes);
            Assert.AreEqual("test", config.URL);
            Assert.AreEqual(false, config.Active);
        }
    }
}

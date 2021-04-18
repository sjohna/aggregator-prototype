using aggregator_server;
using aggregator_server.Models;
using LiteDB;
using NodaTime;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace aggregator_server_test
{
    [TestFixture]
    public class TestLiteDBActionStorage
    {
        private LiteDatabase database;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            LiteDBFunctions.DoLiteDBGlobalSetUp();
        }

        [SetUp]
        public void SetUp()
        {
            database = new LiteDatabase(":memory:");
        }

        [TearDown]
        public void TearDown()
        {
            database?.Dispose();
        }

        public class TestEventTypeOne : EntityEvent
        {
            public TestEventTypeOne(Guid AffectedID, EntityEvent.EntityType Type) : base(AffectedID, Type) { }

            public TestEventTypeOne() { }
        }

        public class TestEventTypeTwo : EntityEvent
        {
            public TestEventTypeTwo(Guid AffectedID, EntityEvent.EntityType Type) : base(AffectedID, Type) { }

            public TestEventTypeTwo() { }
        }

        // what I learned from this test:
        // LiteDB can set properties with private setters, but not readonly properties
        [Test]
        public void ActionWithNoEvents()
        {
            var actionCollection = database.GetCollection<AggregatorAction>("Actions");

            var actionID = Guid.NewGuid();

            var action = new AggregatorAction(actionID, "Test Action", AggregatorAction.ActionOrigin.User, Instant.FromUnixTimeMilliseconds(1000000));

            actionCollection.Insert(action);

            var actionInCollection = actionCollection.FindAll().First();

            Assert.AreEqual(actionID, actionInCollection.ID);
            Assert.AreEqual("Test Action", actionInCollection.Description);
            Assert.AreEqual(AggregatorAction.ActionOrigin.User, actionInCollection.Origin);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(1000000), actionInCollection.ActionTime);
            Assert.IsNotNull(actionInCollection.Events);
            Assert.AreEqual(0, actionInCollection.Events.Count);
        }

        [Test]
        public void ActionWithOneEvent()
        {
            var actionCollection = database.GetCollection<AggregatorAction>("Actions");
            var eventCollection = database.GetCollection<EntityEvent>("Events");

            var actionID = Guid.NewGuid();

            var action = new AggregatorAction(actionID, "Test Action", AggregatorAction.ActionOrigin.User, Instant.FromUnixTimeMilliseconds(1000000));

            var affectedEntityID = Guid.NewGuid();

            var eventForAction = new TestEventTypeOne(affectedEntityID, EntityEvent.EntityType.Document);

            action.AddEvent(eventForAction);

            eventCollection.Insert(eventForAction);
            actionCollection.Insert(action);            // NRE here was caused by the EntityEvent ID being a field instead of a property...

            var actionInCollection = actionCollection.Include(x => x.Events).FindAll().First();

            Assert.AreEqual(actionID, actionInCollection.ID);
            Assert.AreEqual("Test Action", actionInCollection.Description);
            Assert.AreEqual(AggregatorAction.ActionOrigin.User, actionInCollection.Origin);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(1000000), actionInCollection.ActionTime);
            Assert.IsNotNull(actionInCollection.Events);
            Assert.AreEqual(1, actionInCollection.Events.Count);

            var eventInAction = actionInCollection.Events.First();

            Assert.IsTrue(eventInAction is TestEventTypeOne);

            var specificEventInAction = eventInAction as TestEventTypeOne;

            Assert.AreEqual(affectedEntityID, specificEventInAction.AffectedEntityID);                      // need EntityEvent property setters to be protected, not private, for these assertions to pass...
            Assert.AreEqual(EntityEvent.EntityType.Document, specificEventInAction.AffectedEntityType);
        }
    }
}

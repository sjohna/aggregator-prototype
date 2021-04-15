﻿using LiteDB;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace aggregator_server_test.LiteDB
{
    class TestLiteDBGuidID
    {
        [TestFixture]
        class TestThingWithGuid
        {
            [BsonId]
            public Guid ID { get; set; }

            public string Name { get; set; }
        }

        // If the BsonId is a Guid, LiteDB will populate it if a default value is inserted. I will need to check that the ID field is populated before I insert in certain collections.
        [Test]
        public void AddedRecordWithDefaultGuidHasGuidSet()
        {
            var dbStream = new MemoryStream();
            var db = new LiteDatabase(dbStream);

            var collection = db.GetCollection<TestThingWithGuid>("TestThingsWithGuids");

            collection.Insert(new TestThingWithGuid { Name = "Thing 1", ID = new Guid() }); // default GUID value

            Assert.AreEqual(1, collection.Count());

            var thing = collection.FindAll().First();

            Assert.AreNotEqual(new Guid(), thing.ID);
        }

        [Test]
        public void InsertingRespectsNonDefaultID()
        {
            var dbStream = new MemoryStream();
            var db = new LiteDatabase(dbStream);

            var collection = db.GetCollection<TestThingWithGuid>("TestThingsWithGuids");

            var thingID = Guid.NewGuid();
            Assert.AreNotEqual(new Guid(), thingID);

            collection.Insert(new TestThingWithGuid { Name = "Thing 1", ID = thingID });

            Assert.AreEqual(1, collection.Count());

            var thing = collection.FindById(thingID);

            Assert.AreEqual("Thing 1", thing.Name);
        }

        [Test]
        public void CannotInsertDuplicateID()
        {
            var dbStream = new MemoryStream();
            var db = new LiteDatabase(dbStream);

            var collection = db.GetCollection<TestThingWithGuid>("TestThingsWithGuids");

            collection.Insert(new TestThingWithGuid { Name = "Thing 1", ID = new Guid() }); // default GUID value

            var thing = collection.FindAll().First();

            var newThing = new TestThingWithGuid() { Name = "Thing 2", ID = thing.ID };

            Assert.Throws<LiteException>(() => collection.Insert(newThing));
        }
    }
}
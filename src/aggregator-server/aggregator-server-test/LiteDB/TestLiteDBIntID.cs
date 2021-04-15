using LiteDB;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace aggregator_server_test.LiteDB
{
    [TestFixture]
    class TestLiteDBIntID
    {
        class TestThing
        {
            [BsonId]
            public int ID { get; set; }

            public string Name { get; set; }
        }

        // I don't think I rely on this assumption anywhere in my production code, but I do rely on it for other tests in this TestFixture
        [Test]
        public void FirstAddedRecordHasID1()
        {
            var dbStream = new MemoryStream();
            var db = new LiteDatabase(dbStream);

            var collection = db.GetCollection<TestThing>("TestThings");

            collection.Insert(new TestThing { Name = "Thing 1" });

            Assert.AreEqual(1, collection.Count());

            Assert.AreEqual("Thing 1", collection.FindById(1).Name);
            Assert.IsNull(collection.FindById(0));
            Assert.IsNull(collection.FindById(2));
        }

        [Test]
        public void CollectionDoesNotReUseDeletedRecordID()
        {
            var dbStream = new MemoryStream();
            var db = new LiteDatabase(dbStream);

            var collection = db.GetCollection<TestThing>("TestThings");

            collection.Insert(new TestThing { Name = "Thing 1" });

            collection.Delete(1);

            collection.Insert(new TestThing { Name = "Thing 2" });

            Assert.AreEqual("Thing 2", collection.FindById(2).Name);
            Assert.IsNull(collection.FindById(1));
            Assert.IsNull(collection.FindById(3));
        }

        [Test]
        public void InsertingRespectsNonDefaultID()
        {
            var dbStream = new MemoryStream();
            var db = new LiteDatabase(dbStream);

            var collection = db.GetCollection<TestThing>("TestThings");

            collection.Insert(new TestThing { ID = 7, Name = "Thing 1" });
            Assert.AreEqual("Thing 1", collection.FindById(7).Name);
            Assert.IsNull(collection.FindById(0));
            Assert.IsNull(collection.FindById(1));
        }

        [Test]
        public void CannotInsertDuplicateID()
        {
            var dbStream = new MemoryStream();
            var db = new LiteDatabase(dbStream);

            var collection = db.GetCollection<TestThing>("TestThings");

            collection.Insert(new TestThing { ID = 7, Name = "Thing 1" });
            Assert.Throws<LiteException>(() => collection.Insert(new TestThing { ID = 7, Name = "Thing 1" }));
            Assert.AreEqual("Thing 1", collection.FindById(7).Name);
            Assert.IsNull(collection.FindById(0));
            Assert.IsNull(collection.FindById(1));
        }
    }
}

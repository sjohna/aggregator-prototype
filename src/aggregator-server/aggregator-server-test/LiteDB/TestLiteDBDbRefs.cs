using LiteDB;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace aggregator_server_test.LiteDB
{
    [TestFixture]
    class TestLiteDBDbRefs
    {
        public class Thing1
        {
            [BsonId]
            public Guid ID { get; set; }

            public List<Thing2> OtherThings { get; set; }

            public int SomeValue { get; set; }
        }

        public class Thing2
        {
            [BsonId]
            public Guid ID { get; set; }

            public string Name { get; set; }
        }

        private LiteDatabase database;

        [SetUp]
        public void SetUp()
        {
            BsonMapper.Global.Entity<Thing1>()
                .DbRef(x => x.OtherThings, "ThingTwos");

            database = new LiteDatabase(":memory:");
        }

        [TearDown]
        public void TearDown()
        {
            database?.Dispose();
        }

        [Test]
        public void StoreAndRetrieveThing1WithNoOtherThings()
        {
            var thing1Collection = database.GetCollection<Thing1>("ThingOnes");
            var thing2Collection = database.GetCollection<Thing2>("ThingTwos");

            var thing1 = new Thing1()
            {
                ID = Guid.NewGuid(),
                OtherThings = new List<Thing2>(),
                SomeValue = 7
            };

            thing1Collection.Insert(thing1);

            var thing1InCollection = thing1Collection.FindAll().First();

            Assert.AreEqual(7, thing1InCollection.SomeValue);
            Assert.AreEqual(0, thing1InCollection.OtherThings.Count);
        }

        [Test]
        public void StoreAndRetrieveThing1WithOneOtherThings()
        {
            var thing1Collection = database.GetCollection<Thing1>("ThingOnes");
            var thing2Collection = database.GetCollection<Thing2>("ThingTwos");

            var thing1 = new Thing1()
            {
                ID = Guid.NewGuid(),
                OtherThings = new List<Thing2>(),
                SomeValue = 7
            };

            var thing2 = new Thing2()
            {
                ID = Guid.NewGuid(),
                Name = "Thing Two #1"
            };

            thing1.OtherThings.Add(thing2);

            thing2Collection.Insert(thing2);
            thing1Collection.Insert(thing1);

            var thing2InCollection = thing2Collection.FindAll().First();

            Assert.AreEqual("Thing Two #1", thing2InCollection.Name);

            var thing1InCollection = thing1Collection.Include(x => x.OtherThings).FindAll().First();    // need to explicitly include OtherThings, or they won't be initialized when pulled from the DB

            Assert.AreEqual(7, thing1InCollection.SomeValue);
            Assert.AreEqual(1, thing1InCollection.OtherThings.Count);

            var thing2FromThing1 = thing1InCollection.OtherThings.First();

            Assert.AreEqual("Thing Two #1", thing2FromThing1.Name);
        }
    }
}

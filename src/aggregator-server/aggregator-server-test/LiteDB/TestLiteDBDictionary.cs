using LiteDB;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace aggregator_server_test.LiteDB
{
    class TestLiteDBDictionary
    {
        class TestThing
        {
            public Guid ID { get; set; }

            private Dictionary<string, object> PrivateDictionary { get; set; }

            [BsonIgnore]
            public IReadOnlyDictionary<string, object> Dictionary => PrivateDictionary;

            public TestThing() { }

            public TestThing(Guid id, Dictionary<string, object> dictionary)
            {
                this.ID = id;
                this.PrivateDictionary = dictionary;
            }
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            BsonMapper.Global.IncludeNonPublic = true;
        }

        private LiteDatabase database;

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

        [Test]
        public void SerializeAndDeserializeNullDictionary()
        {
            var collection = database.GetCollection<TestThing>("TestThings");

            collection.Insert(new TestThing(new Guid(), null)); // default GUID value

            Assert.AreEqual(1, collection.Count());

            var thing = collection.FindAll().First();

            Assert.IsNull(thing.Dictionary);
        }

        [Test]
        public void SerializeAndDeserializeNonemptyDictionary()
        {
            var collection = database.GetCollection<TestThing>("TestThings");

            var dictionary = new Dictionary<string, object>();
            dictionary.Add("one", 1);
            dictionary.Add("two", 2);

            collection.Insert(new TestThing(new Guid(), dictionary)); // default GUID value

            Assert.AreEqual(1, collection.Count());

            var thing = collection.FindAll().First();

            Assert.IsNotNull(thing.Dictionary);
            Assert.AreEqual(1, thing.Dictionary["one"]);
            Assert.AreEqual(2, thing.Dictionary["two"]);

            var serialized = BsonMapper.Global.Serialize<TestThing>(thing);
        }
    }
}

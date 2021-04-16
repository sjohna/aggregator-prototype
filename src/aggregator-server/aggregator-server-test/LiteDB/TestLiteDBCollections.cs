using LiteDB;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace aggregator_server_test.LiteDB
{
    [TestFixture]
    class TestLiteDBCollections
    {
        class TestThing
        {
            [BsonId]
            public int ID { get; set; }

            public string Name { get; set; }
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
        public void FindAllDoesNotBreakIfItemAddedWhileEnumerating()
        {
            var collection = database.GetCollection<TestThing>("TestThings");

            collection.Insert(new TestThing { ID = 1, Name = "Thing 1" });
            collection.Insert(new TestThing { ID = 2, Name = "Thing 2" });

            var enumerable = collection.FindAll();

            int index = 0;

            foreach (var thing in enumerable)
            {
                if (index == 1)
                {
                    collection.Insert(new TestThing { ID = 3, Name = "Thing 3" });
                }

                Assert.AreEqual($"Thing {index+1}", thing.Name);
                ++index;
            }

            Assert.AreEqual(2, index);
        }
    }
}

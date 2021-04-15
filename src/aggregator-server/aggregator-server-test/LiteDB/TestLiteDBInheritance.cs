using LiteDB;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aggregator_server_test.LiteDB
{
    [TestFixture]
    class TestLiteDBInheritance
    {
        public abstract class BaseClass
        {
            [BsonId]
            public Guid ID { get; set; }

            public int BaseNumber { get; set; }
        }

        public abstract class IntermediateA : BaseClass
        {
            public string IntermediateAThing { get; set; }
        }

        public abstract class IntermediateB : BaseClass
        {
            public string IntermediateBThing { get; set; }
        }

        public class LeafA1 : IntermediateA
        {
            public int ValueOne { get; set; }
        }

        public class LeafA2 : IntermediateA
        {
            public int ValueTwo { get; set; }
        }

        public class LeafB1 : IntermediateB
        {
            public int ValueOne { get; set; }
        }

        public class LeafB2 : IntermediateB
        {
            public int ValueTwo { get; set; }
        }

        public class OtherThing
        {
            [BsonId]
            public Guid ID { get; set; }

            public List<BaseClass> BaseClasses { get; set; }
        }

        [Test]
        public void StoreAndRetrieveOneLeafInstanceFromLeafCollection()
        {
            using (var db = new LiteDatabase(":memory:"))
            {
                var leafCollection = db.GetCollection<LeafA1>("LeafA1s");

                var leaf = new LeafA1()
                {
                    ID = Guid.NewGuid(),
                    BaseNumber = 7,
                    IntermediateAThing = "Thing",
                    ValueOne = 12
                };

                leafCollection.Insert(leaf);

                var leafInCollection = leafCollection.FindAll().First();

                Assert.AreEqual(7, leafInCollection.BaseNumber);
                Assert.AreEqual("Thing", leafInCollection.IntermediateAThing);
                Assert.AreEqual(12, leafInCollection.ValueOne);
            }
        }

        [Test]
        public void StoreAndRetrieveOneLeafInstanceFromBaseClassCollection()
        {
            using (var db = new LiteDatabase(":memory:"))
            {
                var baseCollection = db.GetCollection<BaseClass>("BaseClasses");

                var leaf = new LeafA1()
                {
                    ID = Guid.NewGuid(),
                    BaseNumber = 7,
                    IntermediateAThing = "Thing",
                    ValueOne = 12
                };

                baseCollection.Insert(leaf);

                var baseInCollection = baseCollection.FindAll().First();

                Assert.IsTrue(baseInCollection is LeafA1);

                var leafInCollection = baseInCollection as LeafA1;

                Assert.AreEqual(7, leafInCollection.BaseNumber);
                Assert.AreEqual("Thing", leafInCollection.IntermediateAThing);
                Assert.AreEqual(12, leafInCollection.ValueOne);
            }
        }

        [Test]
        public void DbRefToListOfBaseClass()
        {
            using (var db = new LiteDatabase(":memory:"))
            {
                BsonMapper.Global.Entity<OtherThing>()
                    .DbRef(x => x.BaseClasses, "BaseClasses");

                var baseCollection = db.GetCollection<BaseClass>("BaseClasses");
                var otherThingCollection = db.GetCollection<OtherThing>("OtherThings");

                var leaf = new LeafA1()
                {
                    ID = Guid.NewGuid(),
                    BaseNumber = 7,
                    IntermediateAThing = "Thing",
                    ValueOne = 12
                };

                var otherThing = new OtherThing()
                {
                    ID = Guid.NewGuid(),
                    BaseClasses = new List<BaseClass>()
                };

                otherThing.BaseClasses.Add(leaf);

                baseCollection.Insert(leaf);
                otherThingCollection.Insert(otherThing);
            }
        }
    }
}

using aggregator_server.Models;
using NodaTime;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace aggregator_server_test
{
    [TestFixture]
    class TestCreateDocumentEvent
    {
        [Test]
        public void AffectedEntityType()
        {
            var e = new CreateDocumentEvent(
                DocumentID: Guid.NewGuid(),
                SourceID: "sourceID",
                Title: "Title",
                Content: "Content",
                PublishTime: Instant.FromUnixTimeMilliseconds(1000000),
                UpdateTime: Instant.FromUnixTimeMilliseconds(1500000),
                SourceLink: "source link");

            Assert.AreEqual(EntityEvent.EntityType.Document, e.AffectedEntityType);
        }

        [Test]
        public void CreateEntity()
        {
            var id = Guid.NewGuid();

            var e = new CreateDocumentEvent(
                DocumentID: id,
                SourceID: "sourceID",
                Title: "Title",
                Content: "Content",
                PublishTime: Instant.FromUnixTimeMilliseconds(1000000),
                UpdateTime: Instant.FromUnixTimeMilliseconds(1500000),
                SourceLink: "source link");

            var doc = e.CreateEntity();

            Assert.AreEqual(id, doc.ID);
            Assert.AreEqual("sourceID", doc.SourceID);
            Assert.AreEqual("Title", doc.Title);
            Assert.AreEqual("Content", doc.Content);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(1000000), doc.PublishTime);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(1500000), doc.UpdateTime);
            Assert.AreEqual("source link", doc.SourceLink);
        }
    }
}

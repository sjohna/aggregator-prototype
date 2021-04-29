using aggregator_server.Exceptions;
using aggregator_server.Models;
using NodaTime;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace aggregator_server_test
{
    [TestFixture]
    class TestUpdateDocumentEvent
    {
        [Test]
        public void AffectedEntityType()
        {
            var e = new UpdateDocumentEvent(Guid.NewGuid());

            Assert.AreEqual(EntityEvent.EntityType.Document, e.AffectedEntityType);
        }

        [Test]
        public void UpdateSourceID()
        {
            var id = Guid.NewGuid();

            var doc = new Document
            {
                ID = id,
                Content = "content",
                PublishTime = Instant.FromUnixTimeMilliseconds(1000000),
                SourceID = "source ID",
                SourceLink = "source link",
                Title = "title",
                UpdateTime = Instant.FromUnixTimeMilliseconds(2000000)
            };

            var e = new UpdateDocumentEvent(id);
            e.SourceID = "new source ID";

            e.UpdateEntity(doc);

            Assert.AreEqual(id, doc.ID);
            Assert.AreEqual("content", doc.Content);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(1000000), doc.PublishTime);
            Assert.AreEqual("new source ID", doc.SourceID);
            Assert.AreEqual("source link", doc.SourceLink);
            Assert.AreEqual("title", doc.Title);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(2000000), doc.UpdateTime);
        }

        [Test]
        public void UpdateTitle()
        {
            var id = Guid.NewGuid();

            var doc = new Document
            {
                ID = id,
                Content = "content",
                PublishTime = Instant.FromUnixTimeMilliseconds(1000000),
                SourceID = "source ID",
                SourceLink = "source link",
                Title = "title",
                UpdateTime = Instant.FromUnixTimeMilliseconds(2000000)
            };

            var e = new UpdateDocumentEvent(id);
            e.Title = "new title";

            e.UpdateEntity(doc);

            Assert.AreEqual(id, doc.ID);
            Assert.AreEqual("content", doc.Content);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(1000000), doc.PublishTime);
            Assert.AreEqual("source ID", doc.SourceID);
            Assert.AreEqual("source link", doc.SourceLink);
            Assert.AreEqual("new title", doc.Title);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(2000000), doc.UpdateTime);
        }

        [Test]
        public void UpdateContent()
        {
            var id = Guid.NewGuid();

            var doc = new Document
            {
                ID = id,
                Content = "content",
                PublishTime = Instant.FromUnixTimeMilliseconds(1000000),
                SourceID = "source ID",
                SourceLink = "source link",
                Title = "title",
                UpdateTime = Instant.FromUnixTimeMilliseconds(2000000)
            };

            var e = new UpdateDocumentEvent(id);
            e.Content = "new content";

            e.UpdateEntity(doc);

            Assert.AreEqual(id, doc.ID);
            Assert.AreEqual("new content", doc.Content);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(1000000), doc.PublishTime);
            Assert.AreEqual("source ID", doc.SourceID);
            Assert.AreEqual("source link", doc.SourceLink);
            Assert.AreEqual("title", doc.Title);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(2000000), doc.UpdateTime);
        }

        [Test]
        public void UpdatePublishTime()
        {
            var id = Guid.NewGuid();

            var doc = new Document
            {
                ID = id,
                Content = "content",
                PublishTime = Instant.FromUnixTimeMilliseconds(1000000),
                SourceID = "source ID",
                SourceLink = "source link",
                Title = "title",
                UpdateTime = Instant.FromUnixTimeMilliseconds(2000000)
            };

            var e = new UpdateDocumentEvent(id);
            e.PublishTime = Instant.FromUnixTimeMilliseconds(1500000);

            e.UpdateEntity(doc);

            Assert.AreEqual(id, doc.ID);
            Assert.AreEqual("content", doc.Content);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(1500000), doc.PublishTime);
            Assert.AreEqual("source ID", doc.SourceID);
            Assert.AreEqual("source link", doc.SourceLink);
            Assert.AreEqual("title", doc.Title);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(2000000), doc.UpdateTime);
        }

        [Test]
        public void UpdateUpdateTime()
        {
            var id = Guid.NewGuid();

            var doc = new Document
            {
                ID = id,
                Content = "content",
                PublishTime = Instant.FromUnixTimeMilliseconds(1000000),
                SourceID = "source ID",
                SourceLink = "source link",
                Title = "title",
                UpdateTime = Instant.FromUnixTimeMilliseconds(2000000)
            };

            var e = new UpdateDocumentEvent(id);
            e.UpdateTime = Instant.FromUnixTimeMilliseconds(2500000);

            e.UpdateEntity(doc);

            Assert.AreEqual(id, doc.ID);
            Assert.AreEqual("content", doc.Content);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(1000000), doc.PublishTime);
            Assert.AreEqual("source ID", doc.SourceID);
            Assert.AreEqual("source link", doc.SourceLink);
            Assert.AreEqual("title", doc.Title);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(2500000), doc.UpdateTime);
        }

        [Test]
        public void UpdateSourceLink()
        {
            var id = Guid.NewGuid();

            var doc = new Document
            {
                ID = id,
                Content = "content",
                PublishTime = Instant.FromUnixTimeMilliseconds(1000000),
                SourceID = "source ID",
                SourceLink = "source link",
                Title = "title",
                UpdateTime = Instant.FromUnixTimeMilliseconds(2000000)
            };

            var e = new UpdateDocumentEvent(id);
            e.SourceLink = "new source link";

            e.UpdateEntity(doc);

            Assert.AreEqual(id, doc.ID);
            Assert.AreEqual("content", doc.Content);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(1000000), doc.PublishTime);
            Assert.AreEqual("source ID", doc.SourceID);
            Assert.AreEqual("new source link", doc.SourceLink);
            Assert.AreEqual("title", doc.Title);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(2000000), doc.UpdateTime);
        }

        [Test]
        public void UpdateAllFields()
        {
            var id = Guid.NewGuid();

            var doc = new Document
            {
                ID = id,
                Content = "content",
                PublishTime = Instant.FromUnixTimeMilliseconds(1000000),
                SourceID = "source ID",
                SourceLink = "source link",
                Title = "title",
                UpdateTime = Instant.FromUnixTimeMilliseconds(2000000)
            };

            var e = new UpdateDocumentEvent(id);
            e.SourceID = "new source ID";
            e.Title = "new title";
            e.Content = "new content";
            e.PublishTime = Instant.FromUnixTimeMilliseconds(1500000);
            e.UpdateTime = Instant.FromUnixTimeMilliseconds(2500000);
            e.SourceLink = "new source link";

            e.UpdateEntity(doc);

            Assert.AreEqual(id, doc.ID);
            Assert.AreEqual("new content", doc.Content);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(1500000), doc.PublishTime);
            Assert.AreEqual("new source ID", doc.SourceID);
            Assert.AreEqual("new source link", doc.SourceLink);
            Assert.AreEqual("new title", doc.Title);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(2500000), doc.UpdateTime);
        }

        [Test]
        public void UpdateNoFieldsThrowsException()
        {
            var id = Guid.NewGuid();

            var doc = new Document
            {
                ID = id,
                Content = "content",
                PublishTime = Instant.FromUnixTimeMilliseconds(1000000),
                SourceID = "source ID",
                SourceLink = "source link",
                Title = "title",
                UpdateTime = Instant.FromUnixTimeMilliseconds(2000000)
            };

            var e = new UpdateDocumentEvent(id);

            Assert.Throws<EventException>(() => e.UpdateEntity(doc));

            Assert.AreEqual(id, doc.ID);
            Assert.AreEqual("content", doc.Content);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(1000000), doc.PublishTime);
            Assert.AreEqual("source ID", doc.SourceID);
            Assert.AreEqual("source link", doc.SourceLink);
            Assert.AreEqual("title", doc.Title);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(2000000), doc.UpdateTime);
        }

        [Test]
        public void UpdateWrongEntityThrowsException()
        {
            var id = Guid.NewGuid();

            var doc = new Document
            {
                ID = id,
                Content = "content",
                PublishTime = Instant.FromUnixTimeMilliseconds(1000000),
                SourceID = "source ID",
                SourceLink = "source link",
                Title = "title",
                UpdateTime = Instant.FromUnixTimeMilliseconds(2000000)
            };

            var e = new UpdateDocumentEvent(Guid.NewGuid());
            e.SourceID = "new source ID";

            Assert.Throws<EventException>(() => e.UpdateEntity(doc));

            Assert.AreEqual(id, doc.ID);
            Assert.AreEqual("content", doc.Content);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(1000000), doc.PublishTime);
            Assert.AreEqual("source ID", doc.SourceID);
            Assert.AreEqual("source link", doc.SourceLink);
            Assert.AreEqual("title", doc.Title);
            Assert.AreEqual(Instant.FromUnixTimeMilliseconds(2000000), doc.UpdateTime);
        }
    }
}

using aggregator_server;
using aggregator_server.Models;
using NodaTime;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aggregator_server_test
{
    abstract class TestDocumentRepository
    {
        protected IDocumentRepository repository;

        [Test]
        public void InitialRepositoryState()
        {
            Assert.AreEqual(0, repository.GetDocuments().Count());
        }

        [Test]

        public void AddOneDocument()
        {
            var doc = new Document()
            {
                Content = "some content",
                PublishTime = Instant.FromUnixTimeSeconds(1000),
                SourceID = "sourceID",
                SourceLink = "sourceLink",
                Title = "The Title",
                UpdateTime = Instant.FromUnixTimeSeconds(2000)
            };

            Assert.AreEqual(0, doc.ID);

            repository.AddDocument(doc);

            Assert.AreEqual(1, repository.GetDocuments().Count());

            Assert.AreNotEqual(0, doc.ID);

            var docInRepository = repository.GetDocuments().First();

            Assert.AreEqual("some content", docInRepository.Content);
            Assert.AreEqual(Instant.FromUnixTimeSeconds(1000), docInRepository.PublishTime);
            Assert.AreEqual("sourceID", docInRepository.SourceID);
            Assert.AreEqual("sourceLink", docInRepository.SourceLink);
            Assert.AreEqual("The Title", docInRepository.Title);
            Assert.AreEqual(Instant.FromUnixTimeSeconds(2000), docInRepository.UpdateTime);
        }

        [Test]
        public void UpdateDocument()
        {
            var doc = new Document()
            {
                Content = "some content",
                PublishTime = Instant.FromUnixTimeSeconds(1000),
                SourceID = "sourceID",
                SourceLink = "sourceLink",
                Title = "The Title",
                UpdateTime = Instant.FromUnixTimeSeconds(2000)
            };

            repository.AddDocument(doc);

            doc.Content = "new content";
            doc.UpdateTime = Instant.FromUnixTimeSeconds(3000);

            var docGottenBeforeUpdate = repository.GetDocuments().First();

            Assert.AreEqual("some content", docGottenBeforeUpdate.Content);
            Assert.AreEqual(Instant.FromUnixTimeSeconds(1000), docGottenBeforeUpdate.PublishTime);
            Assert.AreEqual("sourceID", docGottenBeforeUpdate.SourceID);
            Assert.AreEqual("sourceLink", docGottenBeforeUpdate.SourceLink);
            Assert.AreEqual("The Title", docGottenBeforeUpdate.Title);
            Assert.AreEqual(Instant.FromUnixTimeSeconds(2000), docGottenBeforeUpdate.UpdateTime);

            repository.UpdateDocument(doc);

            var docGottenAfterUpdate = repository.GetDocuments().First();

            Assert.AreEqual("new content", docGottenAfterUpdate.Content);
            Assert.AreEqual(Instant.FromUnixTimeSeconds(1000), docGottenAfterUpdate.PublishTime);
            Assert.AreEqual("sourceID", docGottenAfterUpdate.SourceID);
            Assert.AreEqual("sourceLink", docGottenAfterUpdate.SourceLink);
            Assert.AreEqual("The Title", docGottenAfterUpdate.Title);
            Assert.AreEqual(Instant.FromUnixTimeSeconds(3000), docGottenAfterUpdate.UpdateTime);
        }

        [Test]
        public void UpdateDocumentWithNoChanges()
        {
            var doc = new Document()
            {
                Content = "some content",
                PublishTime = Instant.FromUnixTimeSeconds(1000),
                SourceID = "sourceID",
                SourceLink = "sourceLink",
                Title = "The Title",
                UpdateTime = Instant.FromUnixTimeSeconds(2000)
            };

            repository.AddDocument(doc);
            repository.UpdateDocument(doc);

            var docGottenAfterUpdate = repository.GetDocuments().First();

            Assert.AreEqual("some content", docGottenAfterUpdate.Content);
            Assert.AreEqual(Instant.FromUnixTimeSeconds(1000), docGottenAfterUpdate.PublishTime);
            Assert.AreEqual("sourceID", docGottenAfterUpdate.SourceID);
            Assert.AreEqual("sourceLink", docGottenAfterUpdate.SourceLink);
            Assert.AreEqual("The Title", docGottenAfterUpdate.Title);
            Assert.AreEqual(Instant.FromUnixTimeSeconds(2000), docGottenAfterUpdate.UpdateTime);
        }

        [Test]
        public void FindBySourceID_EmpytRepository()
        {
            Assert.AreEqual(0, repository.FindBySourceID("ID").Count());
        }

        [Test]
        public void FindBySourceID_OneMatchingDocument()
        {
            var doc = new Document()
            {
                Content = "some content",
                PublishTime = Instant.FromUnixTimeSeconds(1000),
                SourceID = "sourceID",
                SourceLink = "sourceLink",
                Title = "The Title",
                UpdateTime = Instant.FromUnixTimeSeconds(2000)
            };

            repository.AddDocument(doc);

            var findResult = repository.FindBySourceID("sourceID");

            Assert.AreEqual(1, findResult.Count());

            var foundDoc = findResult.First();

            Assert.AreEqual("some content", foundDoc.Content);
            Assert.AreEqual(Instant.FromUnixTimeSeconds(1000), foundDoc.PublishTime);
            Assert.AreEqual("sourceID", foundDoc.SourceID);
            Assert.AreEqual("sourceLink", foundDoc.SourceLink);
            Assert.AreEqual("The Title", foundDoc.Title);
            Assert.AreEqual(Instant.FromUnixTimeSeconds(2000), foundDoc.UpdateTime);
        }

        [Test]
        public void FindBySourceID_NoMatchingDocuments()
        {
            var doc = new Document()
            {
                Content = "some content",
                PublishTime = Instant.FromUnixTimeSeconds(1000),
                SourceID = "sourceID",
                SourceLink = "sourceLink",
                Title = "The Title",
                UpdateTime = Instant.FromUnixTimeSeconds(2000)
            };

            repository.AddDocument(doc);

            var findResult = repository.FindBySourceID("differentSourceID");

            Assert.AreEqual(0, findResult.Count());
        }

        [Test]
        public void FindBySourceID_OneOfTwoDocumentsMatches()
        {
            var doc1 = new Document()
            {
                Content = "some content",
                PublishTime = Instant.FromUnixTimeSeconds(1000),
                SourceID = "sourceID",
                SourceLink = "sourceLink",
                Title = "The Title",
                UpdateTime = Instant.FromUnixTimeSeconds(2000)
            };

            var doc2 = new Document()
            {
                Content = "different content",
                PublishTime = Instant.FromUnixTimeSeconds(3000),
                SourceID = "differentSourceID",
                SourceLink = "differentSourceLink",
                Title = "The Different Title",
                UpdateTime = Instant.FromUnixTimeSeconds(4000)
            };

            repository.AddDocument(doc1);
            repository.AddDocument(doc2);

            var findResult = repository.FindBySourceID("sourceID");

            Assert.AreEqual(1, findResult.Count());

            var foundDoc = findResult.First();

            Assert.AreEqual("some content", foundDoc.Content);
            Assert.AreEqual(Instant.FromUnixTimeSeconds(1000), foundDoc.PublishTime);
            Assert.AreEqual("sourceID", foundDoc.SourceID);
            Assert.AreEqual("sourceLink", foundDoc.SourceLink);
            Assert.AreEqual("The Title", foundDoc.Title);
            Assert.AreEqual(Instant.FromUnixTimeSeconds(2000), foundDoc.UpdateTime);
        }
    }
}

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

    }
}

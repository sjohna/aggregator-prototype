using aggregator_server;
using LiteDB;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;
using NodaTime;

namespace aggregator_server_test.Poller
{
    [TestFixture]
    class TestPoller
    {
        protected IDocumentRepository repository;

        private MemoryStream databaseStream;
        private LiteDatabase database;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            LiteDBFunctions.DoLiteDBGlobalSetUp();
        }

        [SetUp]
        public void SetUp()
        {
            databaseStream = new MemoryStream();
            database = new LiteDatabase(databaseStream);
            repository = new LiteDBDocumentRepository(database);
        }

        [TearDown]
        public void TearDown()
        {
            if (repository != null) repository.Dispose();
            if (database != null) database.Dispose();
            if (databaseStream != null) databaseStream.Dispose();
        }

        private XmlReader GetAtomResourceXmlReader(string filename)
        {
            string resourceName = $"aggregator_server_test.TestData.atom.{filename}";

            var assembly = typeof(aggregator_server_test.Poller.TestPoller).Assembly;

            Stream resourceStream = assembly.GetManifestResourceStream(resourceName);

            return XmlReader.Create(resourceStream);
        }

        [Test]
        public void SinglePostInFreshRepository()
        {
            using (var reader = GetAtomResourceXmlReader("singlePost.xml"))
            {
                aggregator_server.Poller.ParseAtomFeedAndUpdateRepository(repository, reader);
            }

            Assert.AreEqual(1, repository.GetDocuments().Count());

            var addedDocument = repository.GetDocuments().First();

            Assert.AreEqual("Test Blog Entry Title", addedDocument.Title);
            Assert.AreEqual("<p>This is the test blog entry content.</p>", addedDocument.Content);
            Assert.AreEqual("12345", addedDocument.SourceID);
            Assert.AreEqual("https://example.com/testblog/test-blog-entry-title", addedDocument.SourceLink);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 0, 0), addedDocument.PublishTime);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 0, 0), addedDocument.UpdateTime);
        }

        [Test]
        public void SinglePostIsUpdated()
        {
            using (var reader = GetAtomResourceXmlReader("singlePost.xml"))
            {
                aggregator_server.Poller.ParseAtomFeedAndUpdateRepository(repository, reader);
            }

            using (var reader = GetAtomResourceXmlReader("singlePostUpdated.xml"))
            {
                aggregator_server.Poller.ParseAtomFeedAndUpdateRepository(repository, reader);
            }

            Assert.AreEqual(1, repository.GetDocuments().Count());

            var addedDocument = repository.GetDocuments().First();

            Assert.AreEqual("Test Blog Entry Title", addedDocument.Title);
            Assert.AreEqual("<p>This is the updated test blog entry content.</p>", addedDocument.Content);
            Assert.AreEqual("12345", addedDocument.SourceID);
            Assert.AreEqual("https://example.com/testblog/test-blog-entry-title", addedDocument.SourceLink);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 0, 0), addedDocument.PublishTime);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 1, 0), addedDocument.UpdateTime);
        }

        [Test]
        public void SinglePostNotUpdatedIfUpdatedTimeIsOlder()
        {
            using (var reader = GetAtomResourceXmlReader("singlePostUpdated.xml"))
            {
                aggregator_server.Poller.ParseAtomFeedAndUpdateRepository(repository, reader);
            }

            using (var reader = GetAtomResourceXmlReader("singlePost.xml"))
            {
                aggregator_server.Poller.ParseAtomFeedAndUpdateRepository(repository, reader);
            }

            Assert.AreEqual(1, repository.GetDocuments().Count());

            var addedDocument = repository.GetDocuments().First();

            Assert.AreEqual("Test Blog Entry Title", addedDocument.Title);
            Assert.AreEqual("<p>This is the updated test blog entry content.</p>", addedDocument.Content);
            Assert.AreEqual("12345", addedDocument.SourceID);
            Assert.AreEqual("https://example.com/testblog/test-blog-entry-title", addedDocument.SourceLink);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 0, 0), addedDocument.PublishTime);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 1, 0), addedDocument.UpdateTime);
        }
    }
}

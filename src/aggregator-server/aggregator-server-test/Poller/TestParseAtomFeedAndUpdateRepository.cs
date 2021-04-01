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

namespace aggregator_server_test
{
    [TestFixture]
    abstract class TestParseAtomFeedAndUpdateRepository
    {
        protected IDocumentRepository repository;
        protected MemoryStream databaseStream;
        protected LiteDatabase database;

        private XmlReader GetAtomResourceXmlReader(string filename)
        {
            string resourceName = $"aggregator_server_test.TestData.atom.{filename}";

            var assembly = typeof(aggregator_server_test.TestParseAtomFeedAndUpdateRepository).Assembly;

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

        [Test]
        public void TwoPostsInFreshRepository()
        {
            using (var reader = GetAtomResourceXmlReader("twoPosts.xml"))
            {
                aggregator_server.Poller.ParseAtomFeedAndUpdateRepository(repository, reader);
            }

            Assert.AreEqual(2, repository.GetDocuments().Count());

            var firstDocument = repository.GetDocuments().First(doc => doc.SourceID == "12345");

            Assert.AreEqual("Test Blog Entry Title", firstDocument.Title);
            Assert.AreEqual("<p>This is the test blog entry content.</p>", firstDocument.Content);
            Assert.AreEqual("12345", firstDocument.SourceID);
            Assert.AreEqual("https://example.com/testblog/test-blog-entry-title", firstDocument.SourceLink);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 0, 0), firstDocument.PublishTime);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 0, 0), firstDocument.UpdateTime);

            var secondDocument = repository.GetDocuments().First(doc => doc.SourceID == "12346");

            Assert.AreEqual("Test Blog Entry Title 2", secondDocument.Title);
            Assert.AreEqual("<p>This is the second test blog entry content.</p>", secondDocument.Content);
            Assert.AreEqual("12346", secondDocument.SourceID);
            Assert.AreEqual("https://example.com/testblog/test-blog-entry-title-2", secondDocument.SourceLink);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 0, 30), secondDocument.PublishTime);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 0, 30), secondDocument.UpdateTime);
        }

        [Test]
        public void UpdateOnePostAfterParsingTwoPostFeed()
        {
            using (var reader = GetAtomResourceXmlReader("twoPosts.xml"))
            {
                aggregator_server.Poller.ParseAtomFeedAndUpdateRepository(repository, reader);
            }

            using (var reader = GetAtomResourceXmlReader("singlePostUpdated.xml"))
            {
                aggregator_server.Poller.ParseAtomFeedAndUpdateRepository(repository, reader);
            }

            Assert.AreEqual(2, repository.GetDocuments().Count());

            var firstDocument = repository.GetDocuments().First(doc => doc.SourceID == "12345");

            Assert.AreEqual("Test Blog Entry Title", firstDocument.Title);
            Assert.AreEqual("<p>This is the updated test blog entry content.</p>", firstDocument.Content);
            Assert.AreEqual("12345", firstDocument.SourceID);
            Assert.AreEqual("https://example.com/testblog/test-blog-entry-title", firstDocument.SourceLink);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 0, 0), firstDocument.PublishTime);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 1, 0), firstDocument.UpdateTime);

            var secondDocument = repository.GetDocuments().First(doc => doc.SourceID == "12346");

            Assert.AreEqual("Test Blog Entry Title 2", secondDocument.Title);
            Assert.AreEqual("<p>This is the second test blog entry content.</p>", secondDocument.Content);
            Assert.AreEqual("12346", secondDocument.SourceID);
            Assert.AreEqual("https://example.com/testblog/test-blog-entry-title-2", secondDocument.SourceLink);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 0, 30), secondDocument.PublishTime);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 0, 30), secondDocument.UpdateTime);
        }

        [Test]
        public void ParseSameFeedTwice()
        {
            using (var reader = GetAtomResourceXmlReader("twoPosts.xml"))
            {
                aggregator_server.Poller.ParseAtomFeedAndUpdateRepository(repository, reader);
            }

            using (var reader = GetAtomResourceXmlReader("twoPosts.xml"))
            {
                aggregator_server.Poller.ParseAtomFeedAndUpdateRepository(repository, reader);
            }

            Assert.AreEqual(2, repository.GetDocuments().Count());

            var firstDocument = repository.GetDocuments().First(doc => doc.SourceID == "12345");

            Assert.AreEqual("Test Blog Entry Title", firstDocument.Title);
            Assert.AreEqual("<p>This is the test blog entry content.</p>", firstDocument.Content);
            Assert.AreEqual("12345", firstDocument.SourceID);
            Assert.AreEqual("https://example.com/testblog/test-blog-entry-title", firstDocument.SourceLink);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 0, 0), firstDocument.PublishTime);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 0, 0), firstDocument.UpdateTime);

            var secondDocument = repository.GetDocuments().First(doc => doc.SourceID == "12346");

            Assert.AreEqual("Test Blog Entry Title 2", secondDocument.Title);
            Assert.AreEqual("<p>This is the second test blog entry content.</p>", secondDocument.Content);
            Assert.AreEqual("12346", secondDocument.SourceID);
            Assert.AreEqual("https://example.com/testblog/test-blog-entry-title-2", secondDocument.SourceLink);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 0, 30), secondDocument.PublishTime);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 0, 30), secondDocument.UpdateTime);
        }

        [Test]
        public void MultiplePostFeedWithOneUpdatedPost()
        {
            using (var reader = GetAtomResourceXmlReader("twoPosts.xml"))
            {
                aggregator_server.Poller.ParseAtomFeedAndUpdateRepository(repository, reader);
            }

            using (var reader = GetAtomResourceXmlReader("twoPosts_oneUpdated.xml"))
            {
                aggregator_server.Poller.ParseAtomFeedAndUpdateRepository(repository, reader);
            }

            Assert.AreEqual(2, repository.GetDocuments().Count());

            var firstDocument = repository.GetDocuments().First(doc => doc.SourceID == "12345");

            Assert.AreEqual("Test Blog Entry Title", firstDocument.Title);
            Assert.AreEqual("<p>This is the updated test blog entry content.</p>", firstDocument.Content);
            Assert.AreEqual("12345", firstDocument.SourceID);
            Assert.AreEqual("https://example.com/testblog/test-blog-entry-title", firstDocument.SourceLink);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 0, 0), firstDocument.PublishTime);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 1, 0), firstDocument.UpdateTime);

            var secondDocument = repository.GetDocuments().First(doc => doc.SourceID == "12346");

            Assert.AreEqual("Test Blog Entry Title 2", secondDocument.Title);
            Assert.AreEqual("<p>This is the second test blog entry content.</p>", secondDocument.Content);
            Assert.AreEqual("12346", secondDocument.SourceID);
            Assert.AreEqual("https://example.com/testblog/test-blog-entry-title-2", secondDocument.SourceLink);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 0, 30), secondDocument.PublishTime);
            Assert.AreEqual(Instant.FromUtc(2021, 2, 27, 0, 30), secondDocument.UpdateTime);
        }

        [Test]
        public void TenPostsInFreshRepository()
        {
            using (var reader = GetAtomResourceXmlReader("tenPosts.xml"))
            {
                aggregator_server.Poller.ParseAtomFeedAndUpdateRepository(repository, reader);
            }

            Assert.AreEqual(10, repository.GetDocuments().Count());
        }

        [Test]
        public void TestPostFeedUpdate()
        {
            using (var reader = GetAtomResourceXmlReader("tenPosts.xml"))
            {
                aggregator_server.Poller.ParseAtomFeedAndUpdateRepository(repository, reader);
            }

            using (var reader = GetAtomResourceXmlReader("tenPosts_3to12.xml"))
            {
                aggregator_server.Poller.ParseAtomFeedAndUpdateRepository(repository, reader);
            }

            Assert.AreEqual(12, repository.GetDocuments().Count());
        }
    }
}

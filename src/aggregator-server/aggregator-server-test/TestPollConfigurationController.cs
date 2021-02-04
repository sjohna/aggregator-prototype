using aggregator_server;
using aggregator_server.Controllers;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using aggregator_server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;
using System.IO;
using LiteDB;

namespace aggregator_server_test
{
    [TestFixture]
    public class TestPollConfigurationController
    {
        private IPollConfigurationRepository repository;
        private MemoryStream databaseStream;
        private LiteDatabase database;

        [SetUp]
        public void SetUp()
        {
            databaseStream = new MemoryStream();
            database = new LiteDatabase(databaseStream);
            repository = new LiteDBPollConfigurationRepository(database);
        }

        [TearDown]
        public void TearDown()
        {
            if (repository != null) repository.Dispose();
            if (database != null) database.Dispose();
            if (databaseStream != null) databaseStream.Dispose();
        }
        // use this property each time a controller method is called to simulate how the framework actually uses it: a new instance is created for each request
        private PollConfigurationController Controller => new PollConfigurationController(repository);

        [Test]
        public void InInitialConditionsGetReturnsNoConfigurations()
        {
            Assert.AreEqual(0, Controller.Get().Count());
        }

        [Test]
        public void PostSingleConfiguration()
        {
            var newConfigurationInput = new PollConfigurationTransferObject()
            {
                URL = "test",
                PollIntervalMinutes = 3
            };

            var result = Controller.Post(newConfigurationInput) as ObjectResult;

            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
            Assert.IsNotNull(result.Value);

            var retValue = result.Value as PollConfiguration;

            Assert.AreEqual("test", retValue.URL);
            Assert.AreEqual(3, retValue.PollIntervalMinutes);
        }

        [Test]
        public void GetAfterAddingSingleConfiguration()
        {
            PostSingleConfiguration();

            var configurations = Controller.Get().ToList();

            Assert.AreEqual(1, configurations.Count);

            var configuration = configurations[0];

            Assert.AreEqual("test", configuration.URL);
            Assert.AreEqual(3, configuration.PollIntervalMinutes);
        }

        [Test]
        public void PostInvalidConfiguration_NoUrl()
        {
            var newConfigurationInput = new PollConfigurationTransferObject()
            {
                URL = null,
                PollIntervalMinutes = 3
            };

            var result = Controller.Post(newConfigurationInput) as IStatusCodeActionResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public void PostInvalidConfiguration_NoPollIntervalMinutes()
        {
            var newConfigurationInput = new PollConfigurationTransferObject()
            {
                URL = "test",
                PollIntervalMinutes = null
            };

            var result = Controller.Post(newConfigurationInput) as IStatusCodeActionResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public void PostInvalidConfiguration_PollIntervalMinutesIsNegative()
        {
            var newConfigurationInput = new PollConfigurationTransferObject()
            {
                URL = "test",
                PollIntervalMinutes = -1
            };

            var result = Controller.Post(newConfigurationInput) as IStatusCodeActionResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }
    }
}

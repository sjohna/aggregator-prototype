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
using System;

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
            Assert.IsTrue(retValue.Active);
        }

        [Test]
        public void PostSingleConfigurationExplicitlyActive()
        {
            var newConfigurationInput = new PollConfigurationTransferObject()
            {
                URL = "test",
                PollIntervalMinutes = 3,
                Active = true
            };

            var result = Controller.Post(newConfigurationInput) as ObjectResult;

            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
            Assert.IsNotNull(result.Value);

            var retValue = result.Value as PollConfiguration;

            Assert.AreEqual("test", retValue.URL);
            Assert.AreEqual(3, retValue.PollIntervalMinutes);
            Assert.IsTrue(retValue.Active);
        }

        [Test]
        public void PostSingleInactiveConfiguration()
        {
            var newConfigurationInput = new PollConfigurationTransferObject()
            {
                URL = "test",
                PollIntervalMinutes = 3,
                Active = false
            };

            var result = Controller.Post(newConfigurationInput) as ObjectResult;

            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
            Assert.IsNotNull(result.Value);

            var retValue = result.Value as PollConfiguration;

            Assert.AreEqual("test", retValue.URL);
            Assert.AreEqual(3, retValue.PollIntervalMinutes);
            Assert.IsFalse(retValue.Active);
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

        [Test]
        public void PostConfigurationWithDuplicateURL()
        {
            var newConfigurationInput = new PollConfigurationTransferObject()
            {
                URL = "test",
                PollIntervalMinutes = 3
            };

            Controller.Post(newConfigurationInput);
            var result = Controller.Post(newConfigurationInput) as IStatusCodeActionResult;

            Assert.AreEqual((int)HttpStatusCode.Conflict, result.StatusCode);
        }

        [Test]
        public void GetPresentPollConfiguration()
        {
            var configuration = repository.AddConfiguration("test", 1, false);

            var result = Controller.Get(configuration.ID) as ObjectResult;

            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);

            var gottenConfiguration = result.Value as PollConfiguration;

            Assert.AreEqual(configuration.ID, gottenConfiguration.ID);
            Assert.AreEqual(configuration.URL, gottenConfiguration.URL);
            Assert.AreEqual(configuration.PollIntervalMinutes, gottenConfiguration.PollIntervalMinutes);
            Assert.AreEqual(configuration.Active, gottenConfiguration.Active);
        }

        [Test]
        public void GetByIDWithEmptyRepository()
        {
            var result = Controller.Get(Guid.NewGuid()) as IStatusCodeActionResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Test]
        public void DeleteSingleConfiguration()
        {
            var configuration = repository.AddConfiguration("test", 1, false);

            var result = Controller.Delete(configuration.ID) as IStatusCodeActionResult;

            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);

            Assert.AreEqual(0, repository.GetConfigurations().Count());
        }

        [Test]
        public void DeleteOneOfTwoConfigurations()
        {
            var config1 = repository.AddConfiguration("test", 1, false);
            var config2 = repository.AddConfiguration("test2", 1, false);

            var deleteResult = Controller.Delete(config2.ID) as IStatusCodeActionResult;

            Assert.AreEqual((int)HttpStatusCode.OK, deleteResult.StatusCode);

            Assert.AreEqual(1, repository.GetConfigurations().Count());

            var getResult = Controller.Get(config1.ID) as ObjectResult;

            Assert.AreEqual((int)HttpStatusCode.OK, getResult.StatusCode);

            config1.AssertEqualTo(getResult.Value as PollConfiguration);
        }

        [Test]
        public void DeleteWithEmptyRepository()
        {
            var deleteResult = Controller.Delete(Guid.NewGuid()) as IStatusCodeActionResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, deleteResult.StatusCode);
        }

        [Test]
        public void DeleteInvalidID()
        {
            var configuration = repository.AddConfiguration("test", 1, false);

            var deleteResult = Controller.Delete(Guid.NewGuid()) as IStatusCodeActionResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, deleteResult.StatusCode);

            var getResult = Controller.Get(configuration.ID) as ObjectResult;

            Assert.AreEqual((int)HttpStatusCode.OK, getResult.StatusCode);

            configuration.AssertEqualTo(getResult.Value as PollConfiguration);
        }

        [Test]
        public void UpdateConfiguration_AllParameters()
        {
            var configuration = repository.AddConfiguration("test", 1, false);

            var updateConfigurationInput = new PollConfigurationTransferObject()
            {
                PollIntervalMinutes = 7,
                Active = true
            };

            var updateResult = Controller.Put(configuration.ID, updateConfigurationInput) as ObjectResult;

            Assert.AreEqual((int)HttpStatusCode.OK, updateResult.StatusCode);

            var resultParameter = updateResult.Value as PollConfiguration;

            Assert.AreEqual(7, resultParameter.PollIntervalMinutes);
            Assert.IsTrue(resultParameter.Active);
        }

        [Test]
        public void UpdateConfiguration_Active()
        {
            var configuration = repository.AddConfiguration("test", 1, false);

            var updateConfigurationInput = new PollConfigurationTransferObject()
            {
                Active = true
            };

            var updateResult = Controller.Put(configuration.ID, updateConfigurationInput) as ObjectResult;

            Assert.AreEqual((int)HttpStatusCode.OK, updateResult.StatusCode);

            var resultParameter = updateResult.Value as PollConfiguration;

            Assert.AreEqual(1, resultParameter.PollIntervalMinutes);
            Assert.IsTrue(resultParameter.Active);
        }

        [Test]
        public void UpdateConfiguration_PollIntervalMinutes()
        {
            var configuration = repository.AddConfiguration("test", 1, false);

            var updateConfigurationInput = new PollConfigurationTransferObject()
            {
                PollIntervalMinutes = 7
            };

            var updateResult = Controller.Put(configuration.ID, updateConfigurationInput) as ObjectResult;

            Assert.AreEqual((int)HttpStatusCode.OK, updateResult.StatusCode);

            var resultParameter = updateResult.Value as PollConfiguration;

            Assert.AreEqual(7, resultParameter.PollIntervalMinutes);
            Assert.IsFalse(resultParameter.Active);
        }

        [Test]
        public void UpdateInvalidID()
        {
            var configuration = repository.AddConfiguration("test", 1, false);

            var updateConfigurationInput = new PollConfigurationTransferObject()
            {
                PollIntervalMinutes = 7,
                Active = true
            };

            var updateResult = Controller.Put(Guid.NewGuid() , updateConfigurationInput) as ObjectResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, updateResult.StatusCode);
        }

        [Test]
        public void UpdateConfiguration_InvalidPollInterval()
        {
            var configuration = repository.AddConfiguration("test", 1, false);

            var updateConfigurationInput = new PollConfigurationTransferObject()
            {
                PollIntervalMinutes = 0,
                Active = true
            };

            var updateResult = Controller.Put(configuration.ID, updateConfigurationInput) as ObjectResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, updateResult.StatusCode);
        }
    }
}

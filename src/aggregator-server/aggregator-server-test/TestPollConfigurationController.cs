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

namespace aggregator_server_test
{
    [TestFixture]
    public class TestPollConfigurationController
    {
        private IPollConfigurationRepository repository;

        [SetUp]
        public void SetUp()
        {
            repository = new InMemoryPollConfigurationRepository();
        }

        [TearDown]
        public void TearDown()
        {
            repository.Dispose();
        }

        // use this property each time a controller method is called to simulate how the framework actually uses it: a new instance is created for each request
        private PollConfigurationController Controller => new PollConfigurationController(repository);

        [Test]
        public void InInitialConditionsGetReturnsNoConfigurations()
        {
            Assert.AreEqual(0, Controller.Get().Count());
        }

        [Test]
        public void AddSingleConfiguration()
        {
            var newConfigurationInput = new PollConfiguration()
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
            AddSingleConfiguration();

            var configurations = Controller.Get().ToList();

            Assert.AreEqual(1, configurations.Count);

            var configuration = configurations[0];

            Assert.AreEqual("test", configuration.URL);
            Assert.AreEqual(3, configuration.PollIntervalMinutes);
        }
    }
}

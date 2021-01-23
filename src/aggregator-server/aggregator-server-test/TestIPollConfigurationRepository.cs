using aggregator_server;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace aggregator_server_test
{
    [TestFixture(typeof(InMemoryPollConfigurationRepository))]
    public class TestIPollConfigurationRepository<TRepository> where TRepository : IPollConfigurationRepository, new()
    {
        private TRepository repository;

        [SetUp]
        public void SetUp()
        {
            repository = new TRepository();
        }

        [TearDown]
        public void TearDown()
        {
            repository.Dispose();
        }

        [Test]
        public void InitialRepositoryState()
        {
            Assert.AreEqual(0, repository.GetConfigurations().Count());
        }

        [Test]
        public void AddOnePollConfiguration()
        {
            repository.AddConfiguration("test", 7);

            Assert.AreEqual(1, repository.GetConfigurations().Count());

            var configuration = repository.GetConfigurations().First();

            Assert.AreEqual("test", configuration.URL);
            Assert.AreEqual(7, configuration.PollIntervalMinutes);
        }

        [Test]
        public void TwoAddedConfigurationsHaveDistinctIDs()
        {
            repository.AddConfiguration("test1", 7);
            repository.AddConfiguration("test2", 7);

            var configurations = repository.GetConfigurations().ToList();

            Assert.AreNotEqual(configurations[0].ID, configurations[1].ID);
        }


    }
}

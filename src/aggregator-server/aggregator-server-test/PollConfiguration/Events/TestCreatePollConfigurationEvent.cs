using aggregator_server.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace aggregator_server_test
{
    [TestFixture]
    class TestCreatePollConfigurationEvent
    {
        [Test]
        public void AffectedEntityType()
        {
            var e = new CreatePollConfigurationEvent(Guid.NewGuid(),2,"",false);

            Assert.AreEqual(EntityEvent.EntityType.PollConfiguration, e.AffectedEntityType);
        }

        [Test]
        public void CreateEntity()
        {
            var id = Guid.NewGuid();
            var e = new CreatePollConfigurationEvent(id, 10, "testURL", true);

            var pollConfiguration = e.CreateEntity();

            Assert.AreEqual(id, pollConfiguration.ID);
            Assert.AreEqual(10, pollConfiguration.PollIntervalMinutes);
            Assert.AreEqual("testURL", pollConfiguration.URL);
            Assert.AreEqual(true, pollConfiguration.Active);
        }

    }
}

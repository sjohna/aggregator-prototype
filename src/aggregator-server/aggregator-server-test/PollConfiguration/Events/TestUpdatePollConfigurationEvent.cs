using aggregator_server.Exceptions;
using aggregator_server.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace aggregator_server_test
{
    [TestFixture]
    class TestUpdatePollConfigurationEvent
    {
        [Test]
        public void AffectedEntityType()
        {
            var e = new UpdatePollConfigurationEvent(Guid.NewGuid());

            Assert.AreEqual(EntityEvent.EntityType.PollConfiguration, e.AffectedEntityType);
        }

        [Test]
        public void UpdateURL()
        {
            var config = new PollConfiguration(Guid.NewGuid(), 10, "test", true);

            var e = new UpdatePollConfigurationEvent(config.ID);

            e.URL = "new URL";

            e.UpdateEntity(config);

            Assert.AreEqual(10, config.PollIntervalMinutes);
            Assert.AreEqual("new URL", config.URL);
            Assert.AreEqual(true, config.Active);
        }

        [Test]
        public void UpdatePollIntervalMinutes()
        {
            var config = new PollConfiguration(Guid.NewGuid(), 10, "test", true);

            var e = new UpdatePollConfigurationEvent(config.ID);

            e.PollIntervalMinutes = 15;

            e.UpdateEntity(config);

            Assert.AreEqual(15, config.PollIntervalMinutes);
            Assert.AreEqual("test", config.URL);
            Assert.AreEqual(true, config.Active);
        }

        [Test]
        public void UpdateActive()
        {
            var config = new PollConfiguration(Guid.NewGuid(), 10, "test", true);

            var e = new UpdatePollConfigurationEvent(config.ID);

            e.Active = false;

            e.UpdateEntity(config);

            Assert.AreEqual(10, config.PollIntervalMinutes);
            Assert.AreEqual("test", config.URL);
            Assert.AreEqual(false, config.Active);
        }

        [Test]
        public void UpdateAllFields()
        {
            var config = new PollConfiguration(Guid.NewGuid(), 10, "test", true);

            var e = new UpdatePollConfigurationEvent(config.ID);

            e.Active = false;
            e.PollIntervalMinutes = 15;
            e.URL = "new URL";

            e.UpdateEntity(config);

            Assert.AreEqual(15, config.PollIntervalMinutes);
            Assert.AreEqual("new URL", config.URL);
            Assert.AreEqual(false, config.Active);
        }

        [Test]
        public void UpdateNoFieldsThrowsException()
        {
            var config = new PollConfiguration(Guid.NewGuid(), 10, "test", true);

            var e = new UpdatePollConfigurationEvent(config.ID);

            Assert.Throws<EventException>(() => e.UpdateEntity(config));

            Assert.AreEqual(10, config.PollIntervalMinutes);
            Assert.AreEqual("test", config.URL);
            Assert.AreEqual(true, config.Active);
        }

        [Test]
        public void UpdateWrongEntityThrowsException()
        {
            var config = new PollConfiguration(Guid.NewGuid(), 10, "test", true);

            var e = new UpdatePollConfigurationEvent(Guid.NewGuid());

            e.URL = "new URL";

            Assert.Throws<EventException>(() => e.UpdateEntity(config));

            Assert.AreEqual(10, config.PollIntervalMinutes);
            Assert.AreEqual("test", config.URL);
            Assert.AreEqual(true, config.Active);
        }
    }
}

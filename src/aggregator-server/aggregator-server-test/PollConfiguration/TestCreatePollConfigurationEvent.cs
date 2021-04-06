﻿using aggregator_server.Models;
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
        public void EventType()
        {
            var e = new CreatePollConfigurationEvent(1,2,"",false);

            Assert.AreEqual(EntityEvent.EntityType.PollConfiguration, e.AffectedEntityType);
        }

        [Test]
        public void CreateEntity()
        {
            var e = new CreatePollConfigurationEvent(1, 10, "testURL", true);

            var pollConfiguration = e.CreateEntity();

            Assert.AreEqual(1, pollConfiguration.ID);
            Assert.AreEqual(10, pollConfiguration.PollIntervalMinutes);
            Assert.AreEqual("testURL", pollConfiguration.URL);
            Assert.AreEqual(true, pollConfiguration.Active);
        }

    }
}
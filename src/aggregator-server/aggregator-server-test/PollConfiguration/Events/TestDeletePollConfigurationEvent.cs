using aggregator_server.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace aggregator_server_test
{
    [TestFixture]
    class TestDeletePollConfigurationEvent
    {
        [Test]
        public void AffectedEntityType()
        {
            var e = new DeletePollConfigurationEvent(Guid.NewGuid());

            Assert.AreEqual(EntityEvent.EntityType.PollConfiguration, e.AffectedEntityType);
        }
    }
}

using aggregator_server.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace aggregator_server_test
{
    static class PollConfigurationRepositoryTestExtensions
    {
        public static void AssertEqualTo(this PollConfiguration left, PollConfiguration right)
        {
            Assert.AreEqual(left.ID, right.ID);
            Assert.AreEqual(left.URL, right.URL);
            Assert.AreEqual(left.PollIntervalMinutes, right.PollIntervalMinutes);
            Assert.AreEqual(left.Active, right.Active);

            if (left.LastPollInformation != null && right.LastPollInformation != null)
            {
                Assert.AreEqual(left.LastPollInformation.PolledTime, right.LastPollInformation.PolledTime);
                Assert.AreEqual(left.LastPollInformation.Successful, right.LastPollInformation.Successful);
            }
            else if (left.LastPollInformation != null && right.LastPollInformation == null)
            {
                Assert.Fail($"Expected non-null {nameof(PollConfiguration.LastPollInformation)}, but was null");
            }
            else if (left.LastPollInformation == null && right.LastPollInformation != null)
            {
                Assert.Fail($"Expected null {nameof(PollConfiguration.LastPollInformation)}, but was non-null");
            }
        }
    }
}

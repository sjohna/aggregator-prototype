using aggregator_server.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server
{
    public class InMemoryPollConfigurationRepository : IPollConfigurationRepository
    {
        private ImmutableList<PollConfiguration>.Builder m_configurations = ImmutableList.CreateBuilder<PollConfiguration>();

        int m_nextID = 1;

        public PollConfiguration AddConfiguration(string url, int pollIntervalMinutes)
        {
            var newConfiguration = new PollConfiguration()
            {
                ID = GetNextID(),
                URL = url,
                PollIntervalMinutes = pollIntervalMinutes
            };

            m_configurations.Add(newConfiguration);

            return newConfiguration;
        }

        public void Dispose()
        {
            // nothing to dispose
        }

        public IEnumerable<PollConfiguration> GetConfigurations()
        {
            return m_configurations.ToImmutableList();
        }

        private int GetNextID()
        {
            lock (this)
            {
                return m_nextID++;
            }
        }
    }
}

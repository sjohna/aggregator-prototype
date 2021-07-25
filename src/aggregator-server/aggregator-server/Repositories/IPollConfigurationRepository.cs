using aggregator_server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server
{
    public interface IPollConfigurationRepository : IDisposable
    {
        IEnumerable<PollConfiguration> GetConfigurations();

        PollConfiguration SetConfigurationLastPollInformation(Guid configurationID, PollingInformation info);

        PollConfiguration GetConfiguration(Guid id);

        void ApplyEvent(EntityEvent<PollConfiguration> e);
    }
}

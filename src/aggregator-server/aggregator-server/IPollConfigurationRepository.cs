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

        // returns the added configuration object
        PollConfiguration AddConfiguration(string URL, int pollIntervalMinutes, bool active);

        PollConfiguration SetConfigurationLastPollInformation(Guid configurationID, PollingInformation info);

        PollConfiguration GetConfiguration(Guid id);

        void DeleteConfiguration(Guid id);

        void UpdateConfiguration(PollConfiguration configuration);
    }
}

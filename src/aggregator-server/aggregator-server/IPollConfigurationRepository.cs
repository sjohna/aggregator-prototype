using aggregator_server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server
{
    public interface IPollConfigurationRepository
    {
        IEnumerable<PollConfiguration> GetConfigurations();

        // returns the added configuration object
        PollConfiguration AddConfiguration(string URL, int pollIntervalMinutes);
    }
}

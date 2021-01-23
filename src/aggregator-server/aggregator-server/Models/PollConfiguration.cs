using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server.Models
{
    public class PollConfiguration
    {
        public string URL { get; set; }

        public int PollIntervalMinutes { get; set; }

        public int ID { get; set; }

        public PollingInformation LastPollInformation { get; set; }

        public PollConfiguration() { }

        public PollConfiguration(int id, int pollIntervalMinutes, string url)
        {
            this.ID = id;
            this.PollIntervalMinutes = pollIntervalMinutes;
            this.URL = url;
        }
    }

    public class PollConfigurationTransferObject
    {
        public int? PollIntervalMinutes { get; set; }

        public string URL { get; set; }
    }
}

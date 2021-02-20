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
        public bool Active { get; set; }

        public PollingInformation LastPollInformation { get; set; }

        public PollConfiguration() { }

        public PollConfiguration(int id, int pollIntervalMinutes, string url, bool active)
        {
            this.ID = id;
            this.PollIntervalMinutes = pollIntervalMinutes;
            this.URL = url;
            this.Active = active;
        }
    }

    public class PollConfigurationTransferObject
    {
        public int? PollIntervalMinutes { get; set; }

        public string URL { get; set; }

        public bool? Active { get; set; }

        public override string ToString()   // TODO: use this more widely in logging
        {
            return $"(URL: {URL}, PollIntervalMinutes: {PollIntervalMinutes}, Active: {Active})";
        }
    }
}

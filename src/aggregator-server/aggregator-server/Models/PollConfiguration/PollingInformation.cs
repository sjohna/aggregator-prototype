using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace aggregator_server.Models
{
    public class PollingInformation
    {
        [JsonConverter(typeof(InstantJsonConverter))]
        public Instant PolledTime { get; set; }

        public bool Successful { get; set; }
    }
}

using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace aggregator_server.Models
{
    public class Document
    {
        public Guid ID { get; set; }     // the ID within this database

        public string SourceID { get; set; }    // the ID of the item from the source

        public string Title { get; set; }

        public string Content { get; set; }

        [JsonConverter(typeof(InstantJsonConverter))]
        public Instant PublishTime { get; set; }

        [JsonConverter(typeof(InstantJsonConverter))]
        public Instant UpdateTime { get; set; }

        public string SourceLink { get; set; }
    }
}

using LiteDB;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace aggregator_server.Models
{
    public class AggregatorAction
    {
        public enum ActionOrigin
        {
            User,
            System,
            Admin
        }

        [BsonId]
        public Guid ID { get; private set; }

        public string Description { get; private set; }

        public ActionOrigin Origin { get; private set; }

        [JsonConverter(typeof(InstantJsonConverter))]
        public Instant ActionTime { get; private set; }

        public List<EntityEvent> Events { get; private set; }

        public AggregatorAction() { }

        public AggregatorAction(Guid ID, string Description, ActionOrigin Origin, Instant ActionTime)
        {
            this.ID = ID;
            this.Description = Description;
            this.Origin = Origin;
            this.ActionTime = ActionTime;
            this.Events = new List<EntityEvent>();
        }

        public void AddEvent(EntityEvent Event)
        {
            Events.Add(Event);
        }
    }
}

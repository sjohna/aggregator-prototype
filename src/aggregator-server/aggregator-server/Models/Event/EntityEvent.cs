using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server.Models
{
    // TODO: going to have to figure out how to handle events polymorphically in the REST API... Maybe a transfer object?

    public abstract class EntityEvent
    {
        public enum EntityType
        {
            PollConfiguration,
            Document
        }

        [BsonId]
        public Guid ID { get; protected set; }

        public Guid AffectedEntityID { get; protected set; }

        public EntityType AffectedEntityType { get; protected set; }

        protected EntityEvent(Guid AffectedEntityID, EntityType AffectedEntityType)
        {
            this.ID = Guid.NewGuid();
            this.AffectedEntityID = AffectedEntityID;
            this.AffectedEntityType = AffectedEntityType;
        }

        protected EntityEvent() { }
    }
}

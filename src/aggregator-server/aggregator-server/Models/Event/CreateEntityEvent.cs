using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server.Models
{
    public abstract class CreateEntityEvent<T> : EntityEvent
    {
        protected Dictionary<String, Object> PrivateCreationParameters { get; set; }

        [BsonIgnore]
        public IReadOnlyDictionary<String, Object> CreationParameters => PrivateCreationParameters;

        protected CreateEntityEvent(Guid AffectedEntityID, EntityType AffectedEntityType) : base(AffectedEntityID, AffectedEntityType)
        {

        }

        protected CreateEntityEvent() { }

        public abstract T CreateEntity();

    }
}

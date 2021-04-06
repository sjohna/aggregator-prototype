using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server.Models
{
    public abstract class CreateEntityEvent<T> : EntityEvent
    {
        // TODO: investigate JSON serialization for this...
        public abstract ImmutableDictionary<String, Object> CreationParameters { get; }

        protected CreateEntityEvent(Guid AffectedEntityID, EntityType AffectedEntityType) : base(AffectedEntityID, AffectedEntityType)
        {

        }

        public abstract T CreateEntity();

    }
}

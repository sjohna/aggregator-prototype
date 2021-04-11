using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server.Models
{
    public abstract class UpdateEntityEvent<T> : EntityEvent
    {
        protected Dictionary<String, Object> PrivateUpdateParameters { get; set; }

        [BsonIgnore]
        public IReadOnlyDictionary<String, Object> UpdateParameters => PrivateUpdateParameters;

        protected UpdateEntityEvent(Guid AffectedEntityID, EntityType AffectedEntityType) : base(AffectedEntityID, AffectedEntityType)
        {

        }

        protected UpdateEntityEvent() { }

        public abstract void UpdateEntity(T entity);
    }
}

using aggregator_server.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server.Models
{
    // TODO: think I need a builder for this...
    public class UpdatePollConfigurationEvent : UpdateEntityEvent<PollConfiguration>
    {
        public override void UpdateEntity(PollConfiguration entity)
        {
            // TODO: look at these exception messages
            if (entity.ID != this.AffectedEntityID)
            {
                throw new EventException("Attempted to apply UpdatePollConfigurationEvent to wrong entity.");
            }

            if (UpdateParameters.Count == 0)
            {
                throw new EventException("Attempted to apply UpdatePollConfigurationEvent with no updated fields.");
            }

            if (UpdateParameters.ContainsKey(nameof(PollConfiguration.PollIntervalMinutes)))
            {
                entity.PollIntervalMinutes = (int) UpdateParameters[nameof(PollConfiguration.PollIntervalMinutes)];
            }

            if (UpdateParameters.ContainsKey(nameof(PollConfiguration.Active)))
            {
                entity.Active = (bool) UpdateParameters[nameof(PollConfiguration.Active)];
            }
        }

        public UpdatePollConfigurationEvent(Guid id) : base(id, EntityType.PollConfiguration)
        {
            PrivateUpdateParameters = new Dictionary<string, object>();
        }

        public UpdatePollConfigurationEvent() { }

        public int PollIntervalMinutes
        {
            set
            {
                PrivateUpdateParameters[nameof(PollConfiguration.PollIntervalMinutes)] = value;
            }
        }

        public bool Active
        {
            set
            {
                PrivateUpdateParameters[nameof(PollConfiguration.Active)] = value;
            }
        }
    }
}

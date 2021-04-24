using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server.Models
{
    public class DeletePollConfigurationEvent : EntityEvent<PollConfiguration>
    {
        public DeletePollConfigurationEvent(Guid id) : base(id, EntityType.PollConfiguration)
        {

        }
    }
}

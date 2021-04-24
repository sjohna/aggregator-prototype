using aggregator_server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server.Repositories
{
    interface IEventRepository : IDisposable
    {
        IEnumerable<EntityEvent> GetEvents();

        EntityEvent GetEvent(Guid id);

        IEnumerable<EntityEvent> FindEventsByAffectedEntityId(Guid affectedEntityId);
    }
}

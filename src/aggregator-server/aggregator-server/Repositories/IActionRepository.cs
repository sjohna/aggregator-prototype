using aggregator_server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server.Repositories
{
    public interface IActionRepository : IDisposable
    {
        IEnumerable<AggregatorAction> GetActions();

        AggregatorAction GetAction(Guid id);

        void InsertAction(AggregatorAction action);
    }
}

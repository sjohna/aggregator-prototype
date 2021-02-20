using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server.Exceptions
{
    public class RepositoryItemNotFoundException : RepositoryException
    {
        public RepositoryItemNotFoundException(string message) : base(message) { }
    }
}

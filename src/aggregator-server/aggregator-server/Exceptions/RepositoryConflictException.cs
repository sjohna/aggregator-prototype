using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server.Exceptions
{
    public class RepositoryConflictException : RepositoryException
    {
        public RepositoryConflictException(string message) : base(message) { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server.Exceptions
{
    // TODO: reconsider whether this should be a distinct exception class. In fact, reconsider exception handling as a whole...
    public class EventException : Exception
    {
        public EventException(string message) : base(message)
        {

        }
    }
}

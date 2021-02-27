using aggregator_server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server
{
    public interface IDocumentRepository : IDisposable
    {
        IEnumerable<Document> GetDocuments();

        Document AddDocument(Document doc); // returns the document as added in the repository
    }
}

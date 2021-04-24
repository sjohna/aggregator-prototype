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

        Document GetDocument(Guid id);

        void InsertDocument(Document doc); 

        // TODO: refactor how this is handled...
        void UpdateDocument(Document doc);

        IEnumerable<Document> FindBySourceID(string sourceID);
    }
}

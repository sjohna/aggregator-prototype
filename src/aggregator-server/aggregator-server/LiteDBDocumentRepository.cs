using aggregator_server.Models;
using LiteDB;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server
{
    public class LiteDBDocumentRepository : IDocumentRepository
    {
        private static readonly ILog configLog = LogManager.GetLogger($"Config.{typeof(LiteDBDocumentRepository)}");
        private static readonly ILog log = LogManager.GetLogger(typeof(LiteDBDocumentRepository));

        private readonly string DocumentCollectionName = "Documents";

        private LiteDatabase database;

        public LiteDBDocumentRepository(LiteDatabase database)
        {
            this.database = database;

            EnsureDatabaseConfiguration();
        }

        private void EnsureDatabaseConfiguration()
        {
            if (!database.CollectionExists(DocumentCollectionName))
                configLog.Info($"Collection {DocumentCollectionName} does not exist in Lite DB and will be created.");

            var collection = database.GetCollection<Document>(DocumentCollectionName);

            if (collection.EnsureIndex(document => document.ID))    // TODO: Do I really need to create an index on the ID explicitly? I don't think so...
                configLog.Info($"Collection {DocumentCollectionName}: ID index created");

            if (collection.EnsureIndex(document => document.ID))
                configLog.Info($"Collection {DocumentCollectionName}: SourceID index created");

            var mapper = BsonMapper.Global;
            mapper.Entity<PollConfiguration>().Id(x => x.ID);
        }

        public Document AddDocument(Document doc)
        {
            database.GetCollection<Document>(DocumentCollectionName).Insert(doc);

            return doc;
        }

        public IEnumerable<Document> GetDocuments()
        {
            return database.GetCollection<Document>(DocumentCollectionName).FindAll();    // TODO: do I need to copy this to its own list?
        }

        public void UpdateDocument(Document doc)
        {
            database.GetCollection<Document>(DocumentCollectionName).Update(doc);
        }

        public IEnumerable<Document> FindBySourceID(string sourceID)
        {
            return database.GetCollection<Document>(DocumentCollectionName).Find(doc => doc.SourceID == sourceID);  // TODO: do I need to copy this to its own list?
        }

        public void Dispose()
        {
            
        }
    }
}

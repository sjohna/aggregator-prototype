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

            configLog.Info($"Collection {DocumentCollectionName} contains {collection.Count()} records.");

            if (collection.EnsureIndex(document => document.SourceID))
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
            return database.GetCollection<Document>(DocumentCollectionName).FindAll();
        }

        public void UpdateDocument(Document doc)
        {
            database.GetCollection<Document>(DocumentCollectionName).Update(doc);
        }

        public IEnumerable<Document> FindBySourceID(string sourceID)
        {
            return database.GetCollection<Document>(DocumentCollectionName).Find(doc => doc.SourceID == sourceID);
        }

        public void Dispose()
        {
            
        }
    }
}

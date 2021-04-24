using aggregator_server.Exceptions;
using aggregator_server.Models;
using LiteDB;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server.Repositories
{
    public class LiteDBActionEventRepository : IActionRepository, IEventRepository
    {
        private static readonly ILog configLog = LogManager.GetLogger($"Config.{typeof(LiteDBActionEventRepository)}");
        private static readonly ILog log = LogManager.GetLogger(typeof(LiteDBActionEventRepository));

        private readonly string ActionCollectionName = "Actions";
        private readonly string EventCollectionName = "Events";

        private LiteDatabase database;

        private ILiteCollection<AggregatorAction> ActionCollection => database.GetCollection<AggregatorAction>(ActionCollectionName);
        private ILiteCollection<EntityEvent> EventCollection => database.GetCollection<EntityEvent>(EventCollectionName);

        public LiteDBActionEventRepository(LiteDatabase database)
        {
            this.database = database;

            EnsureDatabaseConfiguration();
        }

        private void EnsureDatabaseConfiguration()
        {
            if (!database.CollectionExists(ActionCollectionName))
                configLog.Info($"Collection {ActionCollectionName} does not exist in Lite DB and will be created.");

            var actionCollection = ActionCollection;

            configLog.Info($"Collection {ActionCollectionName} contains {actionCollection.Count()} records.");

            if (actionCollection.EnsureIndex(action => action.Origin))
                configLog.Info($"Collection {ActionCollectionName}: Origin index created");

            if (!database.CollectionExists(EventCollectionName))
                configLog.Info($"Collection {EventCollectionName} does not exist in Lite DB and will be created.");

            var eventCollection = EventCollection;

            configLog.Info($"Collection {EventCollectionName} contains {eventCollection.Count()} records.");

            if (eventCollection.EnsureIndex(e => e.AffectedEntityID))
                configLog.Info($"Collection {EventCollectionName}: AffectedEntityID index created");

            if (eventCollection.EnsureIndex(e => e.AffectedEntityType))
                configLog.Info($"Collection {EventCollectionName}: AffectedEntityType index created");
        }

        public void Dispose()
        {
            
        }

        public AggregatorAction GetAction(Guid id)
        {
            var action = ActionCollection.FindById(id);

            if (action == null)
            {
                log.Info($"GetAction: Action ID {id} not present in {ActionCollectionName} collection.");
                throw new RepositoryItemNotFoundException($"Action ID {id} not present.");
            }

            return action;
        }

        public IEnumerable<AggregatorAction> GetActions()
        {
            return ActionCollection.Include(x => x.Events).FindAll();
        }

        public EntityEvent GetEvent(Guid id)
        {
            var entityEvent = EventCollection.FindById(id);

            if (entityEvent == null)
            {
                log.Info($"GetVent: Event ID {id} not present in {EventCollectionName} collection.");
                throw new RepositoryItemNotFoundException($"Event ID {id} not present.");
            }

            return entityEvent;
        }

        public IEnumerable<EntityEvent> GetEvents()
        {
            return EventCollection.FindAll();
        }

        public IEnumerable<EntityEvent> FindEventsByAffectedEntityId(Guid affectedEntityId)
        {
            return EventCollection.Query().Where(e => e.AffectedEntityID == affectedEntityId).ToEnumerable();
        }

        public void InsertAction(AggregatorAction action)
        {
            if (action.ID == default)
            {
                log.Warn($"Attempted to insert action into {ActionCollectionName} with default ID.");
                throw new RepositoryConflictException($"Action has default ID.");
            }

            try
            {
                EventCollection.InsertBulk(action.Events);
            }
            catch (LiteException le)
            {
                log.Warn($"LiteException adding events for Action ID {action.ID} to {EventCollectionName}: {le.Message}");
                throw new RepositoryConflictException($"Failed to add ID {action.ID} to repository.");
            }

            try
            {
                ActionCollection.Insert(action);
            }
            catch (LiteException le)
            {
                log.Warn($"LiteException adding ID {action.ID} to {ActionCollectionName}: {le.Message}");
                throw new RepositoryConflictException($"Failed to add ID {action.ID} to repository.");
            }
        }
    }
}

using aggregator_server.Exceptions;
using aggregator_server.Models;
using LiteDB;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server
{
    public class LiteDBPollConfigurationRepository : IPollConfigurationRepository
    {
        private static readonly ILog configLog = LogManager.GetLogger($"Config.{typeof(LiteDBPollConfigurationRepository)}");
        private static readonly ILog log = LogManager.GetLogger(typeof(LiteDBPollConfigurationRepository));

        private readonly string PollConfigurationCollectionName = "PollConfigurations";

        private LiteDatabase database;

        public LiteDBPollConfigurationRepository(LiteDatabase database)
        {
            this.database = database;

            EnsureDatabaseConfiguration();
        }

        private void EnsureDatabaseConfiguration()
        {
            if (!database.CollectionExists(PollConfigurationCollectionName))
                configLog.Info($"Collection {PollConfigurationCollectionName} does not exist in Lite DB and will be created.");

            var collection = database.GetCollection<PollConfiguration>(PollConfigurationCollectionName);

            if (collection.EnsureIndex(configuration => configuration.URL))
                configLog.Info($"Collection {PollConfigurationCollectionName}: URL index created");

            if (collection.EnsureIndex(configuration => configuration.ID))
                configLog.Info($"Collection {PollConfigurationCollectionName}: ID index created");

            var mapper = BsonMapper.Global;
            mapper.Entity<PollConfiguration>().Id(x => x.ID);
        }

        public PollConfiguration AddConfiguration(string url, int pollIntervalMinutes, bool active)
        {
            lock (database)
            {
                var newConfiguration = new PollConfiguration()
                {
                    URL = url,
                    PollIntervalMinutes = pollIntervalMinutes,
                    Active = active
                };

                var collection = database.GetCollection<PollConfiguration>(PollConfigurationCollectionName);

                if (collection.Find(config => config.URL == url).Any())
                {
                    throw new RepositoryConflictException($"Configuration already exists for URL {url}");
                }

                database.GetCollection<PollConfiguration>(PollConfigurationCollectionName).Insert(newConfiguration);

                return newConfiguration;
            }
        }

        public void Dispose()
        {
            
        }

        public IEnumerable<PollConfiguration> GetConfigurations()
        {
            return database.GetCollection<PollConfiguration>(PollConfigurationCollectionName).FindAll();    // TODO: do I need to copy this to its own list?
        }

        public PollConfiguration SetConfigurationLastPollInformation(int configurationID, PollingInformation info)
        {
            lock(database)
            {
                var configuration = GetConfiguration(configurationID);

                configuration.LastPollInformation = info;

                var updateSuccessful = database.GetCollection<PollConfiguration>(PollConfigurationCollectionName).Update(configuration);

                if (!updateSuccessful)
                {
                    log.Warn($"Attempted update of LastPollInformation for configuration ID {configuration.ID} failed!");
                    throw new RepositoryException($"Attempted update of LastPollInformation for configuration ID {configuration.ID} failed!");
                }

                log.Info($"Updated configuration ID {configuration.ID} LastPollInformation to {info}"); // TODO: ToString for PollingInformation

                return GetConfiguration(configurationID);
            }
        }

        public PollConfiguration GetConfiguration(int id)
        {
            var configuration = database.GetCollection<PollConfiguration>(PollConfigurationCollectionName).FindById(id);

            if (configuration == null)
                throw new RepositoryItemNotFoundException($"PollConfiguration ID {id} not present in repository.");

            return configuration;
        }

        public void DeleteConfiguration(int id)
        {
            if(!database.GetCollection<PollConfiguration>(PollConfigurationCollectionName).Delete(id))
            {
                throw new RepositoryItemNotFoundException($"PollConfiguration ID {id} not present in repository.");
            }
        }

        public void UpdateConfiguration(PollConfiguration configuration)
        {
            lock (database)
            {
                var configInDatabase = GetConfiguration(configuration.ID);

                configInDatabase.PollIntervalMinutes = configuration.PollIntervalMinutes;
                configInDatabase.Active = configuration.Active;

                bool updated = database.GetCollection<PollConfiguration>(PollConfigurationCollectionName).Update(configInDatabase);

                if (!updated)
                {
                    throw new RepositoryItemNotFoundException($"PollConfiguration ID {configuration.ID} not present in repository.");
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server.Models
{
    public class CreatePollConfigurationEvent : CreateEntityEvent<PollConfiguration>
    {
        public override PollConfiguration CreateEntity()
        {
            var ID = (Guid) CreationParameters[nameof(PollConfiguration.ID)];
            var PollIntervalMinutes = (int)CreationParameters[nameof(PollConfiguration.PollIntervalMinutes)];
            var URL = CreationParameters[nameof(PollConfiguration.URL)] as String;
            var Active = (bool)CreationParameters[nameof(PollConfiguration.Active)];

            return new PollConfiguration(ID, PollIntervalMinutes, URL, Active);
        }

        public CreatePollConfigurationEvent()
        {

        }

        public CreatePollConfigurationEvent(Guid id, int pollIntervalMinutes, string url, bool active) : base(id, EntityType.PollConfiguration)
        {
            PrivateCreationParameters = new Dictionary<string, object>();

            PrivateCreationParameters.Add(nameof(PollConfiguration.ID), id);
            PrivateCreationParameters.Add(nameof(PollConfiguration.PollIntervalMinutes), pollIntervalMinutes);
            PrivateCreationParameters.Add(nameof(PollConfiguration.URL), url);
            PrivateCreationParameters.Add(nameof(PollConfiguration.Active), active);
        }
    }
}

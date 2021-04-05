using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server.Models
{
    public class CreatePollConfigurationEvent : CreateEntityEvent<PollConfiguration>
    {
        public override ImmutableDictionary<String, Object> CreationParameters { get; }

        public override PollConfiguration CreateEntity()
        {
            var ID = (int) CreationParameters[nameof(PollConfiguration.ID)];
            var PollIntervalMinutes = (int)CreationParameters[nameof(PollConfiguration.PollIntervalMinutes)];
            var URL = CreationParameters[nameof(PollConfiguration.URL)] as String;
            var Active = (bool)CreationParameters[nameof(PollConfiguration.Active)];

            return new PollConfiguration(ID, PollIntervalMinutes, URL, Active);
        }

        public CreatePollConfigurationEvent(int id, int pollIntervalMinutes, string url, bool active) : base(id, EntityType.PollConfiguration)
        {
            ImmutableDictionary<String, Object>.Builder CreationParametersBuilder = ImmutableDictionary.CreateBuilder<String, Object>();

            CreationParametersBuilder.Add(nameof(PollConfiguration.ID), id);
            CreationParametersBuilder.Add(nameof(PollConfiguration.PollIntervalMinutes), pollIntervalMinutes);
            CreationParametersBuilder.Add(nameof(PollConfiguration.URL), url);
            CreationParametersBuilder.Add(nameof(PollConfiguration.Active), active);

            CreationParameters = CreationParametersBuilder.ToImmutable();
        }
    }
}

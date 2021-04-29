using aggregator_server.Exceptions;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server.Models
{
    public class UpdateDocumentEvent : UpdateEntityEvent<Document>
    {
        public UpdateDocumentEvent(Guid DocumentID) : base(DocumentID, EntityType.Document)
        {
            PrivateUpdateParameters = new Dictionary<string, object>();
        }

        public override void UpdateEntity(Document entity)
        {
            // TODO: look at these exception messages
            if (entity.ID != this.AffectedEntityID)
            {
                throw new EventException("Attempted to apply UpdateDocumentEvent to wrong entity.");
            }

            if (UpdateParameters.Count == 0)
            {
                throw new EventException("Attempted to apply UpdateDocumentEvent with no updated fields.");
            }

            if (UpdateParameters.ContainsKey(nameof(Document.SourceID)))
            {
                entity.SourceID = (string) UpdateParameters[nameof(Document.SourceID)];
            }

            if (UpdateParameters.ContainsKey(nameof(Document.Title)))
            {
                entity.Title = (string) UpdateParameters[nameof(Document.Title)];
            }

            if (UpdateParameters.ContainsKey(nameof(Document.Content)))
            {
                entity.Content = (string) UpdateParameters[nameof(Document.Content)];
            }

            if (UpdateParameters.ContainsKey(nameof(Document.PublishTime)))
            {
                entity.PublishTime = (Instant) UpdateParameters[nameof(Document.PublishTime)];
            }

            if (UpdateParameters.ContainsKey(nameof(Document.UpdateTime)))
            {
                entity.UpdateTime = (Instant) UpdateParameters[nameof(Document.UpdateTime)];
            }

            if (UpdateParameters.ContainsKey(nameof(Document.SourceLink)))
            {
                entity.SourceLink = (string) UpdateParameters[nameof(Document.SourceLink)];
            }
        }

        public string SourceID
        {
            set
            {
                PrivateUpdateParameters[nameof(Document.SourceID)] = value;
            }
        }

        public string Title
        {
            set
            {
                PrivateUpdateParameters[nameof(Document.Title)] = value;
            }
        }

        public string Content
        {
            set
            {
                PrivateUpdateParameters[nameof(Document.Content)] = value;
            }
        }

        public Instant PublishTime
        {
            set
            {
                PrivateUpdateParameters[nameof(Document.PublishTime)] = value;
            }
        }

        public Instant UpdateTime
        {
            set
            {
                PrivateUpdateParameters[nameof(Document.UpdateTime)] = value;
            }
        }

        public string SourceLink
        {
            set
            {
                PrivateUpdateParameters[nameof(Document.SourceLink)] = value;
            }
        }
    }
}

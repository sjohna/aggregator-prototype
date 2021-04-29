using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server.Models
{
    public class CreateDocumentEvent : CreateEntityEvent<Document>
    {
        public CreateDocumentEvent(Guid DocumentID,
                                   string SourceID,
                                   string Title,
                                   string Content,
                                   Instant PublishTime,
                                   Instant UpdateTime,
                                   string SourceLink) : base(DocumentID, EntityEvent.EntityType.Document)
        {
            PrivateCreationParameters = new Dictionary<string, object>();

            PrivateCreationParameters.Add(nameof(Document.ID), DocumentID);
            PrivateCreationParameters.Add(nameof(Document.SourceID), SourceID);
            PrivateCreationParameters.Add(nameof(Document.Title), Title);
            PrivateCreationParameters.Add(nameof(Document.Content), Content);
            PrivateCreationParameters.Add(nameof(Document.PublishTime), PublishTime);
            PrivateCreationParameters.Add(nameof(Document.UpdateTime), UpdateTime);
            PrivateCreationParameters.Add(nameof(Document.SourceLink), SourceLink);
        }

        public override Document CreateEntity()
        {
            var ID = (Guid) CreationParameters[nameof(Document.ID)];
            var SourceID = (string) CreationParameters[nameof(Document.SourceID)];
            var Title = (string) CreationParameters[nameof(Document.Title)];
            var Content = (string) CreationParameters[nameof(Document.Content)];
            var PublishTime = (Instant) CreationParameters[nameof(Document.PublishTime)];
            var UpdateTime = (Instant) CreationParameters[nameof(Document.UpdateTime)];
            var SourceLink = (string) CreationParameters[nameof(Document.SourceLink)];


            return new Document()
            {
                ID = ID,
                SourceID = SourceID,
                Title = Title,
                Content = Content,
                PublishTime = PublishTime,
                UpdateTime = UpdateTime,
                SourceLink = SourceLink
            };
        }
    }
}

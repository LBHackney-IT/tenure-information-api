using Hackney.Core.JWT;
using System;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Domain.Sns;
using TenureInformationApi.V1.Infrastructure;

namespace TenureInformationApi.V1.Factories
{
    public class TenureSnsFactory : ISnsFactory
    {
        public TenureSns Create(TenureInformation tenure, Token token)
        {
            return new TenureSns
            {
                CorrelationId = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                EntityId = tenure.Id,
                Id = Guid.NewGuid(),
                EventType = CreateEventConstants.EVENTTYPE,
                Version = CreateEventConstants.V1_VERSION,
                SourceDomain = CreateEventConstants.SOURCE_DOMAIN,
                SourceSystem = CreateEventConstants.SOURCE_SYSTEM,
                EventData = new EventData
                {
                    NewData = tenure
                },
                User = new User { Name = token.Name, Email = token.Email }
            };
        }

        public TenureSns Update(TenureInformation tenure, Token token)
        {
            return new TenureSns
            {
                CorrelationId = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                EntityId = tenure.Id,
                Id = Guid.NewGuid(),
                EventType = UpdateEventConstants.EVENTTYPE,
                Version = UpdateEventConstants.V1_VERSION,
                SourceDomain = UpdateEventConstants.SOURCE_DOMAIN,
                SourceSystem = UpdateEventConstants.SOURCE_SYSTEM,
                EventData = new EventData
                {
                    NewData = tenure
                },
                User = new User { Name = token.Name, Email = token.Email }
            };
        }

    }
}

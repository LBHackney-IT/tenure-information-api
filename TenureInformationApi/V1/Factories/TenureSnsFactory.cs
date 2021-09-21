using Hackney.Core.JWT;
using System;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Domain.Sns;
using TenureInformationApi.V1.Infrastructure;

namespace TenureInformationApi.V1.Factories
{
    public class TenureSnsFactory : ISnsFactory
    {
        public TenureSns CreateTenure(TenureInformation tenure, Token token)
        {
            return new TenureSns
            {
                CorrelationId = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                EntityId = tenure.Id,
                Id = Guid.NewGuid(),
                EventType = CreateTenureEventConstants.EVENTTYPE,
                Version = CreateTenureEventConstants.V1_VERSION,
                SourceDomain = CreateTenureEventConstants.SOURCE_DOMAIN,
                SourceSystem = CreateTenureEventConstants.SOURCE_SYSTEM,
                EventData = new EventData
                {
                    NewData = tenure
                },
                User = new User { Name = token.Name, Email = token.Email }
            };
        }

        public TenureSns UpdateTenure(UpdateEntityResult<TenureInformationDb> updateResult, Token token)
        {
            return new TenureSns
            {
                CorrelationId = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                EntityId = updateResult.UpdatedEntity.Id,
                Id = Guid.NewGuid(),
                EventType = UpdateTenureConstants.EVENTTYPE,
                Version = UpdateTenureConstants.V1_VERSION,
                SourceDomain = UpdateTenureConstants.SOURCE_DOMAIN,
                SourceSystem = UpdateTenureConstants.SOURCE_SYSTEM,
                EventData = new EventData
                {
                    NewData = updateResult.NewValues,
                    OldData = updateResult.OldValues
                },
                User = new User { Name = token.Name, Email = token.Email }
            };
        }

        public TenureSns PersonAddedToTenure(UpdateEntityResult<TenureInformationDb> updateResult, Token token)
        {
            return new TenureSns
            {
                CorrelationId = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                EntityId = updateResult.UpdatedEntity.Id,
                Id = Guid.NewGuid(),
                EventType = PersonAddedToTenureConstants.EVENTTYPE,
                Version = PersonAddedToTenureConstants.V1_VERSION,
                SourceDomain = PersonAddedToTenureConstants.SOURCE_DOMAIN,
                SourceSystem = PersonAddedToTenureConstants.SOURCE_SYSTEM,
                EventData = new EventData
                {
                    NewData = updateResult.NewValues,
                    OldData = updateResult.OldValues
                },
                User = new User { Name = token.Name, Email = token.Email }
            };
        }
    }
}

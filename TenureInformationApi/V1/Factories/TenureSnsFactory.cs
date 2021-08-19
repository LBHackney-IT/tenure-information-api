using Hackney.Core.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Domain;
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
                EventType = Constants.EVENTTYPE,
                Version = Constants.V1_VERSION,
                SourceDomain = Constants.SOURCE_DOMAIN,
                SourceSystem = Constants.SOURCE_SYSTEM,
                User = new User
                {
                    Id = Guid.NewGuid(),
                    Name = token.Name,
                    Email = token.Email
                }
            };
        }
    }
}

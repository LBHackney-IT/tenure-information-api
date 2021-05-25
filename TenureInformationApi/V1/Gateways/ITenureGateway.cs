using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TenureInformationApi.V1.Domain;

namespace TenureInformationApi.V1.Gateways
{
    public interface ITenureGateway
    {
        TenureInformation GetEntityById(Guid id);

    }
}

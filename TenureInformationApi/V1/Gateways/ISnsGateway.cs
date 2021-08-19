using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Domain;

namespace TenureInformationApi.Tests.V1.Gateways
{
    public interface ISnsGateway
    {
        Task Publish(TenureSns tenureSns);

    }
}

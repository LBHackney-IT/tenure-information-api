using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.Domain;

namespace TenureInformationApi.V1.Factories
{
    public interface IResponseFactory
    {
        TenureResponseObject ToResponse(TenureInformation domain);
    }
}

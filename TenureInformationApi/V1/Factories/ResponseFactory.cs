using System.Collections.Generic;
using System.Linq;
using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Infrastructure;

namespace TenureInformationApi.V1.Factories
{
    public static class ResponseFactory
    {
        //TODO: Map the fields in the domain object(s) to fields in the response object(s).
        // More information on this can be found here https://github.com/LBHackney-IT/lbh-base-api/wiki/Factory-object-mappings
        public static TenureResponseObject ToResponse(this TenureInformation domain)
        {
            return new TenureResponseObject();
        }

        public static List<TenureResponseObject> ToResponse(this IEnumerable<TenureInformation> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }
    }
}

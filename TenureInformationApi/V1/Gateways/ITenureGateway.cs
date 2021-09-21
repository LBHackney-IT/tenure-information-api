using Hackney.Core.JWT;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Infrastructure;

namespace TenureInformationApi.V1.Gateways
{
    public interface ITenureGateway
    {
        Task<TenureInformation> GetEntityById(TenureQueryRequest query);

        Task<TenureInformation> PostNewTenureAsync(CreateTenureRequestObject createTenureRequestObject);

        Task<UpdateEntityResult<TenureInformationDb>> UpdateTenureForPerson(UpdateTenureRequest query, UpdateTenureForPersonRequestObject updateTenureRequestObject);

        Task<UpdateEntityResult<TenureInformationDb>> EditTenureDetails(TenureQueryRequest query, EditTenureDetailsRequestObject editTenureDetailsRequestObject, string requestBody, int? ifMatch);
    }
}

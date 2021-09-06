using Hackney.Core.Logging;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Gateways;
using TenureInformationApi.V1.UseCase.Interfaces;

namespace TenureInformationApi.V1.UseCase
{
    public class UpdateTenureForPersonUseCase : IUpdateTenureForPersonUseCase
    {
        private readonly ITenureGateway _tenureGateway;

        public UpdateTenureForPersonUseCase(ITenureGateway gateway)
        {
            _tenureGateway = gateway;
        }

        [LogCall]
        public async Task<TenureResponseObject> ExecuteAsync(UpdateTenureRequest query, UpdateTenureForPersonRequestObject updateTenureRequestObject)
        {
            var tenure = await _tenureGateway.UpdateTenureForPerson(query, updateTenureRequestObject).ConfigureAwait(false);
            return tenure.ToResponse();
        }
    }
}

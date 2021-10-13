using Hackney.Core.Logging;
using Hackney.Shared.Tenure.Boundary.Requests;
using Hackney.Shared.Tenure.Domain;
using System.Threading.Tasks;
using TenureInformationApi.V1.Gateways;
using TenureInformationApi.V1.UseCase.Interfaces;

namespace TenureInformationApi.V1.UseCase
{
    public class GetByIdUseCase : IGetByIdUseCase
    {
        private readonly ITenureGateway _gateway;



        public GetByIdUseCase(ITenureGateway gateway)
        {
            _gateway = gateway;
        }

        [LogCall]
        public async Task<TenureInformation> Execute(TenureQueryRequest query)
        {
            var tenure = await _gateway.GetEntityById(query).ConfigureAwait(false);

            return tenure;
        }
    }
}

using Hackney.Core.Logging;
using Hackney.Shared.Tenure.Boundary.Requests;
using Hackney.Shared.Tenure.Domain;
using System.Threading.Tasks;
using TenureInformationApi.V1.Gateways.Interfaces;
using TenureInformationApi.V1.UseCase.Interfaces;

namespace TenureInformationApi.V1.UseCase
{
    public class GetByIdUseCase : IGetByIdUseCase
    {
        private readonly ITenureDynamoDbGateway _gateway;

        public GetByIdUseCase(ITenureDynamoDbGateway gateway)
        {
            _gateway = gateway;
        }

        [LogCall]
        public async Task<TenureInformation> Execute(TenureQueryRequest query)
        {
            var tenure = await _gateway.GetEntityById(query).ConfigureAwait(false);

            var irrelevantValue = Guid.NewGuid();

            var yo = new Yo();
            var valid = yo.InvalidFunctionality(irrelevantValue);

            if (valid)
            {
                yo.Wassup();
            }
            else
            {
                yo.Wassup("no");
            }

            return tenure;
        }
    }
}

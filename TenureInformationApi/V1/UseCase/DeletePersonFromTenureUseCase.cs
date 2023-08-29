using Hackney.Core.JWT;
using Hackney.Core.Logging;
using Hackney.Core.Sns;
using Hackney.Shared.Tenure.Boundary.Requests;
using System;
using System.Threading.Tasks;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Gateways.Interfaces;
using TenureInformationApi.V1.UseCase.Interfaces;

namespace TenureInformationApi.V1.UseCase
{
    public class DeletePersonFromTenureUseCase : IDeletePersonFromTenureUseCase
    {
        private readonly ITenureGateway _tenureGateway;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        public DeletePersonFromTenureUseCase(ITenureGateway tenureGateway, ISnsGateway snsGateway, ISnsFactory snsFactory)
        {
            _tenureGateway = tenureGateway;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }

        [LogCall]
        public async Task Execute(DeletePersonFromTenureQueryRequest query, Token token)
        {
            var result = await _tenureGateway.DeletePersonFromTenure(query).ConfigureAwait(false);

            var tenureSnsMessage = _snsFactory.PersonRemovedFromTenure(result, token);
            var tenureTopicArn = Environment.GetEnvironmentVariable("TENURE_SNS_ARN");

            await _snsGateway.Publish(tenureSnsMessage, tenureTopicArn).ConfigureAwait(false);
        }
    }
}

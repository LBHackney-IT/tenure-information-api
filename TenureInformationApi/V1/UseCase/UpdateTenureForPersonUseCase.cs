using Hackney.Core.JWT;
using Hackney.Core.Logging;
using Hackney.Core.Sns;
using System;
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
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;
        public UpdateTenureForPersonUseCase(ITenureGateway gateway, ISnsGateway snsGateway, ISnsFactory snsFactory)
        {
            _tenureGateway = gateway;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }

        [LogCall]
        public async Task<TenureResponseObject> ExecuteAsync(UpdateTenureRequest query, UpdateTenureForPersonRequestObject updateTenureRequestObject, Token token)
        {
            var updateResult = await _tenureGateway.UpdateTenureForPerson(query, updateTenureRequestObject).ConfigureAwait(false);
            if (updateResult == null) return null;

            var tenureSnsMessage = _snsFactory.Update(updateResult, token);
            var tenureTopicArn = Environment.GetEnvironmentVariable("TENURE_SNS_ARN");

            await _snsGateway.Publish(tenureSnsMessage, tenureTopicArn).ConfigureAwait(false);
            return updateResult.UpdatedEntity.ToDomain().ToResponse();
        }
    }
}

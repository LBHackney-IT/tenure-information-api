using Hackney.Core.JWT;
using Hackney.Core.Sns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Gateways;
using TenureInformationApi.V1.UseCase.Interfaces;

namespace TenureInformationApi.V1.UseCase
{
    public class UpdateTenureUseCase : IUpdateTenureUseCase
    {
        private readonly ITenureGateway _tenureGateway;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;
        public UpdateTenureUseCase(ITenureGateway gateway, ISnsGateway snsGateway, ISnsFactory snsFactory)
        {
            _tenureGateway = gateway;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }
        public async Task<TenureResponseObject> ExecuteAsync(TenureQueryRequest query, UpdateTenureRequestObject updateTenureRequestObject, Token token)
        {
            var tenure = await _tenureGateway.UpdateTenure(query, updateTenureRequestObject).ConfigureAwait(false);
            if (tenure == null) return null;
            var tenureSnsMessage = _snsFactory.Update(tenure, token);
            var tenureTopicArn = Environment.GetEnvironmentVariable("TENURE_SNS_ARN");

            await _snsGateway.Publish(tenureSnsMessage, tenureTopicArn).ConfigureAwait(false);
            return tenure.ToResponse();
        }

    }
}

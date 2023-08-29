using Hackney.Core.JWT;
using Hackney.Core.Sns;
using Hackney.Shared.Tenure.Boundary.Requests;
using Hackney.Shared.Tenure.Boundary.Response;
using Hackney.Shared.Tenure.Factories;
using System;
using System.Threading.Tasks;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Factories.Interfaces;
using TenureInformationApi.V1.Gateways.Interfaces;
using TenureInformationApi.V1.UseCase.Interfaces;

namespace TenureInformationApi.V1.UseCase
{
    public class PostNewTenureUseCase : IPostNewTenureUseCase
    {
        private readonly ITenureGateway _tenureGateway;
        private readonly ISnsGateway _snsGateway;
        private readonly ITenureSnsFactory _snsFactory;

        public PostNewTenureUseCase(ITenureGateway tenureGateway, ISnsGateway snsGateway, ITenureSnsFactory snsFactory)
        {
            _tenureGateway = tenureGateway;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }

        public async Task<TenureResponseObject> ExecuteAsync(CreateTenureRequestObject createTenureRequestObject, Token token)
        {
            var tenure = await _tenureGateway.PostNewTenureAsync(createTenureRequestObject).ConfigureAwait(false);

            var tenureSnsMessage = _snsFactory.CreateTenure(tenure, token);
            var tenureTopicArn = Environment.GetEnvironmentVariable("TENURE_SNS_ARN");

            await _snsGateway.Publish(tenureSnsMessage, tenureTopicArn).ConfigureAwait(false);

            return tenure.ToResponse();
        }
    }
}

using Hackney.Core.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.Tests.V1.Gateways;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Gateways;
using TenureInformationApi.V1.UseCase.Interfaces;

namespace TenureInformationApi.V1.UseCase
{
    public class PostNewTenureUseCase : IPostNewTenureUseCase
    {
        private readonly ITenureGateway _tenureGateway;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        public PostNewTenureUseCase(ITenureGateway tenureGateway, ISnsGateway snsGateway, ISnsFactory snsFactory)
        {
            _tenureGateway = tenureGateway;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }

        public async Task<TenureResponseObject> ExecuteAsync(CreateTenureRequestObject createTenureRequestObject, Token token)
        {
            var tenure = await _tenureGateway.PostNewTenureAsync(createTenureRequestObject).ConfigureAwait(false);

            var tenureSnsMessage = _snsFactory.Create(tenure, token);
            await _snsGateway.Publish(tenureSnsMessage).ConfigureAwait(false);

            return tenure.ToResponse();
        }
    }
}

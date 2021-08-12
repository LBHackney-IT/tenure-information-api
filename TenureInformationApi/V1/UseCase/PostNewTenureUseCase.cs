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
    public class PostNewTenureUseCase : IPostNewTenureUseCase
    {
        private readonly ITenureGateway _tenureGateway;
        private readonly IResponseFactory _responseFactory;

        public PostNewTenureUseCase(ITenureGateway tenureGateway, IResponseFactory responseFactory)
        {
            _tenureGateway = tenureGateway;
            _responseFactory = responseFactory;
        }

        public async Task<TenureResponseObject> ExecuteAsync(CreateTenureRequestObject createTenureRequestObject)
        {
            var tenure = await _tenureGateway.PostNewTenureAsync(createTenureRequestObject).ConfigureAwait(false);
            return _responseFactory.ToResponse(tenure);
        }
    }
}

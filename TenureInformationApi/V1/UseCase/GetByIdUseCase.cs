using Hackney.Core.Logging;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Gateways;
using TenureInformationApi.V1.UseCase.Interfaces;

namespace TenureInformationApi.V1.UseCase
{
    public class GetByIdUseCase : IGetByIdUseCase
    {
        private readonly ITenureGateway _gateway;
        private readonly IResponseFactory _responseFactory;



        public GetByIdUseCase(ITenureGateway gateway, IResponseFactory responseFactory)
        {
            _gateway = gateway;
            _responseFactory = responseFactory;
        }

        [LogCall]
        public async Task<TenureResponseObject> Execute(GetByIdRequest query)
        {
            var tenure = await _gateway.GetEntityById(query).ConfigureAwait(false);

            return _responseFactory.ToResponse(tenure);
        }
    }
}

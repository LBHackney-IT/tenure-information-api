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

        public UpdateTenureUseCase(ITenureGateway gateway)
        {
            _tenureGateway = gateway;
        }
        public async Task<TenureResponseObject> ExecuteAsync(TenureQueryRequest query, UpdateTenureRequestObject updateTenureRequestObject)
        {
            var tenure = await _tenureGateway.UpdateTenure(query, updateTenureRequestObject).ConfigureAwait(false);
            return tenure.ToResponse();
        }

    }
}

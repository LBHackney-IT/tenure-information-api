using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Gateways;
using TenureInformationApi.V1.UseCase.Interfaces;

namespace TenureInformationApi.V1.UseCase
{
    public class DeletePersonFromTenureUseCase : IDeletePersonFromTenureUseCase
    {
        private readonly ITenureGateway _tenureGateway;

        public DeletePersonFromTenureUseCase(ITenureGateway tenureGateway)
        {
            _tenureGateway = tenureGateway;
        }

        public async Task Execute(RemovePersonFromTenureQueryRequest query)
        {
            await _tenureGateway.DeletePersonFromTenure(query).ConfigureAwait(false);
        }
    }
}

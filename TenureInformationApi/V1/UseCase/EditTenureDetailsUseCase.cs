using Hackney.Core.JWT;
using Hackney.Core.Sns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Response;
using Hackney.Shared.Tenure;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Gateways;
using TenureInformationApi.V1.Infrastructure;
using TenureInformationApi.V1.UseCase.Interfaces;

namespace TenureInformationApi.V1.UseCase
{
    public class EditTenureDetailsUseCase : IEditTenureDetailsUseCase
    {
        private readonly ITenureGateway _tenureGateway;

        public EditTenureDetailsUseCase(ITenureGateway tenureGateway)
        {
            _tenureGateway = tenureGateway;
        }

        public async Task<TenureResponseObject> ExecuteAsync(TenureQueryRequest query, EditTenureDetailsRequestObject editTenureDetailsRequestObject, string requestBody)
        {
            var result = await _tenureGateway.EditTenureDetails(query, editTenureDetailsRequestObject, requestBody).ConfigureAwait(false);
            if (result == null) return null;

            return result.UpdatedEntity.ToDomain().ToResponse();
        }
    }
}

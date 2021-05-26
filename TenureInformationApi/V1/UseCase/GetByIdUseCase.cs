using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Gateways;
using TenureInformationApi.V1.UseCase.Interfaces;

namespace TenureInformationApi.V1.UseCase
{
    public class GetByIdUseCase : IGetByIdUseCase
    {
        private ITenureGateway _gateway;
        public GetByIdUseCase(ITenureGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<TenureResponseObject> Execute(Guid id)
        {
            var tenure = await _gateway.GetEntityById(id).ConfigureAwait(false);

            return tenure.ToResponse();
        }
    }
}

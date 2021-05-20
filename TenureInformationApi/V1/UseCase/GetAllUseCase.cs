using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Gateways;
using TenureInformationApi.V1.UseCase.Interfaces;

namespace TenureInformationApi.V1.UseCase
{
    //TODO: Rename class name and interface name to reflect the entity they are representing eg. GetAllClaimantsUseCase
    public class GetAllUseCase : IGetAllUseCase
    {
        private readonly IExampleGateway _gateway;
        public GetAllUseCase(IExampleGateway gateway)
        {
            _gateway = gateway;
        }

        public ResponseObjectList Execute()
        {
            return new ResponseObjectList { ResponseObjects = _gateway.GetAll().ToResponse() };
        }
    }
}

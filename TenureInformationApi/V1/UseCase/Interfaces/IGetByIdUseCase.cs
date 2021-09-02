using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Response;

namespace TenureInformationApi.V1.UseCase.Interfaces
{
    public interface IGetByIdUseCase
    {
        Task<TenureResponseObject> Execute(TenureQueryRequest query);
    }
}

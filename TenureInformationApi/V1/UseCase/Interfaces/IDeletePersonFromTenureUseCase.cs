using Hackney.Core.JWT;
using Hackney.Shared.Tenure.Boundary.Requests;
using System.Threading.Tasks;

namespace TenureInformationApi.V1.UseCase.Interfaces
{
    public interface IDeletePersonFromTenureUseCase
    {
        Task Execute(DeletePersonFromTenureQueryRequest query, Token token);
    }
}

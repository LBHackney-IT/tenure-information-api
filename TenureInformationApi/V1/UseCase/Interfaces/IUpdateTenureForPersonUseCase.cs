using Hackney.Core.JWT;
using Hackney.Shared.Tenure.Boundary.Requests;
using Hackney.Shared.Tenure.Boundary.Response;
using System.Threading.Tasks;

namespace TenureInformationApi.V1.UseCase.Interfaces
{
    public interface IUpdateTenureForPersonUseCase
    {
        Task<TenureResponseObject> ExecuteAsync(UpdateTenureRequest query, UpdateTenureForPersonRequestObject updateTenureRequestObject,
                    Token token, int? ifMatch);
    }
}

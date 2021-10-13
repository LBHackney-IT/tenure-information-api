using Hackney.Core.JWT;
using Hackney.Shared.Tenure.Boundary.Requests;
using Hackney.Shared.Tenure.Boundary.Response;
using System.Threading.Tasks;

namespace TenureInformationApi.V1.UseCase.Interfaces
{
    public interface IEditTenureDetailsUseCase
    {
        Task<TenureResponseObject> ExecuteAsync(TenureQueryRequest query, EditTenureDetailsRequestObject editTenureDetailsRequestObject, string requestBody, Token token, int? ifMatch);
    }
}

using Hackney.Core.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Response;
using Hackney.Shared.Tenure;
using TenureInformationApi.V1.Infrastructure;

namespace TenureInformationApi.V1.UseCase.Interfaces
{
    public interface IEditTenureDetailsUseCase
    {
        Task<TenureResponseObject> ExecuteAsync(TenureQueryRequest query, EditTenureDetailsRequestObject editTenureDetailsRequestObject, string requestBody);
    }
}

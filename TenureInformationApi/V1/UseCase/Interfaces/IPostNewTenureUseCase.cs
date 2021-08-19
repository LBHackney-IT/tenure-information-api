using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Response;

namespace TenureInformationApi.V1.UseCase.Interfaces
{
    public interface IPostNewTenureUseCase
    {
        Task<TenureResponseObject> ExecuteAsync(CreateTenureRequestObject createTenureRequestObject);
    }
}

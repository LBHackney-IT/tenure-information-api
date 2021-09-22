using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;

namespace TenureInformationApi.V1.UseCase.Interfaces
{
    public interface IDeletePersonFromTenureUseCase
    {
        Task Execute(RemovePersonFromTenureQueryRequest query);
    }
}

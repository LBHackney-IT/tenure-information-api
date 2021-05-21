using System;
using TenureInformationApi.V1.Boundary.Response;

namespace TenureInformationApi.V1.UseCase.Interfaces
{
    public interface IGetByIdUseCase
    {
        TenureResponseObject Execute(Guid id);
    }
}

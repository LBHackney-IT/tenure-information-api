using Hackney.Core.JWT;
using Hackney.Shared.Tenure;
using TenureInformationApi.V1.Domain.Sns;
using TenureInformationApi.V1.Infrastructure;

namespace TenureInformationApi.V1.Factories
{
    public interface ISnsFactory
    {
        TenureSns Create(TenureInformation tenure, Token token);
        TenureSns Update(UpdateEntityResult<TenureInformationDb> updateResult, Token token);
    }
}

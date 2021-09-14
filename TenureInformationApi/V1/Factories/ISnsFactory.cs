using Hackney.Core.JWT;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Domain.Sns;
using TenureInformationApi.V1.Infrastructure;

namespace TenureInformationApi.V1.Factories
{
    public interface ISnsFactory
    {
        TenureSns CreateTenure(TenureInformation tenure, Token token);
        TenureSns PersonAddedToTenure(UpdateEntityResult<TenureInformationDb> updateResult, Token token);
        TenureSns UpdateTenure(UpdateEntityResult<TenureInformationDb> updateResult, Token token);
    }
}

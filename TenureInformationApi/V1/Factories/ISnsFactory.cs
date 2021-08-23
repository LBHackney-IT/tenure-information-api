using Hackney.Core.JWT;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Domain.Sns;

namespace TenureInformationApi.V1.Factories
{
    public interface ISnsFactory
    {
        TenureSns Create(TenureInformation tenure, Token token);
    }
}

using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Factories;
using NUnit.Framework;

namespace TenureInformationApi.Tests.V1.Factories
{
    public class ResponseFactoryTest
    {
        [Test]
        [Ignore("TO DO")]
        public void CanMapADatabaseEntityToADomainObject()
        {
            var domain = new TenureInformation();
            var response = domain.ToResponse();
        }
    }
}

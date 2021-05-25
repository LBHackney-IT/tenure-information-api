using TenureInformationApi.V1.Domain;
using Xunit;

namespace TenureInformationApi.Tests.V1.Domain
{
    public class EntityTests
    {
        [Fact]
        public void EntitiesHaveAnId()
        {
            var entity = new TenureInformation();
            //entity.Id.Should().GetType(Type);
        }

        [Fact]
        public void EntitiesHaveACreatedAt()
        {
            var entity = new TenureInformation();


        }
    }
}

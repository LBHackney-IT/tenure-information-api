using AutoFixture;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Infrastructure;

namespace TenureInformationApi.Tests.V1.Helper
{
    public static class DatabaseEntityHelper
    {
        public static TenureInformationDb CreateDatabaseEntity()
        {
            var entity = new Fixture().Create<TenureInformation>();

            return CreateDatabaseEntityFrom(entity);
        }

        public static TenureInformationDb CreateDatabaseEntityFrom(TenureInformation entity)
        {
            return new TenureInformationDb
            {
                Id = entity.Id
            };
        }
    }
}

using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Infrastructure;

namespace TenureInformationApi.V1.Factories
{
    public static class EntityFactory
    {
        public static TenureInformation ToDomain(this TenureInformationDb databaseEntity)
        {
            //TODO: Map the rest of the fields in the domain object.
            // More information on this can be found here https://github.com/LBHackney-IT/lbh-base-api/wiki/Factory-object-mappings

            return new TenureInformation
            {
                Id = databaseEntity.Id
            };
        }

        public static TenureInformationDb ToDatabase(this TenureInformation entity)
        {
            //TODO: Map the rest of the fields in the database object.

            return new TenureInformationDb
            {
                Id = entity.Id
            };
        }
    }
}

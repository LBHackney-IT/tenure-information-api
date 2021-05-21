using System;
using System.Collections.Generic;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Infrastructure;

namespace TenureInformationApi.V1.Gateways
{
    //TODO: Rename to match the data source that is being accessed in the gateway eg. MosaicGateway
    public class ExampleGateway : IExampleGateway
    {
        private readonly DatabaseContext _databaseContext;

        public ExampleGateway(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public TenureInformation GetEntityById(Guid id)
        {
            var result = _databaseContext.DatabaseEntities.Find(id);

            return result?.ToDomain();
        }

        public List<TenureInformation> GetAll()
        {
            return new List<TenureInformation>();
        }
    }
}

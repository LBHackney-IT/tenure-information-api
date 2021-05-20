using Amazon.DynamoDBv2.DataModel;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Infrastructure;
using System.Collections.Generic;

namespace TenureInformationApi.V1.Gateways
{
    public class DynamoDbGateway : IExampleGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;

        public DynamoDbGateway(IDynamoDBContext dynamoDbContext)
        {
            _dynamoDbContext = dynamoDbContext;
        }

        public List<Entity> GetAll()
        {
            return new List<Entity>();
        }

        public Entity GetEntityById(int id)
        {
            var result = _dynamoDbContext.LoadAsync<DatabaseEntity>(id).GetAwaiter().GetResult();
            return result?.ToDomain();
        }
    }
}

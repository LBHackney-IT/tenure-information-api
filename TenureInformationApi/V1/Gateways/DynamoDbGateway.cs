using Amazon.DynamoDBv2.DataModel;
using Hackney.Core.Logging;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Infrastructure;

namespace TenureInformationApi.V1.Gateways
{
    public class DynamoDbGateway : ITenureGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly ILogger<DynamoDbGateway> _logger;


        public DynamoDbGateway(IDynamoDBContext dynamoDbContext, ILogger<DynamoDbGateway> logger)
        {
            _logger = logger;
            _dynamoDbContext = dynamoDbContext;
        }

        [LogCall]
        public async Task<TenureInformation> GetEntityById(GetByIdRequest query)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.LoadAsync for id {query.Id}");
            var result = await _dynamoDbContext.LoadAsync<TenureInformationDb>(query.Id).ConfigureAwait(false);
            return result?.ToDomain();
        }
    }
}

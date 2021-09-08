using Amazon.DynamoDBv2.DataModel;
using Force.DeepCloner;
using Hackney.Core.Logging;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<TenureInformation> GetEntityById(TenureQueryRequest query)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.LoadAsync for id {query.Id}");
            var result = await _dynamoDbContext.LoadAsync<TenureInformationDb>(query.Id).ConfigureAwait(false);
            return result?.ToDomain();
        }

        [LogCall]

        public async Task<TenureInformation> PostNewTenureAsync(CreateTenureRequestObject createTenureRequestObject)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.SaveAsync");
            var tenureDbEntity = createTenureRequestObject.ToDatabase();

            await _dynamoDbContext.SaveAsync(tenureDbEntity).ConfigureAwait(false);

            return tenureDbEntity.ToDomain();

        }

        [LogCall]
        public async Task<UpdateEntityResult<TenureInformationDb>> UpdateTenureForPerson(UpdateTenureRequest query, UpdateTenureForPersonRequestObject updateTenureRequestObject)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.LoadAsync for id {query.Id} and then IDynamoDBContext.SaveAsync");
            var tenure = await _dynamoDbContext.LoadAsync<TenureInformationDb>(query.Id).ConfigureAwait(false);
            if (tenure == null) return null;

            var result = new UpdateEntityResult<TenureInformationDb>()
            {
                UpdatedEntity = tenure,
                OldValues = new Dictionary<string, object>
                {
                    { "householdMembers", tenure.HouseholdMembers.DeepClone() }
                }
            };

            var householdMember = tenure.HouseholdMembers.FirstOrDefault(x => x.Id == query.PersonId);
            if (householdMember is null)
            {
                householdMember = new HouseholdMembers();
                tenure.HouseholdMembers.Add(householdMember);
            }

            if (updateTenureRequestObject.DateOfBirth.HasValue)
                householdMember.DateOfBirth = updateTenureRequestObject.DateOfBirth.Value;
            householdMember.FullName = updateTenureRequestObject.FullName;
            householdMember.Id = query.PersonId;
            if (updateTenureRequestObject.IsResponsible.HasValue)
                householdMember.IsResponsible = updateTenureRequestObject.IsResponsible.Value;
            if (updateTenureRequestObject.Type.HasValue)
                householdMember.Type = updateTenureRequestObject.Type.Value;

            householdMember.PersonTenureType = TenureTypes.GetPersonTenureType(tenure.TenureType, householdMember.IsResponsible);

            await _dynamoDbContext.SaveAsync(tenure).ConfigureAwait(false);

            result.NewValues = new Dictionary<string, object>
            {
                { "householdMembers", tenure.HouseholdMembers }
            };

            return result;
        }
    }
}

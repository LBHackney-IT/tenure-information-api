using Amazon.DynamoDBv2.DataModel;
using FluentValidation.Results;
using Force.DeepCloner;
using Hackney.Core.Logging;
using Hackney.Shared.Tenure.Boundary.Requests;
using Hackney.Shared.Tenure.Boundary.Requests.Validation;
using Hackney.Shared.Tenure.Domain;
using Hackney.Shared.Tenure.Factories;
using Hackney.Shared.Tenure.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Gateways.Interfaces;
using TenureInformationApi.V1.Infrastructure;
using TenureInformationApi.V1.Infrastructure.Exceptions;
using TenureInformationApi.V1.Infrastructure.Interfaces;

namespace TenureInformationApi.V1.Gateways
{
    public class TenureDynamoDbGateway : ITenureGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly IEntityUpdater _updater;
        private readonly ILogger<TenureDynamoDbGateway> _logger;

        public TenureDynamoDbGateway(IDynamoDBContext dynamoDbContext, IEntityUpdater updater, ILogger<TenureDynamoDbGateway> logger)
        {
            _logger = logger;
            _updater = updater;
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
            _logger.LogInformation($"isTemporaryAccommodation Request={createTenureRequestObject.TenuredAsset?.IsTemporaryAccommodation}");
            var tenureDbEntity = createTenureRequestObject.ToDatabase();

            await _dynamoDbContext.SaveAsync(tenureDbEntity).ConfigureAwait(false);
            _logger.LogInformation($"isTemporaryAccommodation DB={tenureDbEntity.TenuredAsset?.IsTemporaryAccommodation}");
            return tenureDbEntity.ToDomain();
        }

        [LogCall]
        public async Task<UpdateEntityResult<TenureInformationDb>> UpdateTenureForPerson(UpdateTenureRequest query, UpdateTenureForPersonRequestObject updateTenureRequestObject,
                                                                                         int? ifMatch)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.LoadAsync for id {query.Id} and then IDynamoDBContext.SaveAsync");
            var tenure = await _dynamoDbContext.LoadAsync<TenureInformationDb>(query.Id).ConfigureAwait(false);
            if (tenure == null) return null;
            if (ifMatch != tenure.VersionNumber)
                throw new VersionNumberConflictException(ifMatch, tenure.VersionNumber);



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
            {
                householdMember.DateOfBirth = updateTenureRequestObject.DateOfBirth.Value;
            }

            householdMember.FullName = updateTenureRequestObject.FullName;
            householdMember.Id = query.PersonId;

            if (updateTenureRequestObject.IsResponsible.HasValue)
            {
                householdMember.IsResponsible = updateTenureRequestObject.IsResponsible.Value;
            }

            if (updateTenureRequestObject.Type.HasValue)
            {
                householdMember.Type = updateTenureRequestObject.Type.Value;
            }

            householdMember.PersonTenureType = TenureTypes.GetPersonTenureType(tenure.TenureType, householdMember.IsResponsible);

            await _dynamoDbContext.SaveAsync(tenure).ConfigureAwait(false);

            result.NewValues = new Dictionary<string, object>
            {
                { "householdMembers", tenure.HouseholdMembers }
            };

            return result;
        }



        [LogCall]
        public async Task<UpdateEntityResult<TenureInformationDb>> EditTenureDetails(TenureQueryRequest query, EditTenureDetailsRequestObject editTenureDetailsRequestObject, string requestBody, int? ifMatch)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.LoadAsync for id {query.Id}");

            var existingTenure = await _dynamoDbContext.LoadAsync<TenureInformationDb>(query.Id).ConfigureAwait(false);
            if (existingTenure == null) return null;

            if (ifMatch != existingTenure.VersionNumber)
                throw new VersionNumberConflictException(ifMatch, existingTenure.VersionNumber);

            var response = _updater.UpdateEntity(existingTenure, requestBody, editTenureDetailsRequestObject);

            // if only tenureStartDate is passed, check if tenureStartDate exists in database and that it's later than the start date
            if (response.NewValues.ContainsKey("startOfTenureDate") && !response.NewValues.ContainsKey("endOfTenureDate"))
            {
                var results = ValidateTenureStartDateIsLessThanCurrentTenureEndDate((DateTime?) response.NewValues["startOfTenureDate"], existingTenure.EndOfTenureDate);
                if (!results.IsValid) throw new EditTenureInformationValidationException(results);
            }

            // if only tenureEndDate is passed, check that it's later than tenureStartDate
            if (response.NewValues.ContainsKey("endOfTenureDate") && !response.NewValues.ContainsKey("startOfTenureDate"))
            {
                var results = ValidateTenureEndDateIsGreaterThanTenureStartDate((DateTime?) response.NewValues["endOfTenureDate"], existingTenure.StartOfTenureDate);
                if (!results.IsValid) throw new EditTenureInformationValidationException(results);
            }

            if (response.NewValues.Any())
            {
                _logger.LogDebug($"Calling IDynamoDBContext.SaveAsync to update id {query.Id}");
                await _dynamoDbContext.SaveAsync<TenureInformationDb>(response.UpdatedEntity).ConfigureAwait(false);
            }

            return response;
        }

        private static ValidationResult ValidateTenureEndDateIsGreaterThanTenureStartDate(DateTime? tenureEndDate, DateTime? tenureStartDate)
        {
            var testObject = new TenureInformation
            {
                EndOfTenureDate = tenureEndDate,
                StartOfTenureDate = tenureStartDate
            };

            var validator = new TenureInformationValidatorWhenOnlyEndDate();

            return validator.Validate(testObject);
        }

        private static ValidationResult ValidateTenureStartDateIsLessThanCurrentTenureEndDate(DateTime? tenureStartDate, DateTime? tenureEndDate)
        {
            var testObject = new TenureInformation
            {
                StartOfTenureDate = tenureStartDate,
                EndOfTenureDate = tenureEndDate
            };

            var validator = new TenureInformationValidatorWhenOnlyStartDate();

            return validator.Validate(testObject);
        }

        [LogCall]
        public async Task<UpdateEntityResult<TenureInformationDb>> DeletePersonFromTenure(DeletePersonFromTenureQueryRequest query)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.LoadAsync for id {query.TenureId}");

            var existingTenure = await _dynamoDbContext.LoadAsync<TenureInformationDb>(query.TenureId).ConfigureAwait(false);
            if (existingTenure == null) throw new TenureNotFoundException();

            // remove person from tenure
            var initialNumberOfTenures = existingTenure.HouseholdMembers.Count;
            var filteredHouseholdMembers = existingTenure.HouseholdMembers.Where(x => x.Id != query.PersonId).ToList();

            // if person was removed, the count should be less
            if (filteredHouseholdMembers.Count == initialNumberOfTenures) throw new PersonNotFoundInTenureException();

            // add changes to UpdateEntityResult
            var result = new UpdateEntityResult<TenureInformationDb>()
            {
                UpdatedEntity = existingTenure,
                OldValues = new Dictionary<string, object>
                {
                    { "householdMembers", existingTenure.HouseholdMembers.DeepClone() }
                },
                NewValues = new Dictionary<string, object>
                {
                    { "householdMembers", filteredHouseholdMembers }
                }
            };

            // save changes to database
            _logger.LogDebug($"Calling IDynamoDBContext.SaveAsync to update id {query.TenureId}");

            existingTenure.HouseholdMembers = filteredHouseholdMembers;
            await _dynamoDbContext.SaveAsync(existingTenure).ConfigureAwait(false);

            return result;
        }
    }
}

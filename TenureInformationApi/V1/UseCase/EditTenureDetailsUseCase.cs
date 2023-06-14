using Hackney.Core.JWT;
using Hackney.Core.Sns;
using Hackney.Shared.Tenure.Boundary.Requests;
using Hackney.Shared.Tenure.Boundary.Response;
using Hackney.Shared.Tenure.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Gateways;
using TenureInformationApi.V1.Infrastructure.Exceptions;
using TenureInformationApi.V1.UseCase.Interfaces;

namespace TenureInformationApi.V1.UseCase
{
    public class EditTenureDetailsUseCase : IEditTenureDetailsUseCase
    {
        private readonly ITenureGateway _tenureGateway;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        private readonly ISet<string> _editChargesGroups;

        public const string EditChargesAllowedGroupsVariable = "EDIT_CHARGES_ALLOWED_GROUPS";

        public EditTenureDetailsUseCase(ITenureGateway tenureGateway, ISnsGateway snsGateway, ISnsFactory snsFactory)
        {
            _tenureGateway = tenureGateway;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;

            var editChargesGroupString = Environment.GetEnvironmentVariable(EditChargesAllowedGroupsVariable);
            if (string.IsNullOrWhiteSpace(editChargesGroupString))
            {
                throw new Exception($"Environment variable not found: {EditChargesAllowedGroupsVariable}");
            }
            _editChargesGroups = editChargesGroupString.Split(',', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
        }

        private bool CanEditCharges(string[] groups) => groups?.Any(g => _editChargesGroups.Contains(g)) ?? false;

        private JsonSerializerOptions CreateJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }

        private bool HasCharges(string updateJson)
        {
            var returnValue = false;
            if (!string.IsNullOrWhiteSpace(updateJson))
            {
                var updateDic = JsonSerializer.Deserialize<Dictionary<string, object>>(updateJson, CreateJsonOptions());
                returnValue = updateDic.ContainsKey(nameof(EditTenureDetailsRequestObject.Charges));
            }
            return returnValue;
        }

        public async Task<TenureResponseObject> ExecuteAsync(
            TenureQueryRequest query, EditTenureDetailsRequestObject editTenureDetailsRequestObject, string requestBody, Token token, int? ifMatch)
        {
            // Perform the least-expensive check first            
            if (!CanEditCharges(token.Groups) && HasCharges(requestBody))
            {
                throw new EditTenureInformationUnauthorisedChangeException(nameof(editTenureDetailsRequestObject.Charges));
            }

            var result = await _tenureGateway.EditTenureDetails(query, editTenureDetailsRequestObject, requestBody, ifMatch).ConfigureAwait(false);
            if (result == null) return null;

            if (result.NewValues.Any())
            {
                var tenureSnsMessage = _snsFactory.UpdateTenure(result, token);
                var tenureTopicArn = Environment.GetEnvironmentVariable("TENURE_SNS_ARN");

                await _snsGateway.Publish(tenureSnsMessage, tenureTopicArn).ConfigureAwait(false);
            }

            return result.UpdatedEntity.ToDomain().ToResponse();
        }
    }
}

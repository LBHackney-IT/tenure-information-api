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
using TenureInformationApi.V1.Infrastructure.Exceptions;
using TenureInformationApi.V1.UseCase.Interfaces;
using TenureInformationApi.V1.Gateways.Interfaces;
using TenureInformationApi.V1.Factories.Interfaces;

namespace TenureInformationApi.V1.UseCase
{
    public class EditTenureDetailsUseCase : IEditTenureDetailsUseCase
    {
        private readonly ITenureDynamoDbGateway _tenureGateway;
        private readonly ISnsGateway _snsGateway;
        private readonly ITenureSnsFactory _snsFactory;

        public EditTenureDetailsUseCase(ITenureDynamoDbGateway tenureGateway, ISnsGateway snsGateway, ITenureSnsFactory snsFactory)
        {
            _tenureGateway = tenureGateway;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }

        public async Task<TenureResponseObject> ExecuteAsync(
            TenureQueryRequest query, EditTenureDetailsRequestObject editTenureDetailsRequestObject, string requestBody, Token token, int? ifMatch)
        {
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

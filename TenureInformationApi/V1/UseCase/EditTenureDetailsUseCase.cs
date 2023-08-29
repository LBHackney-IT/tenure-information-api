using Hackney.Core.JWT;
using Hackney.Core.Sns;
using Hackney.Shared.Tenure.Boundary.Requests;
using Hackney.Shared.Tenure.Boundary.Response;
using Hackney.Shared.Tenure.Factories;
using System;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.UseCase.Interfaces;
using TenureInformationApi.V1.Gateways.Interfaces;
using TenureInformationApi.V1.Factories.Interfaces;
using Microsoft.Extensions.Logging;

namespace TenureInformationApi.V1.UseCase
{
    public class EditTenureDetailsUseCase : IEditTenureDetailsUseCase
    {
        private readonly ITenureDynamoDbGateway _tenureGateway;
        private readonly ISnsGateway _snsGateway;
        private readonly ITenureSnsFactory _snsFactory;
        private readonly ILogger<EditTenureDetailsUseCase> _logger;

        public EditTenureDetailsUseCase(ITenureDynamoDbGateway tenureGateway, ISnsGateway snsGateway, ITenureSnsFactory snsFactory, ILogger<EditTenureDetailsUseCase> logger)
        {
            _tenureGateway = tenureGateway;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
            _logger = logger;
        }

        public async Task<TenureResponseObject> ExecuteAsync(
            TenureQueryRequest query, EditTenureDetailsRequestObject editTenureDetailsRequestObject, string requestBody, Token token, int? ifMatch)
        {
            _logger.LogInformation("Calling EditTenureDetailsUseCase for {TenureId}", query.Id);

            var result = await _tenureGateway.EditTenureDetails(query, editTenureDetailsRequestObject, requestBody, ifMatch).ConfigureAwait(false);
            if (result == null)
            {
                _logger.LogInformation("No updates returned from EditTenureDetails with {TenureId}", query.Id);
                return null;
            }

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

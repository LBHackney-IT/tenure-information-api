using FluentValidation;
using TenureInformationApi.V1.Domain;

namespace TenureInformationApi.V1.Boundary.Requests.Validation
{
    public class TenureInformationValidatorWhenOnlyStartDate : AbstractValidator<TenureInformation>
    {
        public TenureInformationValidatorWhenOnlyStartDate()
        {
            // tenureStartDate has been requested to be updated in the EditTenureDetails request.
            // The new start date must be validated against the end date if it exists in the database

            When(tenure => tenure.EndOfTenureDate != null, () =>
            {
                RuleFor(x => x.StartOfTenureDate)
                    .LessThan(x => x.EndOfTenureDate)
                    .WithErrorCode(ErrorCodes.TenureEndDate);
            });
        }
    }
}

using FluentValidation;
using Hackney.Core.Validation;

namespace TenureInformationApi.V1.Boundary.Requests.Validation
{
    public class UpdateTenureForPersonRequestObjectValidation : AbstractValidator<UpdateTenureForPersonRequestObject>
    {
        public UpdateTenureForPersonRequestObjectValidation()
        {
            RuleFor(x => x.FullName).NotXssString()
                         .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}

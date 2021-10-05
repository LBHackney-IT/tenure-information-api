using FluentValidation;
using System;

namespace TenureInformationApi.V1.Boundary.Requests.Validation
{
    public class UpdateTenureRequestObjectValidator : AbstractValidator<UpdateTenureRequestObject>
    {
        public UpdateTenureRequestObjectValidator()
        {
            RuleFor(x => x.Id).NotNull()
                             .NotEqual(Guid.Empty);
            RuleForEach(x => x.HouseholdMembers).SetValidator(new HouseholdMemberValidator());
        }
    }
}

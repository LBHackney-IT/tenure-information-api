using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenureInformationApi.V1.Boundary.Requests.Validation
{
    public class UpdateTenureRequestValidation : AbstractValidator<UpdateTenureRequestObject>
    {
        public UpdateTenureRequestValidation()
        {
            RuleFor(x => x.Id).NotNull()
                             .NotEqual(Guid.Empty);
            RuleForEach(x => x.HouseholdMembers).SetValidator(new HouseholdMemberValidator());
        }
    }
}

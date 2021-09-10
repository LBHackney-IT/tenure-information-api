using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenureInformationApi.V1.Boundary.Requests.Validation
{
    public class EditTenureDetailsRequestValidation : AbstractValidator<EditTenureDetailsRequestObject>
    {
        public EditTenureDetailsRequestValidation()
        {
            RuleFor(x => x.EndOfTenureDate)
               .GreaterThan(x => x.StartOfTenureDate)
               .WithErrorCode(ErrorCodes.TenureEndDate);

            RuleFor(x => x.TenureType).SetValidator(new TenureTypeValidator());
        }
    }
}

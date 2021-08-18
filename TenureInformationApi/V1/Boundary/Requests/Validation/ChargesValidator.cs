using FluentValidation;
using Hackney.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Domain;

namespace TenureInformationApi.V1.Boundary.Requests.Validation
{
    public class ChargesValidator : AbstractValidator<Charges>
    {
        public ChargesValidator()
        {
            RuleFor(x => x.BillingFrequency).NotXssString()
                         .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}

using FluentValidation;
using Hackney.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Domain;

namespace TenureInformationApi.V1.Boundary.Requests.Validation
{
    public class TerminatedValidator : AbstractValidator<Terminated>
    {
        public TerminatedValidator()
        {
            RuleFor(x => x.ReasonForTermination).NotXssString()
                         .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
using FluentValidation;
using System;
using TenureInformationApi.V1.Boundary.Requests;

namespace TenureInformationApi.V1.Boundary.Request.Validation
{
    public class UpdateTenureRequestValidator : AbstractValidator<UpdateTenureRequest>
    {
        public UpdateTenureRequestValidator()
        {
            RuleFor(x => x.Id).NotNull()
                              .NotEqual(Guid.Empty);
            RuleFor(x => x.PersonId).NotNull()
                              .NotEqual(Guid.Empty);
        }
    }
}

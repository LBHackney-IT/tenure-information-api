using FluentValidation;
using System;
using TenureInformationApi.V1.Boundary.Requests;

namespace TenureInformationApi.V1.Boundary.Request.Validation
{
    public class GetByIdRequestValidator : AbstractValidator<TenureQueryRequest>
    {
        public GetByIdRequestValidator()
        {
            RuleFor(x => x.Id).NotNull()
                              .NotEqual(Guid.Empty);
        }
    }
}

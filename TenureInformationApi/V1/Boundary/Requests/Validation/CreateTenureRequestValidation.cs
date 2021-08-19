using FluentValidation;
using Hackney.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenureInformationApi.V1.Boundary.Requests.Validation
{
    public class CreateTenureRequestValidation : AbstractValidator<CreateTenureRequestObject>
    {
        public CreateTenureRequestValidation()
        {
            RuleFor(x => x.EndOfTenureDate)
                .GreaterThan(x => x.StartOfTenureDate)
                .WithErrorCode(ErrorCodes.TenureEndDate);

            RuleFor(x => x.PaymentReference).NotXssString()
                                     .WithErrorCode(ErrorCodes.XssCheckFailure);

            RuleForEach(x => x.HouseholdMembers).SetValidator(new HouseholdMemberValidator());

            RuleForEach(x => x.Notices).SetValidator(new NoticesValidator());
            RuleFor(x => x.AgreementType).SetValidator(new AgreementTypeValidator());

            RuleFor(x => x.Charges).SetValidator(new ChargesValidator());
            RuleForEach(x => x.LegacyReferences).SetValidator(new LegacyReferenceValidator());
            RuleFor(x => x.TenuredAsset).SetValidator(new TenuredAssetValidator());
            RuleFor(x => x.TenureType).SetValidator(new TenureTypeValidator());
            RuleFor(x => x.Terminated).SetValidator(new TerminatedValidator());









        }
    }
}

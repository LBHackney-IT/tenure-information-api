using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Requests.Validation;
using Xunit;

namespace TenureInformationApi.Tests.V1.Boundary.Validation
{
    public class CreateTenureRequestValidatorTests
    {
        public CreateTenureRequestValidation _classUnderTest;

        public CreateTenureRequestValidatorTests()
        {
            _classUnderTest = new CreateTenureRequestValidation();
        }
        private const string StringWithTags = "Some string with <tag> in it.";

        [Fact]
        public void WhenEndDateisCorrectNoError()
        {
            var request = new CreateTenureRequestObject()
            {
                EndOfTenureDate = DateTime.UtcNow.AddDays(1),
                StartOfTenureDate = DateTime.UtcNow
            };
            var result = _classUnderTest.TestValidate(request);
            result.ShouldNotHaveValidationErrorFor(x => x.EndOfTenureDate);
        }

        [Fact]
        public void WhenEndDateIsInCorrectHasError()
        {
            var request = new CreateTenureRequestObject()
            {
                EndOfTenureDate = DateTime.UtcNow,
                StartOfTenureDate = DateTime.UtcNow.AddDays(1)
            };
            var result = _classUnderTest.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.EndOfTenureDate)
                .WithErrorCode(ErrorCodes.TenureEndDate);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void PaymentReferenceShouldNotErrorWithNoValue(string value)
        {
            var model = new CreateTenureRequestObject() { PaymentReference = value };
            var result =_classUnderTest.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.PaymentReference);

        }

        [Fact]
        public void PaymentReferenceShouldErrorWithhTagsInValue()
        {
            var model = new CreateTenureRequestObject() { PaymentReference = StringWithTags };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.PaymentReference)
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}

using FluentValidation.TestHelper;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Requests.Validation;
using Xunit;

namespace TenureInformationApi.Tests.V1.Boundary.Validation
{
    public class UpdateTenureRequestObjectValidatorTests
    {
        private const string StringWithTags = "Some string with <tag> in it.";

        public UpdateTenureRequestObjectValidation _classUnderTest;
        public UpdateTenureRequestObjectValidatorTests()
        {
            _classUnderTest = new UpdateTenureRequestObjectValidation();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RequesthouldNotErrorWithNoValue(string value)
        {
            var model = new UpdateTenureForPersonRequestObject() { FullName = value };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.FullName);

        }

        [Fact]
        public void RequestShouldErrorWithTagsInValue()
        {
            var model = new UpdateTenureForPersonRequestObject() { FullName = StringWithTags };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FullName)
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}

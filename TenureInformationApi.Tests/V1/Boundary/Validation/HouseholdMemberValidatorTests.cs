using FluentValidation.TestHelper;
using TenureInformationApi.V1.Boundary.Requests.Validation;
using TenureInformationApi.V1.Domain;
using Xunit;

namespace TenureInformationApi.Tests.V1.Boundary.Validation
{
    public class HouseholdMemberValidatorTests
    {
        public HouseholdMemberValidator _classUnderTest;

        public HouseholdMemberValidatorTests()
        {
            _classUnderTest = new HouseholdMemberValidator();
        }
        private const string StringWithTags = "Some string with <tag> in it.";



        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FullNameShouldNotErrorWithNoValue(string value)
        {
            var model = new HouseholdMembers() { FullName = value };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.FullName);

        }

        [Fact]
        public void FullnameShouldErrorWithhTagsInValue()
        {
            var model = new HouseholdMembers() { FullName = StringWithTags };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FullName)
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }


    }
}

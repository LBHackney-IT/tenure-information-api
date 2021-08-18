using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests.Validation;
using TenureInformationApi.V1.Domain;
using Xunit;

namespace TenureInformationApi.Tests.V1.Boundary.Validation
{
    public class NoticesValidatorTests
    {
        public NoticesValidator _classUnderTest;

        public NoticesValidatorTests()
        {
            _classUnderTest = new NoticesValidator();
        }
        private const string StringWithTags = "Some string with <tag> in it.";



        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void TypeShouldNotErrorWithNoValue(string value)
        {
            var model = new Notices() { Type = value };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Type);

        }

        [Fact]
        public void TypeShouldErrorWithhTagsInValue()
        {
            var model = new Notices() { Type = StringWithTags };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Type)
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }


    }
}

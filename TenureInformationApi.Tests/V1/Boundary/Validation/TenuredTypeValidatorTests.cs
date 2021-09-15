using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests.Validation;
using Hackney.Shared.Tenure;
using Xunit;

namespace TenureInformationApi.Tests.V1.Boundary.Validation
{
    public class TenuredTypeValidatorTests
    {
        public TenureTypeValidator _classUnderTest;

        public TenuredTypeValidatorTests()
        {
            _classUnderTest = new TenureTypeValidator();
        }
        private const string StringWithTags = "Some string with <tag> in it.";



        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void CodeShouldNotErrorWithNoValue(string value)
        {
            var model = new TenureType() { Code = value };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Code);

        }

        [Fact]
        public void CodeShouldErrorWithhTagsInValue()
        {
            var model = new TenureType() { Code = StringWithTags };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Code)
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void DescriptionShouldNotErrorWithNoValue(string value)
        {
            var model = new TenureType() { Description = value };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);

        }

        [Fact]
        public void DescriptionShouldErrorWithhTagsInValue()
        {
            var model = new TenureType() { Description = StringWithTags };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}

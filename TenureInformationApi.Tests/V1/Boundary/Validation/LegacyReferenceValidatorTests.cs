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
    public class LegacyReferenceValidatorTests
    {
        public LegacyReferenceValidator _classUnderTest;

        public LegacyReferenceValidatorTests()
        {
            _classUnderTest = new LegacyReferenceValidator();
        }
        private const string StringWithTags = "Some string with <tag> in it.";



        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void NameShouldNotErrorWithNoValue(string value)
        {
            var model = new LegacyReference() { Name = value };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);

        }

        [Fact]
        public void NameShouldErrorWithhTagsInValue()
        {
            var model = new LegacyReference() { Name = StringWithTags };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ValueShouldNotErrorWithNoValue(string value)
        {
            var model = new LegacyReference() { Value = value };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Value);

        }

        [Fact]
        public void ValueShouldErrorWithhTagsInValue()
        {
            var model = new LegacyReference() { Value = StringWithTags };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Value)
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}

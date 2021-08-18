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
    public class TenuredAssetValidatorTests
    {
        public TenuredAssetValidator _classUnderTest;

        public TenuredAssetValidatorTests()
        {
            _classUnderTest = new TenuredAssetValidator();
        }
        private const string StringWithTags = "Some string with <tag> in it.";



        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FullAddressShouldNotErrorWithNoValue(string value)
        {
            var model = new TenuredAsset() { FullAddress = value };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.FullAddress);

        }

        [Fact]
        public void FullAddressShouldErrorWithhTagsInValue()
        {
            var model = new TenuredAsset() { FullAddress = StringWithTags };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FullAddress)
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void UprnShouldNotErrorWithNoValue(string value)
        {
            var model = new TenuredAsset() { Uprn = value };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Uprn);

        }

        [Fact]
        public void UprnShouldErrorWithhTagsInValue()
        {
            var model = new TenuredAsset() { Uprn = StringWithTags };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Uprn)
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}

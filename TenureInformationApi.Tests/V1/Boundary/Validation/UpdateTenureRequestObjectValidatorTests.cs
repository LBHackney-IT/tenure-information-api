using FluentValidation.TestHelper;
using System;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Requests.Validation;
using Xunit;

namespace TenureInformationApi.Tests.V1.Boundary.Request.Validation
{
    public class UpdateTenureRequestObjectValidatorTests
    {
        private readonly UpdateTenureRequestObjectValidator _sut;

        public UpdateTenureRequestObjectValidatorTests()
        {
            _sut = new UpdateTenureRequestObjectValidator();
        }

        [Fact]
        public void RequestShouldErrorWithNullId()
        {
            var query = new UpdateTenureRequestObject();
            var result = _sut.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void RequestShouldErrorWithEmptyId()
        {
            var query = new UpdateTenureRequestObject() { Id = Guid.Empty };
            var result = _sut.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }
    }
}

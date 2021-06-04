using FluentValidation.TestHelper;
using System;
using TenureInformationApi.V1.Boundary.Request.Validation;
using TenureInformationApi.V1.Boundary.Requests;
using Xunit;

namespace TenureInformationApi.Tests.V1.Boundary.Request.Validation
{
    public class GetByIdRequestValidatorTests
    {
        private readonly GetByIdRequestValidator _sut;

        public GetByIdRequestValidatorTests()
        {
            _sut = new GetByIdRequestValidator();
        }

        [Fact]
        public void RequestShouldErrorWithNullTargetId()
        {
            var query = new GetByIdRequest();
            var result = _sut.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void RequestShouldErrorWithEmptyTargetId()
        {
            var query = new GetByIdRequest() { Id = Guid.Empty };
            var result = _sut.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }
    }
}

using FluentValidation.TestHelper;
using System;
using TenureInformationApi.V1.Boundary.Request.Validation;
using TenureInformationApi.V1.Boundary.Requests;
using Xunit;

namespace TenureInformationApi.Tests.V1.Boundary.Request.Validation
{
    public class UpdateTenureRequestValidatorTests
    {
        private readonly UpdateTenureRequestValidator _sut;

        public UpdateTenureRequestValidatorTests()
        {
            _sut = new UpdateTenureRequestValidator();
        }

        [Fact]
        public void RequestShouldErrorWithNullId()
        {
            var query = new UpdateTenureRequest();
            var result = _sut.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void RequestShouldErrorWithEmptyId()
        {
            var query = new UpdateTenureRequest() { Id = Guid.Empty };
            var result = _sut.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void RequestShouldErrorWithNullPersonId()
        {
            var query = new UpdateTenureRequest();
            var result = _sut.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.PersonId);
        }

        [Fact]
        public void RequestShouldErrorWithEmptyPersonId()
        {
            var query = new UpdateTenureRequest() { PersonId = Guid.Empty };
            var result = _sut.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.PersonId);
        }
    }
}

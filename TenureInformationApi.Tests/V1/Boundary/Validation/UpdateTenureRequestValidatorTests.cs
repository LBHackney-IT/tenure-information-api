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
    public class UpdateTenureRequestValidatorTests
    {
        public UpdateTenureRequestValidation _classUnderTest;
        public UpdateTenureRequestValidatorTests()
        {
            _classUnderTest = new UpdateTenureRequestValidation();
        }

        [Fact]
        public void RequestShouldErrorWithNullTargetId()
        {
            var query = new UpdateTenureRequestObject();
            var result = _classUnderTest.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void RequestShouldErrorWithEmptyTargetId()
        {
            var query = new UpdateTenureRequestObject() { Id = Guid.Empty };
            var result = _classUnderTest.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }
    }
}

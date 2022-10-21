using FluentAssertions;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace TenureInformationApi.Tests.V1.Helper
{
    public static class ErrorHelper
    {
        public static void ShouldHaveErrorFor(JEnumerable<JToken> errors, string propertyName, string errorCode = null)
        {
            var error = errors.FirstOrDefault(x => string.Equals(x.Path.Split('.').Last().Trim('\'', ']'), propertyName, StringComparison.OrdinalIgnoreCase)) as JProperty;
            error.Should().NotBeNull();
            if (!string.IsNullOrEmpty(errorCode))
                error.Value.ToString().Should().Contain(errorCode);
        }
    }
}

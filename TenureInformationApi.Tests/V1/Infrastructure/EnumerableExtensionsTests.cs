using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using TenureInformationApi.V1.Infrastructure;
using Xunit;

namespace TenureInformationApi.Tests.V1.Infrastructure
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void ToListOrEmptyTestNullReturnsEmpty()
        {
            var sut = (IEnumerable<string>) null;
            sut.ToListOrEmpty().Should().BeEmpty();
        }

        [Fact]
        public void ToListOrEmptyTestEmptyReturnsEmpty()
        {
            var sut = Enumerable.Empty<string>();
            sut.ToListOrEmpty().Should().BeEmpty();
        }

        [Fact]
        public void ToListOrEmptyTestPupluatedReturnsList()
        {
            var sut = new[] { "one", "two", "three" };
            sut.ToListOrEmpty().Should().BeEquivalentTo(sut);
        }
    }
}

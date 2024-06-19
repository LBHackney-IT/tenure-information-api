using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using TenureInformationApi.V1;
using Xunit;

namespace TenureInformationApi.Tests.V1
{
    public class YoTests
    {
        [Fact]
        public void CustomYo()
        {
            var yo = new Yo();

            var resp = yo.Wassup("borgatron");

            resp.Should().Be("Wassup, borgatron?");
        }

        [Fact]
        public void DefaultYo()
        {
            var yo = new Yo();

            var resp = yo.Wassup();

            resp.Should().Be("Wassup, dawg?");
        }
    }
}

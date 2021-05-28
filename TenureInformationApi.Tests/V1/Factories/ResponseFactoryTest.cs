using AutoFixture;
using FluentAssertions;
using System.Collections.Generic;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Factories;
using Xunit;

namespace TenureInformationApi.Tests.V1.Factories
{
    public class ResponseFactoryTest
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void CanMapANullTenureInfoToAResponseObject()
        {
            TenureInformation domain = null;
            var response = domain.ToResponse();

            response.Should().BeNull();
        }

        [Fact]
        public void CanMapATenureInfoToAResponseObject()
        {
            var domain = _fixture.Create<TenureInformation>();
            var response = domain.ToResponse();
            domain.Should().BeEquivalentTo(response);

        }

        [Fact]
        public void CanMapDomainTenureInfoListToAResponsesList()
        {
            var list = _fixture.CreateMany<TenureInformation>(10);
            var responseNotes = list.ToResponse();

            responseNotes.Should().BeEquivalentTo(list);
        }

        [Fact]
        public void CanMapNullDomainTenureInfoListToAnEmptyResponsesList()
        {
            List<TenureInformation> list = null;
            var responseNotes = list.ToResponse();

            responseNotes.Should().BeEmpty();
        }
    }
}

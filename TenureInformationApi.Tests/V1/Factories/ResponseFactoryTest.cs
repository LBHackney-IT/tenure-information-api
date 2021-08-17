using AutoFixture;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Factories;
using Xunit;

namespace TenureInformationApi.Tests.V1.Factories
{
    public class ResponseFactoryTest
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly ResponseFactory _sut;

        public ResponseFactoryTest()
        {
            _sut = new ResponseFactory();
        }

        [Fact]
        public void CanMapANullTenureInfoToAResponseObject()
        {
            TenureInformation domain = null;
            var response = _sut.ToResponse(domain);

            response.Should().BeNull();
        }

        [Fact]
        public void CanMapATenureInfoToAResponseObject()
        {
            var domain = _fixture.Create<TenureInformation>();
            var response = _sut.ToResponse(domain);
            domain.Should().BeEquivalentTo(response);

        }

        [Fact]
        public void CanMapDomainTenureInfoListToAResponsesList()
        {
            var list = _fixture.CreateMany<TenureInformation>(10);
            var responseNotes = _sut.ToResponse(list);

            responseNotes.Should().BeEquivalentTo(list);
        }

        [Fact]
        public void CanMapNullDomainTenureInfoListToAnEmptyResponsesList()
        {
            List<TenureInformation> list = null;
            var responseNotes = _sut.ToResponse(list);

            responseNotes.Should().BeEmpty();
        }
    }
}

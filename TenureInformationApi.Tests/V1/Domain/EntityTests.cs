using System;
using TenureInformationApi.V1.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace TenureInformationApi.Tests.V1.Domain
{
    [TestFixture]
    public class EntityTests
    {
        [Test]
        public void EntitiesHaveAnId()
        {
            var entity = new TenureInformation();
            //entity.Id.Should().GetType(Type);
        }

        [Test]
        public void EntitiesHaveACreatedAt()
        {
            var entity = new TenureInformation();


        }
    }
}

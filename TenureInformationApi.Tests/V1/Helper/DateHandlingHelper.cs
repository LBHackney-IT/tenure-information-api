using System;
using AutoFixture.Kernel;

namespace TenureInformationApi.Tests.V1.Helper
{
    public class UtcDateTimeHelper : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var type = request as Type;
            if (type == typeof(DateTime))
            {
                return DateTime.UtcNow;
            }
            return new NoSpecimen();
        }
    }
}

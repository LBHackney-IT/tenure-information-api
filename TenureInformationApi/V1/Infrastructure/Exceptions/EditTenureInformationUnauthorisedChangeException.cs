using System;

namespace TenureInformationApi.V1.Infrastructure.Exceptions
{
    public class EditTenureInformationUnauthorisedChangeException : Exception
    {
        public string Member { get; private set; }

        public EditTenureInformationUnauthorisedChangeException(string member)
        {
            Member = member;
        }
    }
}

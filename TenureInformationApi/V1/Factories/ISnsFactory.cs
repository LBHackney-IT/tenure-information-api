using Hackney.Core.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Domain;

namespace TenureInformationApi.V1.Factories
{
    public interface ISnsFactory
    {
        TenureSns Create(TenureInformation tenure, Token token);
    }
}

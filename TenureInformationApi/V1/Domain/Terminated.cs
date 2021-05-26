using System;

namespace TenureInformationApi.V1.Domain
{
    public class Terminated
    {
        public bool IsTerminated { get; set; }

        public string ReasonForTermination { get; set; }
    }
}

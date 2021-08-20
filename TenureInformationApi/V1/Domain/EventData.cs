using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenureInformationApi.V1.Domain
{
    public class EventData
    {
        public DataItem OldData { get; set; }
        public DataItem NewData { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenureInformationApi.V1.Domain
{
    public class DataItem
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public int ContactType { get; set; }
        public string Description { get; set; }
    }
}

using System;
using System.Linq;

namespace namespace TenureInformationApi.V1.Models
{
    public class FF
    {
        public int OPs = 500;
        public int[] Numbers; 

        public FF()
        {
            Numbers = new int[] { OPs };
        }

        public int GetFirstNumber() => Numbers.ToList().FirstOrDefault();
    }
}

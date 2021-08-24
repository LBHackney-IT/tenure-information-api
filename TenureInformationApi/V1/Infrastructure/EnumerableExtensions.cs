using System.Collections.Generic;
using System.Linq;

namespace TenureInformationApi.V1.Infrastructure
{
    public static class EnumerableExtensions
    {
        public static List<T> ToListOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null ? new List<T>() : enumerable.ToList();
        }
    }
}

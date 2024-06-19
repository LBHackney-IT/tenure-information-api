using System;

namespace TenureInformationApi.V1
{
    public class Yo
    {
        public string Target { get; set; }

        public Yo()
        {
            Target = "dawg";
        }

        public string Wassup(string customTarget = null)
        {
            return $"Wassup, {customTarget ?? this.Target}?";
        }

        // Intends to trigger analysis issue.
        public bool InvalidFunctionality(Guid guid)
        {
            if (guid is null)
                return true;
            
            return false;
        }
    }
}

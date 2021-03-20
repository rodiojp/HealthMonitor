using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitor.Domain
{
    public static class SystemConstants
    {
        // Error list
        public const string MISSING_HEALTH_CHECK_PARAMETER = "Missing {0} parameter for {1}";
        public const string MISSING_HEALTH_CHECK_SECTION = "HealthChecksSection is not defined";
    }
}

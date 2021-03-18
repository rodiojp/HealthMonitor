using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain.Results;

namespace HealthMonitor.WindowsServices.Common
{
    public class SeriousEventArgs : EventArgs
    {
        public HealthMonitorResult HealthCheckResult { get; set; }
        public string HealthCheckName { get; set; }
    }
}

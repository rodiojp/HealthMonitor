using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain.Results;

namespace HealthMonitor.WindowsServices.Common
{
    public class HealthCheckErrorArgs : EventArgs
    {
        public Exception HealthCheckException { get; set; }
        public string HealthCheckName { get; set; }
    }
}

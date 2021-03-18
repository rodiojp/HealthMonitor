using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitor.WindowsServices.Common
{
    public class ScheduleProviderException : Exception
    {
        public ScheduleProviderException(string providerName)
        {
            ProviderName = providerName;
        }

        public string ProviderName { get; private set; }
    }
}

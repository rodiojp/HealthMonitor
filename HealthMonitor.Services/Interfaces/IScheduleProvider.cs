using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitor.Services.Interfaces
{
    public interface IScheduleProvider
    {
        DateTime NextRunTime { get; }

        Double GetInterval { get; }

        void Reset();
    }
}

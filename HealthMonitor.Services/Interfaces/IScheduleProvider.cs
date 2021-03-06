using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain.Scheduling;

namespace HealthMonitor.Services.Interfaces
{
    public interface IScheduleProvider
    {
        FrequencyInterval IntervalType { get; }

        DateTime NextRunTime { get; }

        Double GetInterval { get; }

        void Reset();
    }
}

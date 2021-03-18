using System.Collections.Generic;

namespace HealthMonitor.Domain.Configuration.Interfaces
{
    public interface IScheduleParameters
    {
        string Frequency { get; }
        string StartTime { get; }
        IEnumerable<string> TimeParameters { get; }
    }
}
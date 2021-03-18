using System.Collections.Generic;


namespace HealthMonitor.Domain.Configuration.Interfaces
{
    public interface IHealthCheck
    {
        string Name { get; }
        string AlertType { get; }
        string To { get; }
        string From { get; }
        IScheduleParameters ScheduleParams { get; }
        IEnumerable<IHealthCheckParameter> HealthCheckParams { get; }
    }
}

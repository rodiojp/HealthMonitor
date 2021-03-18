namespace HealthMonitor.Domain.Configuration.Interfaces
{
    public interface IHealthCheckParameter
    {
        string Name { get; }
        string Type { get; }
        string Value { get; }
    }
}
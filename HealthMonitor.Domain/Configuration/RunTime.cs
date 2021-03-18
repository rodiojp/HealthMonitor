using System.Configuration;

namespace HealthMonitor.Domain.Configuration
{
    public class RunTime : ConfigurationElement
    {
        [ConfigurationProperty("Value", IsRequired = true)]
        public string Value => this["Value"] as string;
    }
}
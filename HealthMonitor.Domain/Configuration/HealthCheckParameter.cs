using System.Configuration;
using HealthMonitor.Domain.Configuration.Interfaces;

namespace HealthMonitor.Domain.Configuration
{
    public class HealthCheckParameter : ConfigurationElement, IHealthCheckParameter
    {
        [ConfigurationProperty("Name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get => this["Name"] as string;
            set => base["Name"] = value;
        }

        [ConfigurationProperty("Type", IsRequired = true)]
        public string Type
        {
            get => this["Type"] as string;
            set => base["Type"] = value;
        }


        [ConfigurationProperty("Value", IsRequired = true)]
        public string Value
        {
            get => this["Value"] as string;
            set => base["Value"] = value;
        }

        public HealthCheckParameter()
        { }

        public HealthCheckParameter(string name, string type, string value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

    }
}
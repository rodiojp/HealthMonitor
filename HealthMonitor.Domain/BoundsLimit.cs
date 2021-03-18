using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain.Configuration;
using HealthMonitor.Domain.Configuration.Interfaces;

namespace HealthMonitor.Domain
{
    public class BoundsLimit: IHealthCheckParameter
    {
        public string Name { get; private set; }
        public string Type { get; private set; }
        public string Value { get; private set; }

        public BoundsLimit(string name, string type, string value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public BoundsLimit(HealthCheckParameter healthParam)
        {
            Name = healthParam.Name;
            Type = healthParam.Type;
            Value = healthParam.Value;
        }
    }
}

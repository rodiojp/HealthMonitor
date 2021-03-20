using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using HealthMonitor.Domain.Configuration.Interfaces;

namespace HealthMonitor.Domain.Configuration
{
    public class HealthCheck : ConfigurationElement, IHealthCheck
    {
        [ConfigurationProperty("Name", IsRequired = true, IsKey = true)]
        public string Name => this["Name"] as string;

        [ConfigurationProperty("Type", IsRequired = true)]
        public string Type => this["Type"] as string;

        [ConfigurationProperty("AlertType", IsRequired = true)]
        public string AlertType => this["AlertType"] as string;

        [ConfigurationProperty("To")]
        public string To => this["To"] as string;

        [ConfigurationProperty("From")]
        public string From => this["From"] as string;

        [ConfigurationProperty("Schedule")]
        public Schedule Schedule => (Schedule)this["Schedule"];

        [ConfigurationProperty("HealthCheckParameters")]
        [ConfigurationCollection(typeof(HealthCheckParameters))]
        public HealthCheckParameters HealthCheckParameters
        {
            get
            {
                object o = this["HealthCheckParameters"];
                return o as HealthCheckParameters;
            }
        }
        public IEnumerable<IHealthCheckParameter> HealthCheckParams => HealthCheckParameters.ReturnAll().Select(x => new HealthCheckParameter(x.Name, x.Type, x.Value));
        IScheduleParameters IHealthCheck.ScheduleParams => Schedule;
    }
}

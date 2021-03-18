using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using HealthMonitor.Domain.Configuration.Interfaces;

namespace HealthMonitor.Domain.Configuration
{
    public class HealthChecksSection : ConfigurationSection
    {
        public static HealthChecksSection GetConfig()
        {
            return (HealthChecksSection)ConfigurationManager.GetSection("HealthChecksSection") ?? new HealthChecksSection();
        }
        [ConfigurationProperty("HealthChecks")]
        [ConfigurationCollection(typeof(HealthChecksSection), AddItemName ="HealthChecks")]
        public HealthChecks HealthChecks
        {
            get
            {
                object o = this["HealthChecks"];
                return o as HealthChecks;
            }
        }
    }
}
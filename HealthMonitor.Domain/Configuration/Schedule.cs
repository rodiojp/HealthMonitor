using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain.Configuration.Interfaces;

namespace HealthMonitor.Domain.Configuration
{

    /// <summary>
    ///    <Schedule Frequency="Interval" StartTime="Apr 6, 2021 9:40 AM">
    ///       <RunTimes>
    ///          <RunTime Value="00:15:05"></RunTime>
    ///       </RunTimes>
    ///    </Schedule>
    /// </summary>
    public class Schedule : ConfigurationElement, IScheduleParameters
    {
        [ConfigurationProperty("Frequency", IsRequired = true)]
        public string Frequency => this["Frequency"] as string;

        [ConfigurationProperty("StartTime", IsRequired = true)]
        public string StartTime => this["StartTime"] as string;

        [ConfigurationProperty("RunTimes")]
        public RunTimes RunTimes
        {
            get
            {
                object o = this["RunTimes"];
                return o as RunTimes;
            }
        }

        public IEnumerable<string> TimeParameters => RunTimes.ReturnAll().Select(x => x.Value);
    }
}

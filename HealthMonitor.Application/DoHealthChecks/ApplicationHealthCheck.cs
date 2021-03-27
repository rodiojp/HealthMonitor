using System;
using System.Collections.Generic;
using System.Linq;
using HealthMonitor.Domain;
using HealthMonitor.Domain.Results;

namespace HealthMonitor.Application.DoHealthChecks
{
    public abstract class ApplicationHealthCheck
    {
        public string Name { get; protected set; }
        public HealthType HealthType { get; internal set; }
        public virtual HealthMonitorResult DoHealthCheck(IEnumerable<BoundsLimit> healthCheckParameters)
        {
            return null;
        }

        protected void EnsureAllParametersArePresent(IEnumerable<string> paramNames, IEnumerable<BoundsLimit> healthCheckParameters)
        {
            foreach (var item in paramNames.Where(x => !healthCheckParameters.Any(y => y.Name.Equals(x))))
            {
                throw new ArgumentException(string.Format(SystemConstants.MISSING_HEALTH_CHECK_PARAMETER, Name, item));
            }
        }

    }
}
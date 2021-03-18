using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitor.Domain.Results
{
    /// <summary>
    /// The Health Result Category/Type to Return
    /// </summary>
    /// <remarks>
    /// Enum: ResultType
    /// </remarks>
    public enum HealthType
    {
        Database,
        Paramters,
        Treshhold,
        Information,
        WindowsService,
        Files
    }
}

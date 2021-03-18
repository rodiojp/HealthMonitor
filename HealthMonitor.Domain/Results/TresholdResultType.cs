using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitor.Domain.Results
{
    /// <summary>
    /// The Treshold Result Type to Return
    /// </summary>
    /// <remarks>
    /// Enum: TresholdResultType
    /// </remarks>
    public enum TresholdResultType
    {
        WithinRange,
        OverMaximum,
        UnderMinimum
    }
}

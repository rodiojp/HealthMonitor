using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitor.Services.Interfaces
{
    public interface IClock
    {
        DateTime Now { get; }

        DateTime CurrentDay(int contractHr);

        DateTime CurrentDay(int contractHr, string timeZone);

        DateTime PreviousDay(int contractHr);

        DateTime PreviousDay(int contractHr, int offset);
    }
}

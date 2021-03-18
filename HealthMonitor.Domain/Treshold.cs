using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain.Results;

namespace HealthMonitor.Domain
{
    /// <summary>
    /// Class to hold a range of values
    /// </summary>
    public class Treshold<T> where T : struct, IComparable
    {
        public T Min { get; }

        public T Max { get; }
        /// <summary>
        /// Initializes a new instance of the Treshold class
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public Treshold(T min, T max)
        {
            if (min.CompareTo(max) > 0)
                throw new ArgumentOutOfRangeException($"{min} must be less than {max}");
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Indicates if the passes value is grater than maximum
        /// or less than the minimum
        /// </summary>
        /// <param name="other">value to see if it is within range of max or min</param>
        /// <returns>True if passed value is NOT within min max range</returns>
        public bool ExceedsBounds(T other)
        {
            return other.CompareTo(Max) > 0 || other.CompareTo(Min) < 0;
        }

        public TresholdResultType CheckWithinBounds(T other)
        {
            if (other.CompareTo(Max) > 0)
                return TresholdResultType.OverMaximum;
            return other.CompareTo(Min) < 0
                ? TresholdResultType.UnderMinimum
                : TresholdResultType.WithinRange;

        }
    }
}

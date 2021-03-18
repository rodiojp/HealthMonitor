using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Services.Interfaces;

namespace HealthMonitor.Services.Common
{
    public class CurrentClock : IClock
    {
        /// <summary>
        /// Returns the current system time
        /// </summary>
        public DateTime Now => DateTime.Now;

        /// <summary>
        /// if day begins at the contract hour not at midnight
        /// </summary>
        /// <param name="contractHr">The Contract Hour</param>
        /// <returns>The Current day</returns>
        public DateTime CurrentDay(int contractHr)
        {
            const string eastern = "Eastern Standard Time";
            return CurrentDay(contractHr, eastern);
        }

        /// <summary>
        /// if day begins at the contract hour not at midnight
        /// </summary>
        /// <param name="contractHr">The Contract Hour</param>
        /// <param name="timeZone">time zone identifier</param>
        /// <returns>The Current day</returns>
        public DateTime CurrentDay(int contractHr, string timeZone)
        {
            var timeZoneInfo = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, timeZone);
            return timeZoneInfo.Date.Hour < contractHr
                ? DateTime.Today.AddDays(-1).AddHours(contractHr)
                : DateTime.Today.AddHours(contractHr);
        }
        /// <summary>
        /// To find the Date and Time that records as the end of a day
        /// </summary>
        /// <param name="contractHr">
        /// The hour in a day where says all readings afterwords are on a different contract
        /// </param>
        /// <returns><see cref="DateTime"/> in Universal Time Clock (UTC)</returns>
        public DateTime PreviousDay(int contractHr)
        {
            const int offsetHr = 5;
            return PreviousDay(contractHr, offsetHr);
        }
        /// <summary>
        /// To find the Date and Time that records as the end of a day
        /// </summary>
        /// <param name="contractHr">
        /// <param name="offset"></param>
        /// The hour in a day where says all readings afterwords are on a different contract
        /// </param>
        /// <returns><see cref="DateTime"/> in Universal Time Clock (UTC)</returns>
        public DateTime PreviousDay(int contractHr, int offset)
        {
            var numberOfDays = DateTime.Now.Hour < contractHr ? -2 : -1;
            return DateTime.Now.Date.AddDays(numberOfDays).AddHours(contractHr + offset);
        }
    }
}

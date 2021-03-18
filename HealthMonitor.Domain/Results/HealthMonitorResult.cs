using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitor.Domain.Results
{
    /// <summary>
    /// Abstract Class for All HealthMonitorResults
    /// </summary>
    /// <remarks>
    /// Class: HealthMonitorResult.cs
    /// Author: 
    /// </remarks>
    public class HealthMonitorResult
    {
        private readonly ResultStatus[] seriousStatus = new[] { ResultStatus.Error, ResultStatus.Warning };

        /// <summary>
        /// Result after processing
        /// </summary>
        public ResultStatus Status { get; set; }
        /// <summary>
        /// The Kind/Category of the Health Result
        /// </summary>
        public HealthType HealthType { get; set; }

        private StringBuilder _messageBuilder;

        public StringBuilder MessageBuilder => _messageBuilder;

        /// <summary>
        /// A Concise Message regarding the Result
        /// </summary>
        public String Message => MessageBuilder.ToString();

        /// <summary>
        /// The Time that the result was calculated
        /// </summary>
        public DateTime ResultTime { get; private set; }

        /// <summary>
        /// Returns True if the current <c>Status</c> is in a state that is serious
        /// </summary>
        public bool IsSerious => seriousStatus.Contains(Status);

        /// <summary>
        /// A Name of the Result
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Constructor for class HealthMonitor Result
        /// </summary>
        public HealthMonitorResult()
        {
            _messageBuilder = new StringBuilder();
        }

        public HealthMonitorResult(string name, HealthType healthType) : this()
        {
            Name = name;
            HealthType = healthType;
        }

        public HealthMonitorResult(string name, HealthType healthType, ResultStatus status, string message = "") : this(name, healthType)
        {
            Status = status;
            if (!string.IsNullOrEmpty(message)) MessageBuilder.Append(message);

        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($"Name: {Name}").AppendLine();
            sb.AppendFormat($"Status: {Status}").AppendLine();
            sb.AppendFormat($"HealthType: {HealthType}").AppendLine();
            sb.AppendFormat($"Result Time: {ResultTime.ToString("yyyy-MM-dd HH:mm")}").AppendLine();
            sb.AppendFormat($"UTC Time: {ResultTime.ToString("U")}").AppendLine();
            sb.AppendFormat($"Message: {Message}").AppendLine();

            return MessageBuilder.ToString();
        }
    }
}

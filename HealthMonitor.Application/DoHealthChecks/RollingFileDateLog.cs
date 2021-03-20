using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain;
using HealthMonitor.Domain.Extensions;
using HealthMonitor.Domain.Results;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;

namespace HealthMonitor.Application.DoHealthChecks
{
    public class RollingFileDateLog : ApplicationHealthCheck
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private HealthMonitorResult Result { get; set; }

        protected const string ServiceTimeoutMillisecondsParameter = "ServiceTimeoutMilliseconds";
        protected TimeSpan ServiceTimeout { get; set; } = TimeSpan.FromMilliseconds(5000);
        public RollingFileDateLog()
        {
            Name = "Delete RollingFile Date Log";
            HealthType = HealthType.WindowsService;
            Result = new HealthMonitorResult(Name, HealthType, ResultStatus.Information);
        }

        public override HealthMonitorResult DoHealthCheck(IEnumerable<BoundsLimit> healthCheckParameters)
        {
            try
            {
                BoundsLimit boundsLimit = healthCheckParameters.FirstOrDefault(x => x.Name.ToLower().Equals(ServiceTimeoutMillisecondsParameter.ToLower()));
                if (boundsLimit != null)
                {
                    int serviceTimeoutMilliseconds = int.Parse(boundsLimit.Value);
                    ServiceTimeout = TimeSpan.FromMilliseconds(serviceTimeoutMilliseconds);
                }
                BoundsLimit[] checkParameters = healthCheckParameters as BoundsLimit[] ?? healthCheckParameters.ToArray();
                string rollingFileAppenderName = checkParameters.FirstOrDefault(x => x.Type.Equals("appender-ref")).Value;
                var rollingFileName = checkParameters.FirstOrDefault(x => x.Type.Equals("file")).Value;


                //Read the configuration
                Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
                CleanupLogs(hierarchy, rollingFileAppenderName, rollingFileName);
            }
            catch (Exception e)
            {
                Result.Status = ResultStatus.Error;
                Result.MessageBuilder.Append(e.ToLogString()).Append(Environment.NewLine);
            }
            return Result;
        }
        /// <summary>
        /// Log4Net does not support rolling files maximum by date. This method does the cleanup.
        /// </summary>
        /// <param name="hierarchy"></param>
        private void CleanupLogs(Hierarchy hierarchy, string rollingFileAppenderName, string rollingFileName)
        {
            if (hierarchy == null)
            {
                throw new ArgumentNullException("Hierarchy is null");
            }

            Result.MessageBuilder.Append($"Start cleanup Appender: \"{rollingFileAppenderName}\", appender's file Name: \"{rollingFileName}\"")
                                 .Append(Environment.NewLine);

            var st = hierarchy.Root.GetAppender(rollingFileAppenderName);// ("RollingFile");
            var appender = hierarchy.Root.GetAppender(rollingFileAppenderName) as RollingFileAppender;
            CleanLogs(appender, rollingFileName);
        }

        /// <summary>
        /// Deletes any log that older than the number of 
        /// maxSizeRollingBackups element in the web.config
        /// </summary>
        /// <param name="appender"></param>
        private void CleanLogs(RollingFileAppender appender, string rollingFileName)
        {
            string dir = Path.GetDirectoryName(appender.File);
            if (dir == null)
            {
                return;
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(dir);
            if (!directoryInfo.Exists)
                return;
            FileInfo[] fileInfos = directoryInfo.GetFiles(rollingFileName);
            if (fileInfos.Length == 0)
            {
                return;
            }
            DateTime maxDate = DateTime.Now.AddDays(-appender.MaxSizeRollBackups);
            //start looking for all file logs older than MaxSizeRollBackups days old
            //and start deleting them
            foreach (FileInfo info in fileInfos.Where(info => info.LastWriteTime < maxDate))
            {
                try
                {
                    Result.MessageBuilder.Append($"Try to delete: {info.FullName}");
                    info.Delete();
                    Result.MessageBuilder.Append(" - success").Append(Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to delete log file: {info.Name}. Exception:{ex.Message}");
                }
            }
        }
    }
}

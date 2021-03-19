using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Application;
using HealthMonitor.Domain.Configuration;
using HealthMonitor.Domain.Configuration.Interfaces;
using HealthMonitor.Domain.Results;
using HealthMonitor.Services;
using HealthMonitor.WindowsServices.Common;
using HealthMonitor.WindowsServices.Components;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;
using Ninject;

namespace HealthMonitor.WindowsServices
{
    public partial class HealthMonitorService : ServiceBase
    {
        //Log4Net logger
        private static readonly ILog Log = LogManager.GetLogger(typeof(HealthMonitorService));
        // from config: appender-ref ref="RollingFile"/>
        private static readonly string[] RollingFileAppenderNames = new string[] { "RollingFile" };

        private readonly IList<IHealthCheck> healthChecks;
        private HealthCheckList healthCheckList;
        private IKernel kernel;

        public HealthMonitorService()
        {
            InitializeComponent();
            healthChecks = new List<IHealthCheck>();
        }

        protected override void OnStart(string[] args)
        {
            InitializeLogging();
            Log.Debug("OnStart");

            try
            {
                HealthChecks healthCheckConfigs = HealthChecksSection.GetConfig().HealthChecks;
                for (int ii = 0; ii < healthCheckConfigs.Count; ii++)
                {
                    healthChecks.Add(healthCheckConfigs[ii]);
                }

                kernel = CreateAndBindKernel();
                healthCheckList = new HealthCheckList(healthChecks, kernel);
                healthCheckList.HealthCheckError += HealthCheckList_HealthCheckError;
                healthCheckList.SeriousResult += HealthCheckList_SeriousResult;
                healthCheckList.Start();
            }
            catch (ScheduleProviderException ex)
            {
                Log.ErrorFormat($"Failed: {ex.ProviderName}");
                Log.Error(ex);
                OnStop();
            }
            finally
            {
                Log.Debug("Finished OnStart");
            }
        }

        private void HealthCheckList_SeriousResult(object sender, EventArgs e)
        {
            try
            {
                var args = e as SeriousEventArgs;
                if (args == null)
                    throw new InvalidCastException("Cannot determin health check result");
                //SendEmailMessage(args.HealthCheckName, "Serious Health Check Result", () => args.HealthCheckResult.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void HealthCheckList_HealthCheckError(object sender, EventArgs e)
        {
            try
            {
                var args = e as HealthCheckErrorArgs;
                if (args == null)
                    throw new InvalidCastException("Cannot determin health check result");
                SendEmailMessage(args.HealthCheckName, "Error Found in Health Check Service",
                () => string.Concat(args.HealthCheckException.Message,Environment.NewLine, args.HealthCheckException.StackTrace));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                OnStop();
            }
        }

        private void SendEmailMessage(string healthCheckName, string subject, Func<string> messageFunc)
        {
            //var email = kernel.Get<EmailNotification>();
            var healthCheckConfig = healthChecks.First(x => x.Name.Equals(healthCheckName));
            string message = messageFunc.Invoke();
            Log.InfoFormat($"Mock :-) Email sent to {healthCheckConfig.To}\r\n from {healthCheckConfig.From}\r\n Message: {message}");
            //if (email.SendEmail(healthCheckConfig.To, healthCheckConfig.From, subject, message))
            //{
            //    Log.ErrorFormat($"Failed to send email to {healthCheckConfig.To}\r\n from {healthCheckConfig.From}\r\n Message: {message}");
            //}
        }

        protected override void OnStop()
        {
            healthCheckList.Stop();
        }

        private static void InitializeLogging()
        {
            //Read the configuration
            XmlConfigurator.Configure();
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            CleanupLogs(hierarchy);
            Log.DebugFormat($"String Health Monitor windows service");
        }

        /// <summary>
        /// Log4Net does not support rolling files maximum by date. This method does the cleanup.
        /// </summary>
        /// <param name="hierarchy"></param>
        private static void CleanupLogs(Hierarchy hierarchy)
        {
            if (hierarchy == null)
            {
                return;
            }
            var st = hierarchy.Root.GetAppender(RollingFileAppenderNames[0]);// ("RollingFile");
            RollingFileAppenderNames
                .Select(name => hierarchy.Root.GetAppender(name)).OfType<RollingFileAppender>()
                .ToList()
                .ForEach(CleanLogs);
        }

        /// <summary>
        /// Deletes any log that older than the number of 
        /// maxSizeRollingBackups element in the web.config
        /// </summary>
        /// <param name="appender"></param>
        private static void CleanLogs(RollingFileAppender appender)
        {
            string dir = Path.GetDirectoryName(appender.File);
            if (dir == null)
            {
                return;
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(dir);
            if (!directoryInfo.Exists)
                return;
            FileInfo[] fileInfos = directoryInfo.GetFiles("HealthMonitor-log*.txt");
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
                    info.Delete();
                }
                catch (Exception ex)
                {
                    if (Log != null)
                    {
                        Log.Error($"Failed to delete log file: {info.Name}. Exception:{ex.Message}");
                    }
                }
            }
        }
        /// <summary>
        /// Creates the Ninject Kernel and binds any short for name to an object
        /// </summary>
        /// <returns>An implementation of <see cref="IKernel"/> object</returns>
        private static StandardKernel CreateAndBindKernel()
        {
            StandardKernel kernel = new StandardKernel(new ServiceModule());

            // any implementation where you don't want to use the full assembly can go here
            kernel.Bind<ApplicationHealthCheck>().To<StopStartWindowsServices>().Named("stop-start-windows-services");
            return kernel;
        }
    }
}

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
using HealthMonitor.Application.Common;
using HealthMonitor.Application.DoHealthChecks;
using HealthMonitor.Domain;
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
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // from config: appender-ref ref="RollingFile"/>
        private static readonly string[] RollingFileAppenderNames = new string[] { "RollingFile" };

        private readonly IList<IHealthCheck> healthChecks;
        private HealthCheckList healthCheckList;
        private IKernel kernel;
        private static readonly HealthChecksSection HealthChecksSection = HealthChecksSection.GetConfig();

        public HealthMonitorService()
        {
            Log.Debug("Initialize HealthMonitorService");
            InitializeComponent();
            healthChecks = new List<IHealthCheck>();
        }

        protected override void OnStart(string[] args)
        {
            Log.Debug("OnStart");
            if (HealthChecksSection == null || HealthChecksSection.HealthChecks == null || HealthChecksSection.HealthChecks.Count == 0)
            {
                Log.Error(SystemConstants.MISSING_HEALTH_CHECK_SECTION);
            }

            try
            {
                HealthChecks healthCheckConfigs = HealthChecksSection.GetConfig().HealthChecks;
                for (int ii = 0; ii < healthCheckConfigs.Count; ii++)
                {
                    healthChecks.Add(healthCheckConfigs[ii]);
                }

                IKernel kernel = new BindKernelwithHealthChecks(new StandardKernel(new ServiceModule()), HealthChecksSection.HealthChecks).Bind();
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
    }
}

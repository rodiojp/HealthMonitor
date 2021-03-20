using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Application;
using HealthMonitor.Domain;
using HealthMonitor.Domain.Configuration;
using HealthMonitor.Domain.Results;
using HealthMonitor.Services;
using Ninject;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;
using log4net.Appender;
using System.IO;

namespace HealthMonitor.ConsoleApp
{
    class Program
    {
        //Log4Net logger
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // from config: appender-ref ref="RollingFile"/>
        private static readonly string[] RollingFileAppenderNames = new string[] { "RollingFile" };
        private static readonly HealthChecksSection HealthChecksSection = HealthChecksSection.GetConfig();

        static void Main(string[] args)
        {
            Log.Debug("Start Application");
            if (HealthChecksSection == null || HealthChecksSection.HealthChecks == null || HealthChecksSection.HealthChecks.Count == 0)
            {
                Log.Error(SystemConstants.MISSING_HEALTH_CHECK_SECTION);
                return;
            }
            if (args.Length == 0)
            {

                Console.WriteLine("Enter one of HealthChecksSection Names:");
                foreach (object key in HealthChecksSection.HealthChecks)
                {
                    var healthCheck = key as HealthCheck;
                    Console.WriteLine($"\"{healthCheck.Name}\"");
                }
            }
            else if (args.Length > 1)
            {
                Console.WriteLine($"Expected only 1 argument but found {args.Length}");
            }
            else
            {
                string healthCheckName = args[0];

                InitializeLogging();
                Log.DebugFormat($"Run The Health Check: \"{args[0]}\"");

                try
                {
                    HealthChecksSection config = HealthChecksSection.GetConfig();
                    IEnumerable<BoundsLimit> bounds = config.HealthChecks[healthCheckName].HealthCheckParameters.ReturnAll()
                                                            .Select(boundsLimit =>
                                                                new BoundsLimit(boundsLimit.Name, boundsLimit.Type, boundsLimit.Value));
                    IKernel kernel = CreateAndBindKernel();
                    ApplicationHealthCheck app = kernel.Get<ApplicationHealthCheck>(healthCheckName);
                    if (app == null)
                    {
                        throw new ArgumentException($"{healthCheckName} is not a valid Health Check Name");
                    }
                    HealthMonitorResult healthMonitorResult = app.DoHealthCheck(bounds);

                    Log.DebugFormat(healthMonitorResult.ToString());

                    if (healthMonitorResult.IsSerious)
                    {
                        //SendNotification(kernel, healthMonitorResult, config.HealthChecks[healthCheckName]);
                        Console.WriteLine(healthMonitorResult);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
                finally
                {
                    Log.DebugFormat($"Finished health check: {healthCheckName}");
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
            foreach (object key in HealthChecksSection.HealthChecks)
            {
                var healthCheck = key as HealthCheck;
                switch (healthCheck.Type)
                {
                   case "StopStartWindowsServices":
                        kernel.Bind<ApplicationHealthCheck>().To<StopStartWindowsServices>().Named(healthCheck.Name);
                        Log.DebugFormat($"Added the Health Check: \"{healthCheck.Name}\" \"{healthCheck.Type}\"");
                        break;
                }
            }

            // any implementation where you don't want to use the full assembly can go here
            return kernel;
        }

        private static void InitializeLogging()
        {
            //Read the configuration
            //XmlConfigurator.Configure();
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            CleanupLogs(hierarchy);
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
    }
}

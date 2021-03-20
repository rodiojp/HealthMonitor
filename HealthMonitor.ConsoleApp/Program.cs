using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using HealthMonitor.Application.DoHealthChecks;
using HealthMonitor.Application.Common;

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
                Log.DebugFormat($"Run The Health Check: \"{args[0]}\"");
                try
                {
                    HealthChecksSection config = HealthChecksSection.GetConfig();
                    IEnumerable<BoundsLimit> bounds = config.HealthChecks[healthCheckName].HealthCheckParameters.ReturnAll()
                                                            .Select(boundsLimit =>
                                                                new BoundsLimit(boundsLimit.Name, boundsLimit.Type, boundsLimit.Value));
                    IKernel kernel = new BindKernelwithHealthChecks(new StandardKernel(new ServiceModule()), HealthChecksSection.HealthChecks).Bind();
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
    }
}

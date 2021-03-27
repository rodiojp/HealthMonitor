using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Application.DoHealthChecks;
using HealthMonitor.Domain.Configuration;
using HealthMonitor.Services.Common;
using HealthMonitor.Services.Interfaces;
using HealthMonitor.WindowsServices.Interfaces;
using log4net;
using Ninject;
using Ninject.Extensions.NamedScope;

namespace HealthMonitor.Application.Common
{
    public class BindKernelwithHealthChecks
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private StandardKernel Kernel { get; set; }
        private HealthChecks HealthChecks { get; set; }
        private BindKernelwithHealthChecks()
        { }
        public BindKernelwithHealthChecks(StandardKernel kernel, HealthChecks healthChecks)
        {
            if (kernel == null || healthChecks == null)
            {
                throw new ArgumentNullException("kernel or HealthChecks parameter is null");
            }
            Kernel = kernel;
            HealthChecks = healthChecks;
        }

        /// <summary>
        /// Creates the Ninject Kernel and binds any short for name to an object
        /// </summary>
        /// <returns>An implementation of <see cref="IKernel"/> object</returns>
        public StandardKernel Bind()
        {
            foreach (object key in HealthChecks)
            {
                var healthCheck = key as HealthCheck;
                switch (healthCheck.Type)
                {
                    case "StopStartWindowsServices":
                        Kernel.Bind<ApplicationHealthCheck>().To<StopStartWindowsServices>().Named(healthCheck.Name);
                        break;
                    case "RollingFileDateLog":
                        Kernel.Bind<ApplicationHealthCheck>().To<RollingFileDateLog>().Named(healthCheck.Name);
                        break;
                    case "SpaceOpimizationCheck":
                        Kernel.Bind<ApplicationHealthCheck>().To<SpaceOpimizationCheck>().Named(healthCheck.Name)
                            .WithConstructorArgument("archiveService", new ArchivingService())
                            .WithConstructorArgument("spaceSavingsService", new SpaceOptimizationService());
                        break;
                }
                Log.DebugFormat($"Added the Health Check: \"{healthCheck.Name}\" as \"{healthCheck.Type}\"");
            }
            // any implementation where you don't want to use the full assembly can go here
            return Kernel;
        }
    }
}
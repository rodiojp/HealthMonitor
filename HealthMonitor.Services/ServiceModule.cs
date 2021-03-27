using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Services.Common;
using HealthMonitor.Services.Interfaces;
using HealthMonitor.WindowsServices.Interfaces;
using Ninject.Modules;

namespace HealthMonitor.Services
{
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            //Bind<ISpaceOptiomizationService>().To<SpaceOptimizationService>();
            // add parameters for SpaceOptimizationService 
            //Bind<IArchiveService>().To<ArchivingService>();
            //Bind<ISpaceOptimizationService>().To<SpaceOptimizationService>();
        }
    }
}

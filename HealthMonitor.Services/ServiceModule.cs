using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;

namespace HealthMonitor.Services
{
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            //Bind<ISpaceOptiomizationService>().To<SpaceOptimizationService>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain.Extensions;
using log4net;

namespace HealthMonitor.WindowsServices
{
    static class Program
    {
        //Log4Net logger
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            try
            {
                Log.Debug("Start WindowsServices");
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new HealthMonitorService()
                };
                Log.Debug("Run WindowsServices");
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToLogString());
            }

        }
    }
}

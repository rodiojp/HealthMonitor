using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain;
using HealthMonitor.Domain.Extensions;
using HealthMonitor.Domain.Results;

namespace HealthMonitor.Application.DoHealthChecks
{
    public class StopStartWindowsServices : ApplicationHealthCheck
    {
        protected const string ServiceTimeoutMillisecondsParameter = "ServiceTimeoutMilliseconds";
        protected TimeSpan ServiceTimeout { get; set; } = TimeSpan.FromMilliseconds(5000);
        public StopStartWindowsServices()
        {
            Name = "Stop Start Windows Serivces";
            HealthType = HealthType.WindowsService;
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
                ServiceController[] servicesToRestart = checkParameters.Where(x => x.Type.Equals("service"))
                                                                       .Select(y => new ServiceController(y.Value))
                                                                       .ToArray();
                return RestartServices(servicesToRestart);
            }
            catch (Exception e)
            {
                return new HealthMonitorResult(Name, HealthType, ResultStatus.Error, e.ToLogString());
            }
        }

        public HealthMonitorResult RestartServices(ServiceController[] services)
        {
            HealthMonitorResult resultToStop = StopServices(services);
            resultToStop.MessageBuilder.Insert(0, Environment.NewLine);
            resultToStop.MessageBuilder.Insert(0, "The Result of Stopping Services:");
            if (resultToStop.IsSerious)
                return resultToStop;
            HealthMonitorResult resultToStart = StartServices(services);
            resultToStart.MessageBuilder.Insert(0, Environment.NewLine);
            resultToStart.MessageBuilder.Insert(0, "The Result of Starting Services:");
            if (resultToStart.IsSerious)
                return resultToStart;
            HealthMonitorResult result = new HealthMonitorResult(Name, HealthType, ResultStatus.Warning, resultToStop.ToString());
            result.MessageBuilder.Append(Environment.NewLine);
            result.MessageBuilder.Append(resultToStart.ToString());
            return result;
        }
        public HealthMonitorResult StopServices(ServiceController[] services)
        {
            HealthMonitorResult result = new HealthMonitorResult(Name, HealthType, ResultStatus.Information);
            result.MessageBuilder.Append($"Stop windows services count: {services.Length}")
                                 .Append(Environment.NewLine);
            foreach (ServiceController service in services)
            {
                try
                {
                    result.MessageBuilder.Append($"Stopping the windows service: {service.DisplayName}").Append(Environment.NewLine);
                    service.Stop();
                    //give a few seconds
                    service.WaitForStatus(ServiceControllerStatus.Stopped, ServiceTimeout);
                    result.MessageBuilder.Append($"Windows service: {service.DisplayName} - has been stopped").Append(Environment.NewLine);
                }
                catch (Exception e)
                {
                    result.Status = ResultStatus.Error;
                    result.MessageBuilder.Append(e.ToLogString());
                    return result;
                }
            }
            return result;
        }
        public HealthMonitorResult StartServices(ServiceController[] services)
        {
            HealthMonitorResult result = new HealthMonitorResult(Name, HealthType, ResultStatus.Information);
            result.MessageBuilder.Append($"Start windows services count: {services.Length}")
                                 .Append(Environment.NewLine);
            foreach (ServiceController service in services)
            {
                try
                {
                    result.MessageBuilder.Append($"Starting the windows service: {service.DisplayName}").Append(Environment.NewLine);
                    service.Start();
                    //give a few seconds
                    service.WaitForStatus(ServiceControllerStatus.Running, ServiceTimeout);
                    result.MessageBuilder.Append($"Windows service: {service.DisplayName} - has been started").Append(Environment.NewLine);
                }
                catch (Exception e)
                {
                    result.Status = ResultStatus.Error;
                    result.MessageBuilder.Append(e.ToLogString());
                    return result;
                }
            }
            return result;
        }
    }
}

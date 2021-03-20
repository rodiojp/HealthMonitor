using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HealthMonitor.Application.DoHealthChecks;
using HealthMonitor.Domain;
using HealthMonitor.Domain.Configuration;
using HealthMonitor.Domain.Configuration.Interfaces;
using HealthMonitor.WindowsServices.Common;
using log4net;
using Ninject;

namespace HealthMonitor.WindowsServices.Components
{
    /// <summary>
    /// Manager to run a list of HealthChecks
    /// </summary>
    /// <remarks>
    /// Class: HealthCheckList.cs
    /// 
    /// How it works: Binds a list of HealthCheckTimer objects, one per health check,
    /// to its health check parameter and schedule. This class will manage when 
    /// to fire, when to start and stop 
    /// </remarks>
    public class HealthCheckList
    {
        //Log4Net logger
        private static readonly ILog Log = LogManager.GetLogger(typeof(HealthCheckList));

        //Dictionary to track all the configured health checks and its configurations to a timer
        private readonly IDictionary<HealthCheckTimer, IHealthCheck> timers = new Dictionary<HealthCheckTimer, IHealthCheck>();

        //tracks what health checks are running
        private readonly IList<string> runningHealthChecks;

        private readonly IKernel kernel;

        //A flag that indicates that the poller list is shutting down
        private ManualResetEvent stoppingEvent;

        /// <summary>
        /// Event raised when the Health Check is in serious status
        /// </summary>
        public event EventHandler SeriousResult;

        /// <summary>
        /// Event raised when the Health Check processing threw an error
        /// </summary>
        public event EventHandler HealthCheckError;

        protected virtual void OnHealthCheckError(EventArgs e)
        {
            EventHandler handler = HealthCheckError;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnSeriousResult(EventArgs e)
        {
            EventHandler handler = SeriousResult;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        /// Indicates whether the Windows Service is Running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                lock (this)
                {
                    return (stoppingEvent != null);
                }
            }
        }

        /// <summary>
        /// Indicates when a Health Check is Processing
        /// </summary>
        public bool IsHealthCheckRunning
        {
            get
            {
                return runningHealthChecks.Any();
            }
        }

        public HealthCheckList(IEnumerable<IHealthCheck> healthChecks, IKernel kernel)
        {
            this.kernel = kernel;
            runningHealthChecks = new List<string>();

            //iterate through configurations and initialize the
            //HealthCheckTimer and add it ot the dictionary
            //that tracks timers and health check parameters
            foreach (var healthCheck in healthChecks)
            {
                HealthCheckTimer timer = new HealthCheckTimer(ScheduleProviderFactory.Build(healthCheck.ScheduleParams));
                timer.Elapsed += timer_Elapsed;
                timer.AutoReset = false;
                Log.DebugFormat($"created timer for {healthCheck.Name}");
                timers.Add(timer, healthCheck);
            }
        }

        /// <summary>
        /// Start All the Health Checks. This method only starts
        /// its configured run time so that its next disignated start
        /// the health check will run
        /// </summary>
        public void Start()
        {
            lock (this)
            {
                if (stoppingEvent != null)
                {
                    throw new InvalidOperationException("Health Checks are already running");
                }
                stoppingEvent = new ManualResetEvent(false);
                foreach (var timer in timers)
                {
                    StartHealthCheckTimer(timer.Value.Name, timer.Key);
                }

            }
            Log.InfoFormat($"{timers.Count} Health Check(s) started");
        }

        /// <summary>
        /// Stop all the Health Checks. if any number of health checks are running,
        /// it will wait until it has finished
        /// </summary>
        public void Stop()
        {
            if (timers == null)
                return;

            Log.InfoFormat($"{timers.Count} Health Check(s) shutting down...");

            lock (this)
            {
                stoppingEvent.Set();

                foreach (var timer in timers)
                {
                    StopHealthCheckTimer(timer.Value.Name, timer.Key);
                }
            }
            Log.InfoFormat($"{timers.Count} Health Check(s) stopped");

            if (runningHealthChecks == null) return;

            while (runningHealthChecks.Any())
            {
                Log.DebugFormat($"waiting for {runningHealthChecks.Count} running health to finish");
                // Wait for one of the worker threads to signal that a task has finished
                Monitor.Wait(this);
            }
            runningHealthChecks.Clear();
        }
        /// <summary>
        /// Initialize and start a health check timer
        /// </summary>
        /// <param name="name">The name of the Health Check</param>
        /// <param name="timer">a <see cref="HealthCheckTimer"/> object</param>
        private static void StartHealthCheckTimer(string name, HealthCheckTimer timer)
        {
            timer.Start();
            Log.DebugFormat($"started timer for {name} with {timer.Interval} (next run:{timer.NextRunTime}) ");
        }

        /// <summary>
        /// Stop a Health Check timer
        /// </summary>
        /// <param name="name">The name of the Health Check</param>
        /// <param name="timer">a <see cref="HealthCheckTimer"/> object</param>
        private static void StopHealthCheckTimer(string name, HealthCheckTimer timer)
        {
            timer.Stop();
            Log.DebugFormat($"Stopping Health Check: {name}");
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var timer = sender as HealthCheckTimer;
            if (timer == null)
                throw new InvalidOperationException("timer is null");
            var healthCheckName = timers[timer].Name;
            try
            {
                Log.DebugFormat($"timer for {healthCheckName} elapsed");

                //only do this if conditions for the event is right
                if (!EnterHealthCheck(healthCheckName)) return;
                Log.DebugFormat($"Processing {healthCheckName} at {DateTime.Now}");
                timer.Enabled = false;
                var bounds = timers[timer].HealthCheckParams.Select(boundsLimit => new BoundsLimit(boundsLimit.Name,
                                                                                                   boundsLimit.Type,
                                                                                                   boundsLimit.Value));
                //use Ninject kernel to create the health check for us                                                                                  boundsLimit.Value));
                var app = kernel.Get<ApplicationHealthCheck>(healthCheckName);
                if (app == null)
                    throw new ArgumentException($"{healthCheckName} is not app valid health check");
                var healthCheckResult = app.DoHealthCheck(bounds);
                Log.DebugFormat(healthCheckResult.ToString());
                if (healthCheckResult.IsSerious)
                    OnSeriousResult(new SeriousEventArgs
                    {
                        HealthCheckResult = healthCheckResult,
                        HealthCheckName = healthCheckName
                    });
            }
            catch (Exception ex)
            {
                OnHealthCheckError(
                     new HealthCheckErrorArgs
                     {
                         HealthCheckException = ex,
                         HealthCheckName = healthCheckName
                     });
            }
            finally
            {
                ExitTask(healthCheckName, timer);
            }
        }

        /// <summary>
        /// Exiting event to Reset HealthCheckTimer and managing which
        /// </summary>
        /// <param name="healthCheckName">The name of the Health Check</param>
        /// <param name="timer">a<see cref="HealthCheckTimer"/> object</param>
        private void ExitTask(string healthCheckName, HealthCheckTimer timer)
        {
            lock (this)
            {
                runningHealthChecks.Remove(healthCheckName);
                //don't recalculate next run time if stopping flag has been
                if (!stoppingEvent.WaitOne(0))
                {
                    timer.Reset();
                    Log.DebugFormat($"timer for {healthCheckName} with {timer.Interval}ms interval (next run:{timer.NextRunTime})");
                }
                Monitor.PulseAll(this);
            }
        }

        /// <summary>
        /// Check to make sure Health Chekc is not already running
        /// </summary>
        /// <param name="healthCheckName"></param>
        /// <returns></returns>
        private bool EnterHealthCheck(string healthCheckName)
        {
            var result = false;
            lock (this)
            {
                if (stoppingEvent.WaitOne(0))
                {
                    Log.Debug("Helath Check Service is stopping");
                }
                else if (runningHealthChecks.Any(x => x.Equals(healthCheckName)))
                {
                    Log.DebugFormat($"health check {healthCheckName} is already running");
                }
                else
                {
                    runningHealthChecks.Add(healthCheckName);
                    result = true;
                }
            }
            return result;
        }
    }
}

using System;
using System.Timers;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    public abstract class AbstractManagerJob
    {
        private readonly int _sleepTimeInMinutes;

        private bool _isJobRunnig;

        private System.Timers.Timer _timer;

        public AbstractManagerJob(int sleepTimeInMinutes)
        {
            _sleepTimeInMinutes = sleepTimeInMinutes;

            _timer = new System.Timers.Timer
            {
                AutoReset = false,
                Interval = TimeSpan.FromMinutes(_sleepTimeInMinutes).TotalMilliseconds
            };

            _timer.Elapsed += new ElapsedEventHandler(UpdateManagerJobs);
        }

        public virtual void Start()
        {
            if (!_isJobRunnig)
            {
                _timer.Start();

                _isJobRunnig = true;
            }
        }

        public virtual void ResetTimer()
        {
            _timer.Start();
        }

        public abstract void UpdateManagerJobs(object sender, ElapsedEventArgs e);

    }
}
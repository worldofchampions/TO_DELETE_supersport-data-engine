using Hangfire;
using System;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    public class LogsJobCleanupManager
    {
        /// <summary>
        /// Will queue a job for deletion from hangfire when given minute elapses.
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <param name="jobId"></param>
        /// <param name="minuteToDeleteOn"></param>
        public static void QueueJobForDeletion(Guid tournamentId, string jobId, int minuteToDeleteOn)
        {
            var timer = new System.Timers.Timer
            {
                AutoReset = false,
                Interval = TimeSpan.FromMinutes(minuteToDeleteOn).TotalMilliseconds
            };

            timer.Elapsed += delegate
            {
                DeleJobFromHangfire(jobId);

                timer.Stop();
            };

            timer.Start(); ;
        }

        /// <summary>
        /// Will delete a job from hangfire storage if the job exists.
        /// </summary>
        /// <param name="jobId"></param>
        private static void DeleJobFromHangfire(string jobId)
        {
            RecurringJob.RemoveIfExists(jobId);
        }
    }
}

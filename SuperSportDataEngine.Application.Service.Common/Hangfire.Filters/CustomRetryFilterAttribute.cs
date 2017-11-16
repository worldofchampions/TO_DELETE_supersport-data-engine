using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Practices.Unity;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
using SuperSportDataEngine.Common.Logging;
using System;

namespace SuperSportDataEngine.Application.Service.Common.Hangfire.Filters
{
    public class CustomRetryFilterAttribute : JobFilterAttribute, IElectStateFilter
    {
        private const int DefaultRetryAttempts = 10;
        private int _attempts;
        private static int _defaultRetryPeriod = 5;
        private IUnityContainer _container;
        private ILoggingService _logger;

        public CustomRetryFilterAttribute(IUnityContainer container, int defaultRetryPeriod = 5)
        {
            _container = container;
            _logger = _container.Resolve<ILoggingService>();

            Attempts = DefaultRetryAttempts;
            LogEvents = true;
            OnAttemptsExceeded = AttemptsExceededAction.Fail;
            _defaultRetryPeriod = defaultRetryPeriod;
        }

        public int Attempts
        {
            get { return _attempts; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", "Attempts value must be equal or greater than zero.");
                }

                _attempts = value;
            }
        }
        public AttemptsExceededAction OnAttemptsExceeded { get; set; }
        public bool LogEvents { get; set; }
        public void OnStateElection(ElectStateContext context)
        {
            if (context.CandidateState is EnqueuedState enqueuedState)
            {
                var qn = context.GetJobParameter<string>("QueueName");
                if (!String.IsNullOrWhiteSpace(qn))
                {
                    enqueuedState.Queue = qn;
                }
                else
                {
                    context.SetJobParameter("QueueName", HangfireQueueConfiguration.HighPriority);
                }
            }

            if (context.CandidateState is FailedState failedState)
            {
                var retryAttempt = context.GetJobParameter<int>("RetryCount") + 1;

                if (retryAttempt <= Attempts)
                {
                    ScheduleAgainLater(context, retryAttempt, failedState);
                }
                else if (retryAttempt > Attempts && OnAttemptsExceeded == AttemptsExceededAction.Delete)
                {
                    //_logger.Error("Hangfire job has failed. It is being deleted. Job name = {0}", context.BackgroundJob.Job.Method.Name);
                    TransitionToDeleted(context, failedState);
                }
                else if(retryAttempt > Attempts)
                {
                    _logger.Error("Hangfire job has failed. Job name = {0}", context.BackgroundJob.Job.Method.Name);
                }
            }
        }

        /// <summary>
        /// Schedules the job to run again later. See <see cref="SecondsToDelay"/>.
        /// </summary>
        /// <param name="context">The state context.</param>
        /// <param name="retryAttempt">The count of retry attempts made so far.</param>
        /// <param name="failedState">Object which contains details about the current failed state.</param>
        private void ScheduleAgainLater(ElectStateContext context, int retryAttempt, FailedState failedState)
        {
            var delay = TimeSpan.FromSeconds(SecondsToDelay(retryAttempt));
            context.SetJobParameter("RetryCount", retryAttempt);
            // If attempt number is less than max attempts, we should
            // schedule the job to run again later.
            context.CandidateState = new ScheduledState(delay)
            {
                Reason = String.Format("Retry attempt {0} of {1}", retryAttempt, Attempts)
            };
        }
        /// <summary>
        /// Transition the candidate state to the deleted state.
        /// </summary>
        /// <param name="context">The state context.</param>
        /// <param name="failedState">Object which contains details about the current failed state.</param>
        private void TransitionToDeleted(ElectStateContext context, FailedState failedState)
        {
            context.CandidateState = new DeletedState
            {
                Reason = string.Format("Automatic deletion after retry count exceeded {0}", Attempts)
            };
        }
        // delayed_job uses the same basic formula
        private static int SecondsToDelay(long retryCount)
        {
            return _defaultRetryPeriod;
        }
    }
}
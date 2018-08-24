using Moq;
using Hangfire;
using SuperSportDataEngine.Application.Service.SchedulerClient.FixedSchedule;
using SuperSportDataEngine.ApplicationLogic.Services;
using NUnit.Framework;
using Hangfire.Common;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.Common.Logging;
using SuperSportDataEngine.Logging.NLog.Logging;
using Unity;
using Unity.Injection;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.Tests.ScheduledManagerTests
{
    [Category("FixedScheduledManagerJob")]
    public class FixedScheduledManagerJobTest
    {
        [TestCase("Rugby→StatsProzone→FixedScheduleJob→ReferenceData", "0 2 * * *", "normal_priority")]
        [TestCase("Rugby→StatsProzone→FixedScheduleJob→Fixtures", "0 2 * * *", "normal_priority")]
        [TestCase("Rugby→StatsProzone→FixedScheduleJob→Logs→ActiveTournaments", "0 2 * * *", "normal_priority")]
        [TestCase("Rugby→StatsProzone→FixedScheduleJob→Logs→CurrentTournaments", "*/15 * * * *", "normal_priority")]
        [TestCase("Rugby→StatsProzone→FixedScheduleJob→Results→AllFixtures", "5 2 * * *", "normal_priority")]
        [TestCase("Rugby→StatsProzone→FixedScheduleJob→Results→EndedFixtures", "0 * * * *", "normal_priority")]
        [TestCase("Rugby→StatsProzone→FixedScheduleJob→Results→CurrentDayFixtures", "*/15 * * * *", "high_priority")]
        public void AllJobsAreCreatedOnHangfire(
            string hangfireJobId,
            string hangfireCronExpression,
            string queue)
        {
            // Mock the Hangfire client, ingest service and job manager.
            var mockRugbyService = new Mock<IRugbyService>();
            var mockIngestWorkerService = new Mock<IRugbyIngestWorkerService>();
            var mockRecurringJobManager = new Mock<IRecurringJobManager>();

            RecurringJobOptions options = new RecurringJobOptions();

            mockRecurringJobManager.Setup(c =>
                c.AddOrUpdate(hangfireJobId, It.IsAny<Job>(), hangfireCronExpression, It.IsAny<RecurringJobOptions>()))
                 .Callback<string, Job, string, RecurringJobOptions>(
                 (name, job, cron, opt) => {
                     options.QueueName = opt.QueueName;
                     options.TimeZone = opt.TimeZone;
                 });

            var mockLogger = new Mock<ILoggingService>();

            // Mock and set the job storage.
            JobStorage.Current = new Mock<JobStorage>().Object;

            // Mock the Unity container and set the dependencies to the mocked objects.
            var container = new UnityContainer();

            container.RegisterType<ILoggingService>(
                new InjectionFactory((x) => mockLogger.Object));

            container.RegisterType<IRugbyIngestWorkerService>(
                new InjectionFactory((x) => mockIngestWorkerService.Object));

            container.RegisterType<IRecurringJobManager>(
                new InjectionFactory((x) => mockRecurringJobManager.Object));

            container.RegisterType<IRugbyService>(
                new InjectionFactory((x) => mockRugbyService.Object));

            // Create the object to invoke method on.
            var fixedScheduledJob = new FixedScheduledJob(container);

            // Call method.
            fixedScheduledJob.UpdateRecurringJobDefinitions();

            // Verify that the AddOrUpdate method is called
            // with the correct Job ID and Cron expression.
            mockRecurringJobManager
                .Verify(
                    m => m.AddOrUpdate(
                            hangfireJobId,
                            It.IsAny<Job>(),
                            hangfireCronExpression,
                            It.IsAny<RecurringJobOptions>()),
                          Times.AtLeastOnce());

            Assert.AreEqual(queue, options.QueueName);
        }
    }
}

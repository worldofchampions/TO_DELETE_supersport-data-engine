using Moq;
using Hangfire;
using SuperSportDataEngine.Application.Service.SchedulerClient.FixedSchedule;
using Microsoft.Practices.Unity;
using SuperSportDataEngine.ApplicationLogic.Services;
using NUnit.Framework;
using Hangfire.Common;
using System.Linq.Expressions;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.Tests
{
    public partial class FixedScheduledJob_Test
    {
        //[Test]
        [TestCase("FixedScheduleJob→ReferenceData", "0 2 * * *")]
        [TestCase("FixedScheduleJob→Fixtures", "5 2 * * *")]
        [TestCase("FixedScheduleJob→Logs→ActiveTournaments", "5 2 * * *")]
        [TestCase("FixedScheduleJob→Logs→CurrentTournaments", "0 */1 * * *")]
        [TestCase("FixedScheduleJob→Results→AllFixtures", "5 2 * * *")]
        [TestCase("FixedScheduleJob→Results→EndedFixtures", "0 */1 * * *")]
        [TestCase("FixedScheduleJob→Results→CurrentDayFixtures", "*/15 * * * *")]
        public void WhenUpdateFixedJobsCalled_AddsJobToHangfire(
            string hangfireJobId,
            string hangfireCronExpression)
        {
            // Mock the Hangfire client, ingest service and job manager.
            var client = new Mock<IBackgroundJobClient>();
            var mockIngestWorkerService = new Mock<IRugbyIngestWorkerService>();
            var mockRecurringJobManager = new Mock<IRecurringJobManager>();

            // Mock and set the job storage.
            var mockStorage = new Mock<JobStorage>();
            JobStorage.Current = new Mock<JobStorage>().Object;

            // Mock the Unity container and set the dependencies to the mocked objects.
            var container = new UnityContainer();

            container.RegisterType<IRugbyIngestWorkerService, RugbyIngestWorkerService>(
                new InjectionFactory((x) => mockIngestWorkerService.Object));

            container.RegisterType<IRecurringJobManager, RecurringJobManager>(
                new InjectionFactory((x) => mockRecurringJobManager.Object));

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
        }
    }
}

using Moq;
using Hangfire;
using SuperSportDataEngine.Application.Service.SchedulerClient.FixedSchedule;
using Microsoft.Practices.Unity;
using SuperSportDataEngine.ApplicationLogic.Services;
using NUnit.Framework;
using Hangfire.Common;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.Tests
{
    public partial class FixedScheduledJob_Test
    {
        [Test]
        public void WhenUpdateFixedJobsCalled_AddsJobToHangfire_IngestingReferenceData()
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
                            "FixedScheduledJob→ReferenceData",
                            It.IsAny<Job>(),
                            "0 2 * * *",
                            It.IsAny<RecurringJobOptions>()),
                          Times.AtLeastOnce());
        }
    }
}

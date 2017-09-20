using Moq;
using Hangfire;
using SuperSportDataEngine.Application.Service.SchedulerClient.FixedSchedule;
using Microsoft.Practices.Unity;
using SuperSportDataEngine.ApplicationLogic.Services;
using NUnit.Framework;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.Tests
{
    public partial class FixedScheduledJob_Test
    {
        [Test]
        public void Add_Job_IngestingReferenceData_To_Hangfire()
        {
            //var client = new Mock<IBackgroundJobClient>();
            //var mockStorage = new Mock<JobStorage>();

            //var container = new UnityContainer();
            //container.RegisterType<IRugbyIngestWorkerService, MockRugbyIngestWorkerService>();

            //JobStorage.Current = new Mock<JobStorage>().Object;
            //var fixedScheduledJob = new FixedScheduledJob(container);

            //fixedScheduledJob.UpdateRecurringJobDefinitions();
        }
    }
}

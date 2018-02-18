using Hangfire;
using Hangfire.Common;
using Microsoft.Practices.Unity;
using Moq;
using NUnit.Framework;
using SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Services;
using SuperSportDataEngine.Common.Logging;
using SuperSportDataEngine.Tests.Common.Repositories.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.Tests.ScheduledManagerTests
{
    [Category("LogsManagerJob")]
    public class LogsManagerJobTest
    {
        Mock<IRugbyService> MockRugbyService;
        Mock<IRugbyIngestWorkerService> MockRugbyIngestWorkerService;
        Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbySeason>> MockSchedulerTrackingSeasonRepository;
        Mock<IRecurringJobManager> MockRecurringJobManager;
        IUnityContainer MockUnityContainer;
        LogsManagerJob LogsManagerJob;
        Mock<ILoggingService> MockLogger;

        [SetUp]
        public void SetUp()
        {
            MockSchedulerTrackingSeasonRepository =
                    new Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbySeason>>(new List<SchedulerTrackingRugbySeason>());

            MockRugbyService = new Mock<IRugbyService>();
            MockRugbyIngestWorkerService = new Mock<IRugbyIngestWorkerService>();
            MockRecurringJobManager = new Mock<IRecurringJobManager>();
            MockUnityContainer = new UnityContainer();

            MockUnityContainer.RegisterType<IUnityContainer>(
                new InjectionFactory((x) => MockUnityContainer));

            MockUnityContainer.RegisterType<IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason>>(
                new InjectionFactory((x) => MockSchedulerTrackingSeasonRepository.Object));

            MockUnityContainer.RegisterType<IRugbyService>(
                new InjectionFactory((x) => MockRugbyService.Object));

            MockUnityContainer.RegisterType<IRugbyIngestWorkerService>(
                new InjectionFactory((x) => MockRugbyIngestWorkerService.Object));

            MockUnityContainer.RegisterType<IRecurringJobManager>(
                new InjectionFactory((x) => MockRecurringJobManager.Object));

            MockLogger = new Mock<ILoggingService>();

            MockUnityContainer.RegisterType<ILoggingService>(
                new InjectionFactory((x) => MockLogger.Object));

            LogsManagerJob =
                new LogsManagerJob(
                        MockRecurringJobManager.Object,
                        MockUnityContainer);
        }
    }
}

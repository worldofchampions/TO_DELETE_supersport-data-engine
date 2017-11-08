using SuperSportDataEngine.Application.Service.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Services;
using Hangfire;

namespace SuperSportDataEngine.Application.Service.Common.Services
{
    public class HangfireJobService : IHangfireJobService
    {
        IRugbyService _rugbyService;
        IRugbyIngestWorkerService _rugbyIngestWorkerService;

        public HangfireJobService(
            IRugbyService rugbyService,
            IRugbyIngestWorkerService rugbyIngestWorkerService)
        {
            _rugbyService = rugbyService;
            _rugbyIngestWorkerService = rugbyIngestWorkerService;
        }

        public async Task IngestPastFixturesNotIngestedYet(CancellationToken cancellationToken)
        {
            var pastFixtures = await _rugbyService.GetFixturesNotIngestedYet();
            foreach(var fixture in pastFixtures)
            {
                BackgroundJob.Enqueue(() => _rugbyIngestWorkerService.IngestLiveMatchDataForFixture(CancellationToken.None, fixture.Id));
            }
        }
    }
}

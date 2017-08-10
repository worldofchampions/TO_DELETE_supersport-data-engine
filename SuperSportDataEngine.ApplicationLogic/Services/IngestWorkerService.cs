using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using System;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class IngestWorkerService : IIngestWorkerService
    {
        private readonly IStatsProzoneIngestService _statsProzoneIngestService;

        public IngestWorkerService(IStatsProzoneIngestService statsProzoneIngestService)
        {
            _statsProzoneIngestService = statsProzoneIngestService;
        }

        public void IngestReferenceData()
        {
            _statsProzoneIngestService.IngestReferenceData();
        }
    }
}

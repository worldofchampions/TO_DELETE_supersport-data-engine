using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class LegacyAuthService : ILegacyAuthService
    {
        private readonly IBaseEntityFrameworkRepository<LegacyZoneSite> _legacyZoneSiteRepository;
        public LegacyAuthService(IBaseEntityFrameworkRepository<LegacyZoneSite> legacyZoneSiteRepository)
        {
            _legacyZoneSiteRepository = legacyZoneSiteRepository;
        }

        public bool IsAuthorised(string authKey, string siteId = null)
        {
            //_legacyZoneSiteRepository.Where
            return false;
        }
    }
}
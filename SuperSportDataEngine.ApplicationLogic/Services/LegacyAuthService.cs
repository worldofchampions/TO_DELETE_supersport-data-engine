using System;
using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Mappers;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SuperSportDataEngine.Application.WebApi.Common.Interfaces;
using SuperSportDataEngine.Common.Logging;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class LegacyAuthService : ILegacyAuthService
    {
        private readonly ILoggingService _loggingService;
        private readonly IBaseEntityFrameworkRepository<LegacyZoneSite> _legacyZoneSiteRepository;
        private readonly IBaseEntityFrameworkRepository<LegacyAuthFeedConsumer> _legacyAuthFeedConsumerRepository;
        private readonly ICache _cache;

        private readonly int _numberOfDaysToKeepAuthKeys;

        public LegacyAuthService(
            ILoggingService loggingService,
            IBaseEntityFrameworkRepository<LegacyZoneSite> legacyZoneSiteRepository,
            IBaseEntityFrameworkRepository<LegacyAuthFeedConsumer> legacyAuthFeedConsumerRepository,
            ICache cache)
        {
            _loggingService = loggingService;
            _legacyZoneSiteRepository = legacyZoneSiteRepository;
            _legacyAuthFeedConsumerRepository = legacyAuthFeedConsumerRepository;
            _cache = cache;

            _numberOfDaysToKeepAuthKeys = int.Parse(ConfigurationManager.AppSettings["NumberOfDaysToKeepAuthKeys"]);

            CacheAuthKeysFromDatabase();
        }

        private async void CacheAuthKeysFromDatabase()
        {
            var legacyAuthFeedKeys = _legacyAuthFeedConsumerRepository.All().ToList();
            const string key = "AUTH_KEYS";

            if (_cache != null && await _cache.GetAsync<IEnumerable<LegacyAuthFeedConsumer>>(key) == null)
            {
                _cache.Add(key, legacyAuthFeedKeys, TimeSpan.FromDays(_numberOfDaysToKeepAuthKeys));
            }
        }

        public async Task<bool> IsAuthorised(string authKey, int siteId = 0)
        {
            int authoriseAttempts = 0;

            BeginAuthorise:

            authoriseAttempts++;
            try
            {
                var legacyAuthFeed = await GetAuthKeyFromCache(authKey);

                // Auth key doesnt exist.
                if (legacyAuthFeed == null)
                {
                    return false;
                }

                if (siteId != 0)
                {
                    // TODO: Temporary auth override until ZoneSite data is seeded.
                    return true;

                    //var legacyZone = _legacyZoneSiteRepository.Where(c => c.Id == siteId).FirstOrDefault();
                    //if (legacyZone == null)
                    //{
                    //    return false;
                    //}
                    //return legacyZone.Feed == legacyAuthFeed.Name.Replace("  ", string.Empty).ToLowerInvariant();
                }

                // Is authorised.
                return true;
            }
            catch (Exception)
            {
                var maxAttempts = int.Parse(ConfigurationManager.AppSettings["MaximumAuthorisationAttempts"]);
                var time = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
                if (authoriseAttempts > maxAttempts)
                {
                    await _loggingService.Error("AuthoriseAttemptFailure." + time,
                    "Request has failed authorisation. " + maxAttempts + " attempts exceeeded.");

                    return false;
                }

                goto BeginAuthorise;
            }
        }

        private async Task<LegacyAuthFeedConsumer> GetAuthKeyFromCache(string authKey)
        {
            const string key = "AUTH_KEYS";

            if (_cache != null)
            {
                var keys = await _cache.GetAsync<IEnumerable<LegacyAuthFeedConsumer>>(key);
                var consumer = keys.FirstOrDefault(k => k.AuthKey == authKey && k.Active) ??
                               _legacyAuthFeedConsumerRepository.FirstOrDefault(
                                   k => k.AuthKey == authKey && k.Active);

                return consumer;
            }

            return _legacyAuthFeedConsumerRepository.FirstOrDefault(
                k => k.AuthKey == authKey && k.Active);
        }

        public async Task<bool> ImportZoneSiteRecords(IEnumerable<LegacyZoneSiteEntity> models)
        {
            var legacyModels = models.Select(entity => Mapper.Map<LegacyZoneSite>(entity));
            _legacyZoneSiteRepository.AddRange(new HashSet<LegacyZoneSite>(legacyModels));
            await _legacyZoneSiteRepository.SaveAsync();

            return true;
        }

        public async Task<bool> ImportAuthFeedRecords(IEnumerable<LegacyAuthFeedConsumerEntity> models)
        {
            var legacyModels = models.Select(entity => LegacyAuthFeedConsumerMapper.MapToModel(entity));
            _legacyAuthFeedConsumerRepository.AddRange(new HashSet<LegacyAuthFeedConsumer>(legacyModels));
            await _legacyAuthFeedConsumerRepository.SaveAsync();

            return true;
        }
    }
}